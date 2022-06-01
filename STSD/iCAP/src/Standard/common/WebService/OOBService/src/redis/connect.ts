import asyncRedis from "async-redis";
import { REDIS_HOST } from '../config/env';

export default function () {
  return asyncRedis.createClient({ host: REDIS_HOST });
}
