import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import million from "million/compiler";
import mkcert from "vite-plugin-mkcert";

// https://vitejs.dev/config/
export default defineConfig({
    server: {
        proxy: {
            "/api": {
                target: "https://localhost:7224",
                changeOrigin: true,
                secure: false,
            },
            "/signin-oidc": {
                target: "https://localhost:7224",
                secure: false,
            },
            "/signout-callback-oidc": {
                target: "https://localhost:7224",
                secure: false,
            },
            "/api/register": {
                target: "https://localhost:7224",
                secure: false
            }
        }
    },
    plugins: [million.vite({ auto: true }), react(), mkcert()]
})
