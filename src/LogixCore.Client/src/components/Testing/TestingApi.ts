import axios, { AxiosError } from 'axios';

const TestingApi = {
    async testing(): Promise<boolean | undefined> {
        try {
            const response = await axios.get<boolean>('api/testing');
            return response.data;
        } catch (error) {
            if (error instanceof AxiosError)
                throw new Error(error?.response?.statusText);
        }
    },
    async testingPost(test: string): Promise<string | undefined> {
        try {
            const response = await axios.post<string>('api/testing', { test: test });
            return response.data;
        } catch (error) {
            if (error instanceof AxiosError)
                throw new Error(error?.response?.statusText);
        }
    },
}

export { TestingApi }