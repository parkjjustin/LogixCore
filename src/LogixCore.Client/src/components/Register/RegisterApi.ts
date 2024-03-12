import axios from 'axios';

export interface UserRegister {
    username: string;
    email: string;
    password: string;
    confirmPassword: string;
}

export interface UserRegisterResponse {
    isAuthenticated: boolean;
}

export const RegisterApi = {
    async register(userRegister: UserRegister): Promise<UserRegisterResponse> {
        try {
            const response = await axios.post<UserRegisterResponse>('api/register', userRegister);
            return response.data;
        }
        catch (error) {
           
            throw new Error("Could not register user.")
        }
    }
}