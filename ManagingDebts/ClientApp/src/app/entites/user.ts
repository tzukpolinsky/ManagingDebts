export interface User {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
    customerId?: number;
    password?: string;
    isActive: boolean;
    isSuperAdmin: boolean;
}
