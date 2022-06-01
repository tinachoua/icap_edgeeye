/*eslint-disable*/
const { CleanWebpackPlugin } = require('clean-webpack-plugin');
const nodeExternals = require('webpack-node-externals');
const path = require('path');

module.exports = {
    target: 'node',
    externals: [nodeExternals({
        // this WILL include `jquery` and `webpack/hot/dev-server` in the bundle, as well as `lodash/*`
        whitelist: ['']
    })],
    entry: './src/index.ts',
    plugins: [
        new CleanWebpackPlugin(),
    ],
    module: {
        rules: [
            {
                test: /\.tsx?$/,
                use: [
                    {
                        loader: 'ts-loader',
                        options: {
                            // 加快编译速度
                            transpileOnly: true,
                            // 指定特定的ts编译配置，为了区分脚本的ts配置
                            configFile: path.resolve(__dirname, './tsconfig.json')
                        }
                    }
                ],
                exclude: /node_modules/
            }
        ],
    },
    resolve: {
        extensions: [".ts", ".js", ".json"],
    }
};