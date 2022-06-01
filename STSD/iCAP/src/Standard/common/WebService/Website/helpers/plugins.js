const _ExtractTextPlugin = require('extract-text-webpack-plugin');
const jVar = require("json-variables");
const webpack = require('webpack');
const ExtractTextPlugin = new _ExtractTextPlugin('[name].bundle.css');

const GLOBAL_VARIABLES = require('./globalVariables');

const GlobalVariablesPlugin = new webpack.DefinePlugin({
  "GLOBAL_VARIABLES":  JSON.stringify(jVar(GLOBAL_VARIABLES,   {
    heads: "${",
    tails: "}",
  }))
});
 
module.exports = {
  ExtractTextPlugin,
  GlobalVariablesPlugin
};