
-- =========================================================
-- CCL Inventory Management
-- File: 02_stored_procedures.sql
-- Description: Inventory operations
-- Database: ccl_inventory_db
-- =========================================================

-- Register a stock entry
CREATE OR REPLACE PROCEDURE public.sp_register_stock_entry
(
    IN p_product_id BIGINT,
    IN p_quantity INTEGER,
    IN p_notes VARCHAR DEFAULT NULL
)
LANGUAGE plpgsql
AS $$
BEGIN
    IF p_quantity IS NULL OR p_quantity <= 0 THEN
        RAISE EXCEPTION 'Entry quantity must be greater than zero.';
    END IF;

    -- Lock the product row to serialize concurrent movements.
    PERFORM 1
    FROM public.products
    WHERE id = p_product_id
      AND is_active = TRUE
    FOR UPDATE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Product % does not exist or is inactive.',
            p_product_id;
    END IF;

    UPDATE public.products
    SET stock = stock + p_quantity,
        updated_at = CURRENT_TIMESTAMP
    WHERE id = p_product_id;

    INSERT INTO public.inventory_movements
    (
        product_id,
        movement_type,
        quantity,
        notes
    )
    VALUES
    (
        p_product_id,
        'ENTRY',
        p_quantity,
        p_notes
    );
END;
$$;


-- Register a stock exit
CREATE OR REPLACE PROCEDURE public.sp_register_stock_exit
(
    IN p_product_id BIGINT,
    IN p_quantity INTEGER,
    IN p_notes VARCHAR DEFAULT NULL
)
LANGUAGE plpgsql
AS $$
DECLARE
    v_current_stock INTEGER;
BEGIN
    IF p_quantity IS NULL OR p_quantity <= 0 THEN
        RAISE EXCEPTION 'Exit quantity must be greater than zero.';
    END IF;

    -- Lock the product row before validating its stock.
    SELECT stock
    INTO v_current_stock
    FROM public.products
    WHERE id = p_product_id
      AND is_active = TRUE
    FOR UPDATE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Product % does not exist or is inactive.',
            p_product_id;
    END IF;

    IF v_current_stock < p_quantity THEN
        RAISE EXCEPTION
            'Insufficient stock. Available: %, requested: %.',
            v_current_stock,
            p_quantity;
    END IF;

    UPDATE public.products
    SET stock = stock - p_quantity,
        updated_at = CURRENT_TIMESTAMP
    WHERE id = p_product_id;

    INSERT INTO public.inventory_movements
    (
        product_id,
        movement_type,
        quantity,
        notes
    )
    VALUES
    (
        p_product_id,
        'EXIT',
        p_quantity,
        p_notes
    );
END;
$$;


-- Query the current inventory
CREATE OR REPLACE FUNCTION public.fn_get_current_inventory()
RETURNS TABLE
(
    product_id BIGINT,
    code VARCHAR(50),
    name VARCHAR(150),
    stock INTEGER,
    unit_price NUMERIC(12, 2),
    inventory_value NUMERIC
)
LANGUAGE sql
STABLE
AS $$
    SELECT
        p.id,
        p.code,
        p.name,
        p.stock,
        p.unit_price,
        p.stock * p.unit_price
    FROM public.products AS p
    WHERE p.is_active = TRUE
    ORDER BY p.name;
$$;
