import { defineConfig } from 'vite'
import { resolve } from 'path'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': resolve(__dirname, 'src'),
    },
  },
  css: {
    preprocessorOptions: {
      scss: {
        silenceDeprecations: ['import', 'global-builtin', 'color-functions', 'if-function'],
      },
    },
  },
  server: {
    port: 5173,
    proxy: {
      '/api': 'http://localhost:5280',
      '/uploads': 'http://localhost:5280',
    },
  },
  test: {
    environment: 'happy-dom',
    globals: true,
    setupFiles: './src/test-setup.ts',
  },
})
