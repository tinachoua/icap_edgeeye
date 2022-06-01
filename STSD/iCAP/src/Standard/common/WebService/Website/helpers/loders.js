const path = require('path');
const plugins = require('./plugins')

const filePath = path.join(process.cwd(), 'config', 'postcss.config.js')

const CSSLoader = {
  test: /\.css$/,
  use: plugins.ExtractTextPlugin.extract({
    use: [
      {
        loader: 'css-loader',
        options: {importLoaders: 1},
      },
      {
        loader: 'postcss-loader',
        options: {
          config: {
            path: filePath
          }
        },
      },
    ],
  }),
};
const HtmlLoader = {
  test: /\.html$/,
  loader: 'raw-loader'
};
const UrlLoader = {
  test: /\.(jpe?g|png|woff|woff2|eot|ttf|svg)(\?[a-z0-9=.]+)?$/,
  loader: 'url-loader?limit=123000'
};
const JSLoader = {
  test: /\.js$/,
  exclude: /(node_modules|bower_components)/,
  loader: 'babel-loader',
  query: {
    presets: ['es2015']
  }
};

module.exports = {
  CSSLoader,
  HtmlLoader,
  UrlLoader,
  JSLoader
};
