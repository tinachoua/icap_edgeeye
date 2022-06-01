const webpack = require('webpack')
const path = require('path')
const HtmlWebpackPlugin = require('html-webpack-plugin');
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

console.log(`Running ${version.VERSION_NUMBER} version`);

const devConfig = {
  devtool: 'eval',
  entry: './src/js/entry.js',
  output: {
    path: path.join(__dirname, 'dist'),
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
        IS_OFFLINE: true,
        VERSION_NUMBER: version.VERSION_NUMBER,
        WEBSOCKET_HOST: "172.16.92.116:8165"
      }),
    })
  ],
  devServer: {
    contentBase: path.join(__dirname, "dist"),
    compress: true,
    port: 9000,
    host: '0.0.0.0',
    proxy: {
      "/DashboardAPI": {
        target: "http://172.16.93.100",
        secure: false,
        changeOrigin: true
      },
      "/SettingAPI": {
        target: "http://172.16.93.100",
        secure: false,
        changeOrigin: true
      },
      "/DeviceInfoAPI": {
        target: "http://172.16.93.100",
        secure: false,
        changeOrigin: true
      },
      "/AuthenticationAPI": {
        target: "http://172.16.93.100",
        secure: false,
        changeOrigin: true
      },
      "/DeviceAPI": {
        target: "http://172.16.93.100",
        secure: false,
        changeOrigin: true
      },
      "/BranchAPI": {
        target: "http://172.16.93.100",
        secure: false,
        changeOrigin: true
      },
      "/StatusAPI": {
        target: "http://172.16.93.100",
        secure: false,
        changeOrigin: true
      },
      "/EventAPI": {
        target: "http://172.16.93.100",
        secure: false,
        changeOrigin: true
      },
      "/EmployeeAPI": {
        target: "http://172.16.93.100",
        secure: false,
        changeOrigin: true
      },
      "/WidgetAPI": {
        target: "http://172.16.93.100",
        secure: false,
        changeOrigin: true
      },
      "/Screenshot": {
        target: "http://172.16.93.100",
        secure: false,
        changeOrigin: true
      },
      "/Screenshot": {
        target: "http://172.16.93.100",
        secure: false,
        changeOrigin: true
      },
      "/innoAGE": {
        target: "http://172.16.92.116:8165",
        secure: false,
        changeOrigin: true
      }
    }
  }
}

module.exports = devConfig