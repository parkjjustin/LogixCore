import axios, { AxiosError } from 'axios';

export interface UserLogin {
    username: string;
    password: string;
}

export interface LoginResponse {
    isAuthenticated: boolean;
/*    token: string;*/
    //refreshToken: string;
}

export const LoginApi = {
    async login(userLogin: UserLogin): Promise<LoginResponse> {
        try {
            const response = await axios.post<LoginResponse>('api/login', userLogin);
            return response.data;
        }
        catch (error) {
            if (error instanceof AxiosError)
                if (error?.response?.status === 401)
                    throw new Error("Bad credentials. Please try again.");

            throw new Error("Error occurred during login.")
        }
    },

    async logout(): Promise<void> {
        try {
            await axios.post<LoginResponse>('api/logout');
        }
        catch (error) {
            throw new Error("Error occurred during login.")
        }
    }
    //async getAntiforgeryToken(): Promise<string> {
    //    try {
    //        const response = await axios.get<string>('api/antiforgery');
    //        return response.data;
    //    }
    //    catch (error) {
    //        throw new Error("Error occurred getting antiforgery token.")
    //    }
    //}
}