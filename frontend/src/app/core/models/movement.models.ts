export interface InventoryMovement {
    id: number;
    productId: number;
    productCode: string;
    productName: string;
    movementType: string;
    quantity: number;
    notes: string | null;
    createdAt: string;
}

export interface RegisterMovement {
    productId: number;
    quantity: number;
    notes: string | null;
}
