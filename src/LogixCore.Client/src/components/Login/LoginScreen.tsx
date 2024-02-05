import { MouseEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import { JwtPayload, jwtDecode } from 'jwt-decode';
import { useAppDispatch, useAppSelector } from '../../hooks.ts';
import styled from 'styled-components';
import { logout, Jwt } from './';

interface ExtendedJwtPayload extends JwtPayload {
    Id?: string;
    name?: string;
}

const LoggedInContainer = styled.div`
    background: #ffffff;
    padding: 2rem;
    display: flex;
    flex-direction: column;
    gap: 10px;
`

const LogoutButton = styled.button`
    background-color: gray;
    color: white;
    border: none;
    padding: 15px;
    font-size: 1rem;
    font-weight: bold;
    border-radius: 5px;
    cursor: pointer;
`

const LoginScreen = () => {
    const isAuthenticated = useAppSelector(state => state.isAuthenticated);
    const dispatch = useAppDispatch();
    const navigate = useNavigate();
    const token = localStorage.getItem(Jwt.Token);
    const decoded = token ? jwtDecode<ExtendedJwtPayload>(token) : null;

    const onLogOutClick = async (e: MouseEvent<HTMLButtonElement>) => {
        e.preventDefault();
        dispatch(logout(false));
        navigate('/');
    }

    if (isAuthenticated) {
        return (
            <>
                <LoggedInContainer>
                <div>
                        You are logged in!
                        <p>Username: {decoded?.name?.toString()}</p>
                        <p>UserId: {decoded?.Id?.toString()}</p>
                </div >
                    <LogoutButton onClick={onLogOutClick}>Logout</LogoutButton>
                </LoggedInContainer>
            </>
        );
    }

    return (<div>Unauthorized</div>);
}

export { LoginScreen }