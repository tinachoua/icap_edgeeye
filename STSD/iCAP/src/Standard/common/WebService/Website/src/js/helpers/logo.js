const logoCache = {}

export default function getLogo(path){
  if(!path) return;
  return logoCache[path] || (logoCache[path] = require(`../../assets/logo/${path}`))
}