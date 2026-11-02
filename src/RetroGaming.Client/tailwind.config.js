/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        retro: {
          dark: '#121212',
          card: '#1e1e24',
          accent: '#00e5ff',
          neon: '#ff007f',
          purple: '#9d4edd',
          gold: '#ffd166',
        }
      }
    },
  },
  plugins: [],
}
