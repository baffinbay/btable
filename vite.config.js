import { defineConfig } from 'vite'

export default defineConfig({
  build: {
    rollupOptions: {
      input: {
        bundle: 'fable/Library.js',
        styles: 'index.css'
      }
    }
  }
})
