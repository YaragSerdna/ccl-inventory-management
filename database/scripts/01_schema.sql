
-- =========================================================
-- CCL Inventory Management
-- File: 01_schema.sql
-- Description: Initial inventory database schema
-- Database: ccl_inventory_db
-- =========================================================

CREATE TABLE IF NOT EXISTS public.products
(
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    code VARCHAR(50) NOT NULL,
    name VARCHAR(150) NOT NULL,
    description VARCHAR(500),
    stock INTEGER NOT NULL DEFAULT 0,
    unit_price NUMERIC(12, 2) NOT NULL DEFAULT 0,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT uq_products_code UNIQUE (code),
    CONSTRAINT ck_products_stock_nonnegative CHECK (stock >= 0),
    CONSTRAINT ck_products_unit_price_nonnegative CHECK (unit_price >= 0)
);

CREATE TABLE IF NOT EXISTS public.inventory_movements
(
    id BIGINT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    product_id BIGINT NOT NULL,
    movement_type VARCHAR(10) NOT NULL,
    quantity INTEGER NOT NULL,
    notes VARCHAR(500),
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_inventory_movements_product
        FOREIGN KEY (product_id)
        REFERENCES public.products(id),

    CONSTRAINT ck_inventory_movements_type
        CHECK (movement_type IN ('ENTRY', 'EXIT')),

    CONSTRAINT ck_inventory_movements_quantity
        CHECK (quantity > 0)
);

CREATE INDEX IF NOT EXISTS ix_inventory_movements_product_date
    ON public.inventory_movements (product_id, created_at DESC);

-- =========================================================
-- Initial schema completed
-- =========================================================
