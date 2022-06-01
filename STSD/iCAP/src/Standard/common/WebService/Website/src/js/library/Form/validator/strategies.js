const Strategies = {
  minValue: function(value, min, errorMsg){
    if (value < min) return errorMsg
  },
  isNonEmpty: function(value, errorMsg){
    if(value === ''){
      return errorMsg
    }
  }
}

export default Strategies;