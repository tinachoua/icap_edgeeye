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
    new webpack.DefinePlugin({
      "process.env.IS_OFFLINE": true
    })
  ],
  devServer: {
    contentBase: path.join(__dirname, "dist"),
    compress: true,
    port: 9000,
    proxy: {
      "/DashboardAPI": {
        target: "http://172.16.93.106",
        secure: false,
        changeOrigin: true
      },
      "/SettingAPI": {
        target: "http://172.16.93.106",
        secure: false,
        changeOrigin: true
      },
      "/DeviceInfoAPI": {
        target: "http://172.16.93.106",
        secure: false,
        changeOrigin: true
      },
      "/AuthenticationAPI": {
        target: "http://172.16.93.106",
        secure: false,
        changeOrigin: true
      },
      "/DeviceAPI": {
        target: "http://172.16.93.106",
        secure: false,
        changeOrigin: true
      },
      "/BranchAPI": {
        target: "http://172.16.93.106",
        secure: false,
        changeOrigin: true
      },
      "/StatusAPI": {
        target: "http://172.16.93.106",
        secure: false,
        changeOrigin: true
      },
      "/EventAPI": {
        target: "http://172.16.93.106",
        secure: false,
        changeOrigin: true
      },
      "/EmployeeAPI": {
        target: "http://172.16.93.106",
        secure: false,
        changeOrigin: true
      },
      "/WidgetAPI": {
        target: "http://172.16.93.106",
        secure: false,
        changeOrigin: true
      },
      "/Screenshot": {
        target: "http://172.16.93.106",
        secure: false,
        changeOrigin: true
      },
      "/Screenshot": {
        target: "http://172.16.93.106",
        secure: false,
        changeOrigin: true
      },
      "/innoAGE": {
        target: "http://172.16.92.116",
        secure: false,
        changeOrigin: true
      }
    }
  }
}