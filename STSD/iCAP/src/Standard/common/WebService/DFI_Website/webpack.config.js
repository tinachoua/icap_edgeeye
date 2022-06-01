var webpack = require('webpack')
var path = require('path')
var HtmlWebpackPlugin = require('html-webpack-plugin')

module.exports = {
  devtool: 'eval',
  entry: './src/js/entry.js',
  output: {
    path: path.join(__dirname, 'dist'),
    filename: 'bundle.js'
  },
  module: {
    loaders: [
      {
        test: /\.css$/,
        loader: 'style-loader!css-loader'
      }, {
        test: /\.html$/,
        loader: 'raw-loader'
      }, {
        test: /\.(jpe?g|png|woff|woff2|eot|ttf|svg)(\?[a-z0-9=.]+)?$/,
        loader: 'url-loader?limit=123000'
      }, {
        test: /\.js$/,
        exclude: /(node_modules|bower_components)/,
        loader: 'babel-loader',
        query: {
          presets: ['es2015']
        }
      }
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
    new webpack.EnvironmentPlugin({
      WEBSOCKET_HOST: '192.168.3.200',
      IS_OFFLINE: true,
      DEBUG: true
    }),
    new webpack.DefinePlugin({
    })
  ],
  devServer: {
    contentBase: path.join(__dirname, "dist"),
    compress: true,
    port: 8000,
    proxy: {
      "/DashboardAPI": {
        target: "http://192.168.3.200",
        secure: false,
        changeOrigin: true
      },
      "/SettingAPI": {
        target: "http://192.168.3.200",
        secure: false,
        changeOrigin: true
      },
      "/DeviceInfoAPI": {
        target: "http://192.168.3.200",
        secure: false,
        changeOrigin: true
      },
      "/AuthenticationAPI": {
        target: "http://192.168.3.200",
        secure: false,
        changeOrigin: true
      },
      "/DeviceAPI": {
        target: "http://192.168.3.200",
        secure: false,
        changeOrigin: true
      },
      "/BranchAPI": {
        target: "http://192.168.3.200",
        secure: false,
        changeOrigin: true
      },
      "/StatusAPI": {
        target: "http://192.168.3.200",
        secure: false,
        changeOrigin: true
      },
      "/EventAPI": {
        target: "http://192.168.3.200",
        secure: false,
        changeOrigin: true
      },
      "/EmployeeAPI": {
        target: "http://192.168.3.200",
        secure: false,
        changeOrigin: true
      },
      "/WidgetAPI": {
        target: "http://192.168.3.200",
        secure: false,
        changeOrigin: true
      },
      "/Screenshot": {
        target: "http://192.168.3.200",
        secure: false,
        changeOrigin: true
      },
      "/Screenshot": {
        target: "http://192.168.3.200",
        secure: false,
        changeOrigin: true
      },
      "/InnoAGE":{
        target: "http://192.168.3.200",
        secure: false,
        changeOrigin: true
      },
      "/devices":{
        target: "http://172.16.92.114:8161",
        secure: false,
        changeOrigin: true
      },
    //   '/ws': {
    //     target: 'ws://172.16.92.123',
    //     ws: true
    //  },
    }
  }
}