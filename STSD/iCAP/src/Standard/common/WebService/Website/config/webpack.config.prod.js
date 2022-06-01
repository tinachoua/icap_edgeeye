var webpack = require('webpack');
var path = require('path');
var HtmlWebpackPlugin = require('html-webpack-plugin');
const UglifyJsPlugin = require('uglifyjs-webpack-plugin')
const {
  CSSLoader,
  HtmlLoader,
  UrlLoader,
  JSLoader
} = require('../helpers/loders');
const {
  ExtractTextPlugin,
  GlobalVariablesPlugin
} = require('../helpers/plugins');
const version = require('../config/version.json');

console.log(`Pack ${version.VERSION_NUMBER} version`);

module.exports = {
  entry: './src/js/entry.js',
  output: {
    path: path.join(process.cwd(), 'dist'),
    filename: 'bundle.js'
  },
  module: {
    loaders: [
      CSSLoader, HtmlLoader, UrlLoader, JSLoader
    ]
  },
  plugins: [
    new webpack.BannerPlugin('Add innodisk license here'),
    new webpack.optimize.OccurrenceOrderPlugin(),
    new HtmlWebpackPlugin({
      template: './src/index.html'
    }),
    new webpack.ProvidePlugin({
      "$": "jquery",
      "jQuery": "jquery",
      "window.jQuery": "jquery"
    }),
    GlobalVariablesPlugin,
    ExtractTextPlugin,
    new webpack.DefinePlugin({
      "process.env": JSON.stringify({
        IS_OFFLINE: false,
        VERSION_NUMBER: version.VERSION_NUMBER
      })
    }),
    new UglifyJsPlugin()
  ],
}