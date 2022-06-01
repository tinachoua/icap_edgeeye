import Swal from 'sweetalert2'
import icon_fail from '../../assets/images/icons/icon_fail.png';
import icon_success from '../../assets/images/icons/icon_success.png';

function createSVG() {
  const xmlns = "http://www.w3.org/2000/svg";
  const boxWidth = 60;
  const boxHeight = 60;

  const svgElem = document.createElementNS(xmlns, "svg");
  svgElem.setAttributeNS(null, "viewBox", "0 0 " + boxWidth + " " + boxHeight);
  svgElem.setAttributeNS(null, "width", boxWidth);
  svgElem.setAttributeNS(null, "height", boxHeight);
  svgElem.setAttributeNS(null, "version", "1.1");

  const gradient = document.createElementNS(xmlns, "linearGradient");
  gradient.id = 'timelineGradient';
  gradient.setAttributeNS(null, "x1", "0");
  gradient.setAttributeNS(null, "x2", "0");
  gradient.setAttributeNS(null, "y1", "0");
  gradient.setAttributeNS(null, "y2", "1");

  const stop1 = document.createElementNS(xmlns, "stop");
  stop1.setAttributeNS(null, "offset", "5%");
  stop1.setAttributeNS(null, "stop-color", "#28CD41");
  const stop2 = document.createElementNS(xmlns, "stop");
  stop2.setAttributeNS(null, "offset", "90%");
  stop2.setAttributeNS(null, "stop-color", "#28CD41");

  gradient.appendChild(stop1);
  gradient.appendChild(stop2);

  const circle1 = document.createElementNS(xmlns, "circle");
  circle1.setAttributeNS(null, "r", '26');
  circle1.setAttributeNS(null, "cx", '30');
  circle1.setAttributeNS(null, "cy", '30');
  circle1.setAttributeNS(null, "fill", 'transparent');
  circle1.setAttributeNS(null, "stroke-dasharray", '163.38');
  circle1.setAttributeNS(null, "stroke-dashoffset", '0');
  circle1.setAttributeNS(null, "class", 'circle');
  //circle1.setAttributeNS(null, "stroke", '#D5D5D5 ');

  const circle2 = createControlBar()

  svgElem.appendChild(gradient);
  svgElem.appendChild(circle1);
  svgElem.appendChild(circle2);

  return {
    svgElem,
    controlBar: circle2,
    circle1
  }
}

function createControlBar() {
  const xmlns = "http://www.w3.org/2000/svg";

  const circle = document.createElementNS(xmlns, "circle");
  circle.setAttributeNS(null, "r", '26');
  circle.setAttributeNS(null, "cx", '30');
  circle.setAttributeNS(null, "cy", '30');
  circle.setAttributeNS(null, "fill", 'transparent');
  circle.setAttributeNS(null, "stroke-dasharray", '163.38');
  circle.setAttributeNS(null, "stroke-dashoffset", '0');
  circle.setAttributeNS(null, "class", 'circle bar');
  circle.setAttributeNS(null, "stroke", 'url(#timelineGradient)');

  return circle;
}

function Button({ onClick }) {
  this.element = null;
  this.onClick = onClick;
  this.duration = 1;
  this.last = 0;
}

Button.prototype.init = function () {
  const { icon } = this;

  const div = document.createElement('div');
  div.className = 'cont';
  div.setAttribute('data-pct', '');
  div.appendChild(icon);

  const { svgElem, controlBar } = createSVG();

  div.appendChild(svgElem);

  const btn = div;
  this.new = btn;

  return this.element = {
    btn,
    controlBar,
    icon,
    svgElem
  }
}

Button.prototype.disabled = function (devName) {
  const { element: { btn } } = this;

  btn.classList.remove('active');
  btn.onclick = () => { };
}

Button.prototype.enabled = function (element) {
  const { onClick } = this;
  const { btn, icon } = (element) ? element : this;

  btn.classList.add('active');
  btn.onclick = onClick;

  icon.style.display = '';

  btn.setAttribute('data-pct', '');
}

Button.prototype.show = function () {
  const { element: { btn } } = this;

  btn.style.dispaly = '';
}

Button.prototype.hide = function () {
  const { element: { btn } } = this;

  btn.style.dispaly = 'none';
}

Button.prototype.remove = function () {
  const { element: { btn } } = this;

  btn.remove();
}

Button.prototype.default = function () {
  const { element: { controlBar, btn } } = this;

  controlBar.classList.add('notransition');
  controlBar.style.strokeDashoffset = '';
  // setTimeout(()=>{
  //   controlBar.classList.remove('notransition');
  // }, 2000);
  // controlBar.classList.remove('notransition');
}

Button.prototype.processBarRotate = function (percent, { controlBar, btn, webkitTransitionEnd, devName }, valueAnimation = true) {
  const self = this;
  let { duration, timer } = self;
  const dataPct = btn.getAttribute('data-pct');

  const last = (dataPct) ? Number((dataPct.match(/\d+/)[0])) : 0;

  if (self[devName]) {
    ///cacel last 
    clearInterval(self[devName].timer);
    self[devName].timer = null;
  }

  const r = controlBar.getAttribute('r');
  const c = Math.PI * (r * 2);

  if (percent < 0) { percent = 0; }
  if (percent > 100) { percent = 100; }

  const pct = ((100 - percent) / 100) * c;

  controlBar.style.strokeDashoffset = pct;

  if (webkitTransitionEnd) {
    if (last === 0 && last === percent) {
      webkitTransitionEnd();
    } else {
      const end = function () {
        webkitTransitionEnd();
        controlBar.removeEventListener('webkitTransitionEnd', end)
      }
      controlBar.addEventListener('webkitTransitionEnd', end)
    }
  }

  function makeUpdate(count) {
    const interval = percent - last;
    const frequency = count;
    const time = duration * 1000 / count;

    function update(count) {
      if (count-- === 0) return;

      self[devName].timer = setTimeout(() => {
        const process = (last + (interval / frequency * (frequency - count)));

        btn.setAttribute('data-pct', process.toFixed() + '%');
        self[devName].timer = null;
        update(count);
      }, time)
    }

    update(count);
  }

  if (valueAnimation) {
    makeUpdate.call(this, 10);
  } else {
    btn.setAttribute('data-pct', percent.toFixed() + '%');
    // btn.setAttribute('data-pct', (percent === 0)? '': percent + '%');
  }
}

Button.prototype.getButton = function () {

  return this.element || (this.element = this.init());
}

Button.prototype.showMsg = function (msg) {
  return Swal.fire({
    //icon: 'success',
    imageUrl: icon_success,
    width: 360,
    // imageWidth: 50,
    // imageHeight: 50,
    title: 'Success',
    text: msg,
    //position: 'top'
  });
}

Button.prototype.showError = function (msg) {
  return Swal.fire({
    //icon: 'error',
    imageUrl: icon_fail,
    width: 360,
    title: 'Failure',
    text: msg || 'Something went wrong!',
    //position: 'top'
  })
}

Button.prototype.hideIcon = function (icon = this.element.icon) {
  icon.style.display = 'none'
}

Button.prototype.showIcon = function () {
  const { icon, btn } = this.element;

  btn.setAttribute('data-pct', '');
  icon.style.display = ''
}

Button.prototype.make = function ({ icon }) {
  const div = document.createElement('div');
  div.className = 'cont';
  div.setAttribute('data-pct', '');
  div.appendChild(icon);

  const { svgElem, ...rest } = createSVG();

  div.appendChild(svgElem);

  const btn = div;
  this.new = btn;

  return this.element = {
    btn,
    icon,
    svgElem,
    ...rest
  }
}

export default Button;