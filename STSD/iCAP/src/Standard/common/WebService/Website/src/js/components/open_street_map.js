import marker_green from "../../assets/images/marker-green.png";
import marker_red from "../../assets/images/marker-red.png";
import Map from 'ol/Map.js';
import View from 'ol/View.js';
import { defaults as defaultControls, FullScreen } from 'ol/control.js';
import OSM from 'ol/source/OSM.js';
import Feature from 'ol/Feature.js';
import Point from 'ol/geom/Point.js';
import { Icon, Style } from 'ol/style.js';
import VectorSource from 'ol/source/Vector.js';
import Overlay from 'ol/Overlay.js';
import { Tile as TileLayer, Vector as VectorLayer } from 'ol/layer.js';
import { timeConverter } from "../library/common";

export function openStreetMap() {
    function getDOM(id) {
        var osm = document.createElement("div");
        var popup = document.createElement('div');
        var closer = document.createElement('a');
        var content = document.createElement('div');

        closer.setAttribute('href', '#');
        closer.setAttribute('id', `popup-closer-${id}`);
        closer.setAttribute('class', 'ol-popup-closer');

        content.setAttribute('id', `popup-content-${id}`);

        popup.setAttribute('id', `popup-${id}`);
        popup.setAttribute('class', 'ol-popup');
        popup.appendChild(closer);
        popup.appendChild(content);

        osm.setAttribute('id', `map-${id}`);
        osm.setAttribute('style', 'height: 100%;');
        osm.setAttribute('class', 'map-osm');
        osm.appendChild(popup);
        //$(osm).append(popup);
        return osm;

    }

    function renderChart(panel) {
        var data = panel.data;
        var content = document.getElementById(`popup-content-${panel.id}`);
        var closer = document.getElementById(`popup-closer-${panel.id}`);
        var element = document.getElementById(`popup-${panel.id}`);
        var features = [];
        var vector, vectorLayer, map, popup;
        var rasterLayer = new TileLayer({
            source: new OSM()
        });

        data.value && data.value.forEach(device => {
            var contentHTML = '<div class="col-md-12 text-center">' +
                '<div><h5>' + ((device.alias === null) ? device.name : device.alias) + '</h5></div>' +
                '<div><h5>' + device.status + '<br/><small>Status</small></h5></div>' +
                '<div><h5>' + ((device.status === 'NORMAL') ? 'N/A' : timeConverter(device.time)) + '<br/><small>Event time</small></h5></div>' +
                '<div><h5>' + device.detail + '<br/><small>Message</small></h5></div>' +
                '</div>';
            var iconStyle;
            var feature = new Feature({
                geometry: new Point([device.position.lng, device.position.lat]),
                info: contentHTML,
                autoPan: true,
                autoPanAnimation: {
                    duration: 250
                }
            });

            if (device.status === 'WARNING') {
                iconStyle = new Style({
                    image: new Icon(/** @type {module:ol/style/Icon~Options} */({
                        anchor: [0.5, 46],
                        anchorXUnits: 'fraction',
                        anchorYUnits: 'pixels',
                        src: marker_red
                    }))
                });
            } else {
                iconStyle = new Style({
                    image: new Icon(/** @type {module:ol/style/Icon~Options} */({
                        anchor: [0.5, 46],
                        anchorXUnits: 'fraction',
                        anchorYUnits: 'pixels',
                        src: marker_green
                    }))
                });
            }

            feature.setStyle(iconStyle);
            features.push(feature);
        });

        vector = new VectorSource({
            features: features
        });

        vectorLayer = new VectorLayer({
            source: vector
        });

        popup = new Overlay({
            element: element,
            positioning: 'bottom-center',
            stopEvent: false,
            offset: [0, -40],
            autoPan: true,
            autoPanAnimation: {
                duration: 250
            }
        });

        closer.onclick = function () {
            popup.setPosition(undefined);
            closer.blur();
            return false;
        };

        $(document).ready(() => {
            map = new Map({
                controls: defaultControls().extend([
                    new FullScreen()
                ]),
                layers: [rasterLayer, vectorLayer],
                target: document.getElementById(`map-${panel.id}`),
                view: new View({
                    projection: 'EPSG:4326',
                    center: [data.centerLng, data.centerLat],
                    zoom: 16
                })
            });

            map.on('click', function (evt) {
                var feature = map.forEachFeatureAtPixel(evt.pixel,
                    function (feature) {
                        return feature;
                    });

                if (feature) {
                    var coordinate = feature.getGeometry().getCoordinates();

                    content.innerHTML = feature.get('info');
                    popup.setPosition(coordinate);
                } else {
                    popup.setPosition(undefined);
                }
            });

            map.on('pointermove', function (e) {
                if (e.dragging) {
                    $(element).popover('destroy');
                    return;
                }
                var pixel = map.getEventPixel(e.originalEvent);
                var hit = map.hasFeatureAtPixel(pixel);

                map.getTarget().style.cursor = hit ? 'pointer' : '';
            });

            map.addOverlay(popup);
        });
    }

    var publicAPI = {
        getDOM: getDOM,
        renderChart: renderChart
    };

    return publicAPI;
}