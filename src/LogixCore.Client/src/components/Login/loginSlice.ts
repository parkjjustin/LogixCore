import axios from 'axios';
import { createSlice, PayloadAction } from '@reduxjs/toolkit';

export enum Jwt {
    Token = 'jwt-token'
}

export interface LoginState {
    isAuthenticated: boolean;
}

const initialState: LoginState = {
    isAuthenticated: false
}

export const loginSlice = createSlice({
    name: 'login',
    initialState,
    reducers: {
        login: (state, action: PayloadAction<boolean>) => {
            const token = localStorage.getItem(Jwt.Token);
            if (token) {
                state.isAuthenticated = action.payload;
                axios.defaults.headers.common = { 'Authorization': `Bearer ${token}` }
            }
        },
        logout: (state, action: PayloadAction<boolean>) => {
            localStorage.removeItem(Jwt.Token);
            state.isAuthenticated = action.payload;
            delete axios.defaults.headers.common['Authorization'];
        },
    },
})

export const { login, logout } = loginSlice.actions

const loginReducer = loginSlice.reducer;

export { loginReducer };