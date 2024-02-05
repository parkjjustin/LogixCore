import axios from 'axios';

export interface UserLogin {
    username: string;
    password: string;
}

export interface LoginResponse {
    isAuthenticated: boolean;
    jwtToken: string;
    refreshToken: string;
}

export const LoginApi = {
    async login(userLogin: UserLogin): Promise<LoginResponse> {
        const response = await axios.post<LoginResponse>('api/login', userLogin)
            .catch(error => {
                throw new Error("Bad credentials. Please try again.");
            });

        return response.data;
    },
}