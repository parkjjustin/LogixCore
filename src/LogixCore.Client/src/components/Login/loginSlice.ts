import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import axios from 'axios';
import { LoginResponse } from '.';

export enum Jwt {
    Token = 'jwt-token'
}

export interface LoginState {
    isAuthenticated: boolean;
    antiforgeryToken: string | null;
}

const initialState: LoginState = {
    isAuthenticated: false,
    antiforgeryToken: null
}

export const loginSlice = createSlice({
    name: 'login',
    initialState,
    reducers: {
        login: (state, action: PayloadAction<LoginResponse>) => {
            //const token = localStorage.getItem(Jwt.Token);
            //if (token) {
            state.isAuthenticated = action.payload.isAuthenticated;
/*            axios.defaults.headers.common['X-XSRF-TOKEN'] = action.payload.token;*/
        },
        logout: (state, action: PayloadAction<boolean>) => {
            /*            localStorage.removeItem(Jwt.Token);*/
            state.isAuthenticated = action.payload;
            delete axios.defaults.headers.common['X-XSRF-TOKEN'];
            /*            delete axios.defaults.headers.common['Authorization'];*/
        },
        setAntiforgeryToken: (state, action: PayloadAction<string>) => {
            state.antiforgeryToken = action.payload;
            axios.defaults.headers.common['X-XSRF-TOKEN'] = action.payload;
        }
    },
})

export const { login, logout, setAntiforgeryToken } = loginSlice.actions

const loginReducer = loginSlice.reducer;

export { loginReducer };
