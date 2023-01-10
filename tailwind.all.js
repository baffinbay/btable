const common = require('./tailwind.common')

module.exports = {
  content: common.content,
  theme: common.theme,
  variants: common.variants,
  plugins: common.plugins,
  safelist: [{ pattern: /.*/ }],
}
