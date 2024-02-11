import { configureStore } from '@reduxjs/toolkit';
import { persistReducer, persistStore } from 'redux-persist';
import { loginReducer } from './components/Login';
import storage from 'redux-persist/lib/storage';

const loginPersistConfig = {
    key: 'root',
    storage
}

const persistedReducer = persistReducer(loginPersistConfig, loginReducer);

export const store = configureStore({
    reducer: {
        isAuthenticated: persistedReducer
    },
    middleware: (getDefaultMiddleware) => getDefaultMiddleware({ serializableCheck: false })
})

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch
export const persistor = persistStore(store);