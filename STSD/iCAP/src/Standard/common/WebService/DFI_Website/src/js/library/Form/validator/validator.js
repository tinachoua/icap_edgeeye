import Strategies from './strategies';

function Validator (){
  this.cache = [];
  this.strategies = Strategies;
}

Validator.prototype.add = function(dom, rules){
  const self = this;

  for(let i = 0, rule; rule=rules[i++];){
    (function(rule){
      let strategyAry = rule.strategy.split(':');
      const errorMsg = rule.errorMsg;

      self.cache.push(function(){
        const strategy = strategyAry.shift();
        strategyAry.unshift( dom.value );
        strategyAry.push( errorMsg );
        return self.strategies[strategy].apply(dom, strategyAry);
      });
    })(rule)
  }
}

Validator.prototype.start = function(){
  for(let i=0, ValidatorFunc; ValidatorFunc = this.cache[i++];){
    const errorMsg = ValidatorFunc();

    if(errorMsg) {
      return errorMsg;
    }
  }
}

function ValidataFunc (fields) {
  let validator = new Validator();

  for(let i = 0, field; field = fields[i++];){
    const {dom, rules} = field;
    validator.add(dom, rules);
  }

  const errorMsg = validator.start();

  return errorMsg;
}

export default Validator;
export {ValidataFunc};