
export default function updateOOBLEDStatus({ element, status }) {
  if (!element) return;

  if (`${status}` === '0') {
    element.className = 'led grey';
  } else if (`${status}` === '1') {
    element.className = 'led green';
  } else if (`${status}` === '-1') {
    element.className = 'led orange';
  }
  element.status = status;
}