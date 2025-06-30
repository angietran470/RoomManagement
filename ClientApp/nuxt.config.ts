// https://nuxt.com/docs/api/configuration/nuxt-config
import { readFileSync } from 'fs'
import { certFilePath, keyFilePath } from './aspnetcore-https.cjs'
import tailwindcss from '@tailwindcss/vite'

export default defineNuxtConfig({
  compatibilityDate: '2025-05-15',
  devtools: { enabled: true },
  css: ['~/assets/css/main.css'],
  ssr: false,
  devServer: {
    https: {
      key: readFileSync(keyFilePath).toString(),
      cert: readFileSync(certFilePath).toString()
    },
    port: 5002
  },
  icon: {
    mode: 'css',
    cssLayer: 'base'
  },
  vite: {
    plugins: [
      tailwindcss()
    ],
    server: {
      warmup: {
        clientFiles: [
          './components/**/*.vue',
          './pages/**/*.vue'
        ]
      },
      proxy: {
        '/api': {
          target: 'https://localhost:5001/',
          changeOrigin: true,
          secure: false
        },
        '/hub':{
          target: 'https://localhost:5001/',
          changeOrigin: true,
          secure: false
        }
      },
    }
  },
  modules: ['@nuxt/ui', '@nuxt/icon', '@nuxt/fonts', '@nuxt/image', '@formkit/nuxt']
})