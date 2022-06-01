import icon_status_off from '../../../../assets/images/icon_status_off.png';
import icon_status_on from '../../../../assets/images/icon_status_on.png';

export default function ({ element, status }) {
  if (!element) return;

  // console.log('status: ', status)
  // console.log('element: ', element)

  if (+status === null) {
    element.src = icon_status_off;
  } else if (+status === 0) {
    element.src = icon_status_off;
  } else if (+status === 1) {
    element.src = icon_status_on;
  }

  element.alt = status;
}