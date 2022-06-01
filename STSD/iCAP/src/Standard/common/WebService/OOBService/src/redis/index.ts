import connect from './connect';

const client = connect();

export async function getValue(db: number, key:string){
  client.select(db);
  return await client.get(key);
}