/*eslint-disable*/
const merge = require('webpack-merge');
const common = require('./webpack.common.js');
const TerserJSPlugin = require("terser-webpack-plugin");
const webpack = require('webpack');
const path = require('path');

module.exports = merge(common, {
    output: {
        path: path.join(__dirname, 'dist'),
        filename: 'index.js',
        libraryTarget: 'commonjs2'
    },
    optimization: {
        minimizer: [
            new TerserJSPlugin({
                terserOptions: {
                    extractComments: 'all',
                    compress: {
                        warnings: false,
                        drop_console: true,
                        drop_debugger: true
                    }
                }
            }),
        ]
    },
    plugins: [
        new webpack.HashedModuleIdsPlugin(),
    ],
    mode: 'production',
});