import { ChangeEvent, useState } from 'react';
import { SubmitHandler, useForm } from 'react-hook-form';
import styled from 'styled-components';

import { RegisterApi, UserRegister } from './'

const Container = styled.div`
    max-width: 400px;
    display: flex;
    flex-direction: column;
    align-items: center;
`
const RegisterContainer = styled.div`
    border-radius: 1rem;
    background: #ffffff;
    padding: 2rem;
    box-sizing: border-box;
    display: flex;
    flex-direction: column;
    align-items: center;
`

const Form = styled.form`
    display: flex;
    flex-direction: column;
    width: 100%;
    gap: 1rem;
`

const InputGroup = styled.div`
    position: relative;
`

const Input = styled.input`
    position: relative;
    border: 1px solid #ccc;
    border-radius: 0.5rem;
    height: 45px;
    padding: 0 10px;
    outline: none;
    width: 100%;
    box-sizing: border-box;

    &:focus {
        border: 1.5px solid orange;
    }
`

const Label = styled.span`
    position: absolute;
    top: 50%;
    left: 5px;
    transform: translateY(-50%);
    font-size: 1rem;
    padding: 0 5px;
    color: #ccc;
    transition: 0.25s;
    pointer-events: none;

    ${Input}:focus ~ & {
        top: 0;
        left: 7px;
        font-size: 0.75rem;
        font-weight: bold;
        background-color: white;
        color: orange;
    }

    ${Input}:valid ~ & {
        top: 0;
        left: 7px;
        font-size: 0.75rem;
        font-weight: bold;
        background-color: white;
        color: orange;
    }
`

const Header = styled.h1`
    font-size: 1.5rem;
`

const SubmitButton = styled.button`
    background-color: orange;
    color: white;
    border: none;
    padding: 15px;
    font-size: 1rem;
    font-weight: bold;
    border-radius: 5px;
    cursor: pointer;
`

const ErrorMessage = styled.span`
    font-size: .75rem;
    color: red;
    padding-left: 0.25rem;
    margin-top: -10px;
`

const ErrorBox = styled.div`
    font-size: .75rem;
    color: red;
    background: #f5c6cb;
    border: 2px solid #721c24;
    border-radius: 5px;
    margin: 10px;
    padding: 10px;
    width: 100%;
    box-sizing: border-box;
    text-align: center;
`
/*const emailRegex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$/;*/
const emailRegex = /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/;

const RegisterModule = () => {
    const {
        register,
        handleSubmit,
        formState: { errors },
        setValue,
        getValues,
        reset
    } = useForm<UserRegister>();
    const [unmaskedPassword, setUnmaskedPassword] = useState('');
    const [errorMessage, setErrorMessage] = useState('');

    const onPasswordChange = (e: ChangeEvent<HTMLInputElement>) => {
        const passwordValue = e.currentTarget.value;

        const maskedValue = passwordValue.substring(0, passwordValue.length - 1)
            .replace(/./g, '●') +
            passwordValue.substring(
                passwordValue.length - 1,
                passwordValue.length
            )
        setValue("password", maskedValue);
        setUnmaskedPassword(unmaskedPassword + passwordValue[passwordValue.length - 1] || '');
    }

    const onSubmit: SubmitHandler<UserRegister> = async (userRegister: UserRegister) => {
        setValue("password", unmaskedPassword);
        try {
            const response = await RegisterApi.register(getValues());
            console.log(response)
            // navigate('/test');
        } catch (error) {
            if (error instanceof Error) {
                setErrorMessage(error.message);
            }
        }
        finally {
            reset();
        }
    }

    return (
        <Container title='Logix'>
            <RegisterContainer>
                <Header>Register</Header>
                {errorMessage && <ErrorBox>{errorMessage}</ErrorBox>}
                <Form noValidate onSubmit={handleSubmit(onSubmit)}>
                    <InputGroup>
                        <Input
                            {...register('username', {
                                required: 'Username is required',
                                minLength: {
                                    value: 5,
                                    message: 'Username must have a minimum of 5 characters.'
                                },
                                maxLength: {
                                    value: 20,
                                    message: 'Username must have a maximum of 20 characters.'
                                },
                            })}
                            type='text'
                            autoComplete={'off'}
                            required
                        />
                        <Label>Username</Label>
                    </InputGroup>
                    {errors?.username && <ErrorMessage>{errors.username.message}</ErrorMessage>}
                    <InputGroup>
                        <Input
                            {...register('email', {
                                required: 'Email is required',
                                pattern: {
                                    value: emailRegex,
                                    message: "Please enter a valid email"
                                }
                            })}
                            type='text'
                            autoComplete={'off'}
                            required
                        />
                        <Label>Email</Label>
                    </InputGroup>
                    {errors?.email && <ErrorMessage>{errors.email.message}</ErrorMessage>}
                    <InputGroup>
                        <Input
                            {...register('password', {
                                required: 'Password is required',
                                onChange: onPasswordChange
                            })}
                            type='text'
                            autoComplete={'off'}
                            required />
                        <Label>Password</Label>
                    </InputGroup>
                    {errors?.password && <ErrorMessage>{errors.password.message}</ErrorMessage>}
                    <SubmitButton type='submit'>Register</SubmitButton>
                </Form>
            </RegisterContainer>
        </Container>
    )
}

export { RegisterModule }
