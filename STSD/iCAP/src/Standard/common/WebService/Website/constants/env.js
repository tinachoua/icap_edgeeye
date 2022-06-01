require('dotenv').config()

module.exports.CONFIG_PATH = process.env.CONFIG_PATH ? process.env.CONFIG_PATH: 'current_config.json';