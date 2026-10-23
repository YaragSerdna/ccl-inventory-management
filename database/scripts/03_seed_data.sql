
-- =========================================================
-- CCL Inventory Management
-- File: 03_seed_data.sql
-- Description: Sample products for development and testing
-- =========================================================

INSERT INTO public.products
    (code, name, description, stock, unit_price)
VALUES
    ('TEC-001', 'Teclado USB', 'Teclado USB para computador', 0, 45000.00),
    ('TEC-002', 'Mouse USB', 'Mouse óptico USB', 0, 25000.00),
    ('TEC-003', 'Monitor 24 pulgadas', 'Monitor para estación de trabajo', 0, 650000.00),
    ('TEC-004', 'Cable HDMI', 'Cable HDMI de 2 metros', 0, 18000.00),
    ('TEC-005', 'Base para portátil', 'Base ajustable para portátil', 0, 85000.00)
ON CONFLICT (code) DO NOTHING;
