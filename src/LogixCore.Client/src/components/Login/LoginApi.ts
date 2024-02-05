import axios, { AxiosResponse } from 'axios';

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
                if(error.response.status === 401)
                    throw new Error("Bad credentials. Please try again.");

                    throw new Error("Error occurred during login.")
            }) as AxiosResponse<LoginResponse>;

        return response.data;
    },
}