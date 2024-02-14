import { MouseEvent } from 'react';
import styled from 'styled-components';
import { TestingApi } from './TestingApi';

const Button = styled.button`
    background-color: orange;
    color: white;
    border: none;
    padding: 10px;
    font-size: 1rem;
    font-weight: bold;
    border-radius: 5px;
    cursor: pointer;
`

const TestingButton = () => {
    const onClick = async (e: MouseEvent<HTMLButtonElement>) => {
        e.preventDefault();

        try {
            const response = await TestingApi.testing();

            window.alert(response);
        } catch (error) {
            if (error instanceof Error)
                window.alert(error.message);
        }
    }

    return (<Button onClick={onClick}>Test Api</Button>)
}
const PostTestingButton = () => {
    const onClick = async (e: MouseEvent<HTMLButtonElement>) => {
        e.preventDefault();

        try {
            const response = await TestingApi.testingPost("test");
            window.alert(response);
        } catch (error) {
            if (error instanceof Error)
                window.alert(error.message);
        }
    }

    return (<Button onClick={onClick}>Test POST Api</Button>)
}
export { TestingButton, PostTestingButton }