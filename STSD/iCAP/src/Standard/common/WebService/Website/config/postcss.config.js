const GLOBAL_STYLE = require('../helpers/globalVariables');

module.exports = {
  plugins: {
    'postcss-import': {},
    'postcss-cssnext': {
      browsers: ['last 2 versions', '> 5%'],
      features: {
        customProperties: { variables: GLOBAL_STYLE }
      }
    },
  },
};