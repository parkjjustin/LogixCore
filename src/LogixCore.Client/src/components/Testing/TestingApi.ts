import axios from 'axios';

const TestingApi = {
    async testing(): Promise<boolean> {
        const response = await axios.get<boolean>('api/testing')
            .catch(error => {
                throw new Error(error.response.statusText);
            });

        return response.data;
    },
}

export { TestingApi }