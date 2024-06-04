/** @type {import('tailwindcss').Config} */
module.exports = {
    content: ["./**/*.{razor,html,cshtml}"],
    darkMode: 'selector',
    theme: {
        extend: {},
    },
    fontFamily: {
        sans: ['Roboto', 'sans-serif'],
    },
    plugins: [
        require('daisyui')
    ],
  daisyui: {
      themes: ["nord", "business"]
  }
}

