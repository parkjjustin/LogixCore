import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import mkcert from 'vite-plugin-mkcert';
import million from "million/compiler";

// https://vitejs.dev/config/
export default defineConfig({
    server: {
        https: true,
        proxy: {
            "/api": {
                target: "https://localhost:7224",
                changeOrigin: true,
                secure: false,
            }
        }
    },
    plugins: [million.vite({ auto: true }), react(), mkcert()],
})
