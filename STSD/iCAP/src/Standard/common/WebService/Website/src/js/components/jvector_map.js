export function vectorMap() {
    function getDOM(id) {
        var map_vector = document.createElement("div");

        map_vector.setAttribute("id", id);
        map_vector.setAttribute("class", "map_vector");
        return map_vector;
    }

    function renderChart(panel) {
        var data = panel.data;
        var getMarkerFormat = function (marker) {
            return [
                {
                    latLng: [marker.position.lat, marker.position.lng],
                    style: { fill: marker.color, r: 6 }
                }
            ];
        }
        var markers = (data.markers === undefined) ? getMarkerFormat(data.value[0]) : data.markers;
        var vetorMapSettingObj = {
            scaleColors: ['#C8EEFF', '#0071A4'],
            normalizeFunction: 'polynomial',
            hoverOpacity: 0.7,
            hoverColor: false,
            backgroundColor: '#383f47',//'#383f47'
            regionStyle: {
                initial: {
                    fill: '#b8e186'//'#A6D854
                },
                selected: {
                    fill: '#8DA0CB'
                },
            },
            markers: markers,
            markerStyle: {
                initial: {
                    fill: '#01fbd8',
                    stroke: '#505050',
                    "fill-opacity": 1,
                    "stroke-width": 1,
                    "stroke-opacity": 1,
                    r: 6
                },
                hover: {
                    stroke: 'black',
                    "stroke-width": 2,
                    cursor: 'pointer'
                }
            },
        };

        $('body > .jvectormap-tip').remove();

        switch (data.mapIndex) {
            case 1:
                require('../plugins/jquery-jvectormap-africa-mill');
                vetorMapSettingObj['map'] = 'africa_mill';
                vetorMapSettingObj['onMarkerTipShow'] = function (event, label, index) {
                    label.html(
                        '<b>Map : </b>' + 'Africa' + '<br/>' +
                        '<b>Device Count : </b>' + data.deviceCount[index] + '</br>' +
                        '<b>Event : </b>' + data.eventCount[index] + ' device(s)'
                    );
                };
                break;
            case 2:
                require('../plugins/jquery-jvectormap-asia-mill');
                vetorMapSettingObj['map'] = 'asia_mill';
                vetorMapSettingObj['onMarkerTipShow'] = function (event, label, index) {
                    label.html(
                        '<b>Map : </b>' + 'Asia' + '<br/>' +
                        '<b>Device Count : </b>' + data.deviceCount[index] + '</br>' +
                        '<b>Event : </b>' + data.eventCount[index] + ' device(s)'
                    );
                };
                break;
            case 3:
                require('../plugins/jquery-jvectormap-europe-mill');
                vetorMapSettingObj['map'] = 'europe_mill';
                vetorMapSettingObj['onMarkerTipShow'] = function (event, label, index) {
                    label.html(
                        '<b>Map : </b>' + 'Europe' + '<br/>' +
                        '<b>Device Count : </b>' + data.deviceCount[index] + '</br>' +
                        '<b>Event : </b>' + data.eventCount[index] + ' device(s)'
                    );
                };
                break;
            case 4:
                require('../plugins/jquery-jvectormap-north_america-mill');
                vetorMapSettingObj['map'] = 'north_america_mill';
                vetorMapSettingObj['onMarkerTipShow'] = function (event, label, index) {
                    label.html(
                        '<b>Map : </b>' + 'North America' + '<br/>' +
                        '<b>Device Count : </b>' + data.deviceCount[index] + '</br>' +
                        '<b>Event : </b>' + data.eventCount[index] + ' device(s)'
                    );
                };
                break;
            case 5:
                require('../plugins/jquery-jvectormap-oceania-mill');
                vetorMapSettingObj['map'] = 'oceania_mill';
                vetorMapSettingObj['onMarkerTipShow'] = function (event, label, index) {
                    label.html(
                        '<b>Map : </b>' + 'Oceania' + '<br/>' +
                        '<b>Device Count : </b>' + data.deviceCount[index] + '</br>' +
                        '<b>Event : </b>' + data.eventCount[index] + ' device(s)'
                    );
                };
                break;
            case 6:
                require('../plugins/jquery-jvectormap-south_america-mill');
                vetorMapSettingObj['map'] = 'south_america_mill';
                vetorMapSettingObj['onMarkerTipShow'] = function (event, label, index) {
                    label.html(
                        '<b>Map : </b>' + 'South America' + '<br/>' +
                        '<b>Device Count : </b>' + data.deviceCount[index] + '</br>' +
                        '<b>Event : </b>' + data.eventCount[index] + ' device(s)'
                    );
                };
                break;
            case 7:
                require('../plugins/jquery-jvectormap-world-mill');
                vetorMapSettingObj['map'] = 'world_mill';
                vetorMapSettingObj['onMarkerTipShow'] = function (event, label, index) {
                    label.html(
                        '<b>Map : </b>' + 'World' + '<br/>' +
                        '<b>Device Count : </b>' + data.deviceCount[index] + '</br>' +
                        '<b>Event : </b>' + data.eventCount[index] + ' device(s)'
                    );
                };
                break;
            default:
                require('../plugins/jquery-jvectormap-world-mill');
                vetorMapSettingObj['map'] = 'world_mill';
                vetorMapSettingObj['onMarkerTipShow'] = function (event, label, index) {
                    label.html(
                        '<b>Map : </b>' + 'World' + '<br/>' +
                        '<b>Device Count : </b>' + 1 + '</br>' +
                        '<b>Event : </b>' + data.value[0].status
                    );
                };
                break;
        }

        $('#' + panel.id).vectorMap(vetorMapSettingObj);

    }

    var publicAPI = {
        getDOM: getDOM,
        renderChart: renderChart
    };

    return publicAPI;
}