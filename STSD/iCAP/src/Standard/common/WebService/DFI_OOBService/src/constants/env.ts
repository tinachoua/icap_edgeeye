import dotenv from 'dotenv';
dotenv.config();

export const MQTT_HOST = process.env.MQTT_HOST;
export const MQTT_PORT = process.env.MQTT_PORT;
export const MQTT_PASSWORD = process.env.MQTT_PASSWORD;
export const MQTT_USERNAME = process.env.MQTT_USERNAME;

export const SQL_HOST = process.env.SQL_HOST
export const SQL_PORT = process.env.SQL_PORT
export const SQL_DB = process.env.SQL_DB

export const SQL_USER = process.env.SQL_USER
export const SQL_PASSWORD = process.env.SQL_PASSWORD

export const REDIS_HOST = process.env.REDIS_HOST
export const REDIS_PORT = process.env.REDIS_PORT

export const INNOAGE_SERVICE_HOST = process.env.INNOAGE_SERVICE_HOST

export const DEBUG = process.env.DEBUG; //tmp

export const AUTHENTICATION_HOST = process.env.AUTHENTICATION_HOST;

export const SERVICE_PORT = process.env.SERVICE_PORT || 8165;