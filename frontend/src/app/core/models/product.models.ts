export interface Product {
    id: number;
    code: string;
    name: string;
    description: string | null;
    stock: number;
    unitPrice: number;
    isActive: boolean;
    createdAt: string;
    updatedAt: string;
}

export interface CreateProduct {
    code: string;
    name: string;
    description: string | null;
    unitPrice: number;
}

export interface UpdateProduct {
    code: string;
    name: string;
    description: string | null;
    unitPrice: number;
}
