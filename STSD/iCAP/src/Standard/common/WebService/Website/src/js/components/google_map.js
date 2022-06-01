import marker_green from "../../assets/images/marker-green.png";
import marker_red from "../../assets/images/marker-red.png";
import marker_gray from "../../assets/images/marker-none.png";
import { timeConverter } from "../library/common";
import getGoogleMapKey from '../helpers/googlemap_key';

(function IIFE(global, $) {
    var Element = function (panel) {
        return new Element.init(panel);
    }

    var GoogleMapsLoader = require('google-maps');;
    var LoadDone = false;
    var OpenedInfoWindow;
    var MarkerClick = false;

    Element.init = function (panel, center_longitude, center_latitude) {
        var self = this;

        self.panel = panel;
        // GoogleMapsLoader = require('google-maps');
        // GoogleMapsLoader.KEY = await getGoogleMapKey();
        // self.center_latitude = center_latitude;
        // self.center_longitude = center_longitude;
    }

    Element.prototype = {
        getDOM: function () {
            var marker = [];
            var icon_path;
            var googleMap = document.createElement("div");
            var data = this.panel.data;

            googleMap.setAttribute("class", "map_google");
            getGoogleMapKey().then((key)=>{
                GoogleMapsLoader.KEY = key;
                GoogleMapsLoader.load((google) => {
                    // setTimeout will put google map initiation into next queue,
                    // Therefore, google will be initiated after map mount on DOM.
                    setTimeout(() => {
                        this.map = new google.maps.Map(googleMap, {
                            center: new google.maps.LatLng(data.centerLat, data.centerLng),
                            zoom: 15,
                            scrollwheel: false,
                            gestureHandling: 'greedy',
                            mapTypeId: google.maps.MapTypeId.ROADMAP
                        });
                        for (var i = 0; i < data.value.length; i++) {
                            var item = data.value[i];
                            var text = '<div class="col-md-12 text-center">' +
                                '<div><h5>' + ((Boolean(item.alias)) ? item.alias : item.name) + '</h5></div>' +
                                '<div><h5>' + item.status + '<br/><small>Status</small></h5></div>' +
                                '<div><h5>' + ((item.status === 'NORMAL') ? 'N/A' : timeConverter(item.time)) + '<br/><small>Event time</small></h5></div>' +
                                '<div><h5>' + item.detail + '<br/><small>Message</small></h5></div>' +
                                '</div>';
    
                            if (item.color == 'red') {
                                icon_path = marker_red;
                            } else {
                                icon_path = marker_green;
                            }
    
    
                            marker[i] = new google.maps.Marker({
                                position: item.position,
                                map: this.map,
                                icon: icon_path,
                                title: text
                            });
    
                            marker[i].addListener('click', function (data) {
                                if (OpenedInfoWindow) {
                                    OpenedInfoWindow.close();
                                }
                                OpenedInfoWindow = new google.maps.InfoWindow({
                                    content: this.title,
                                    position: this.position
                                });
                                OpenedInfoWindow.open(this.map, marker[i]);
                                MarkerClick = true;
                            });
    
                            marker[i].addListener('mouseover', function (data) {
                                if (data.xa !== undefined) {
                                    data.xa.target.title = '';
                                    data.xa.target.parentElement.removeAttribute('title');
                                } else if (data.wa !== undefined) {
                                    data.wa.target.title = '';
                                    data.wa.target.parentElement.removeAttribute('title');
                                } else if (data.ya !== undefined) {
                                    data.ya.target.title = '';
                                    data.ya.target.parentElement.removeAttribute('title');
                                } else if (data.va !== undefined) {
                                    data.va.target.title = '';
                                    data.va.target.parentElement.removeAttribute('title');
                                } else if (data.za !== undefined) {
                                    data.za.target.title = '';
                                    data.za.target.parentElement.removeAttribute('title');
                                }
                            });
                        }
                        LoadDone = true;
    
                        google.maps.event.addDomListener(googleMap, 'click', function () {
                            if (OpenedInfoWindow && !MarkerClick) {
                                OpenedInfoWindow.close();
                                OpenedInfoWindow = null;
                            }
                            MarkerClick = false;
                        });
                    });
                }, 0);

            });

            return googleMap;
        },
    }

    Element.init.prototype = Element.prototype;

    global.$MapCard = Element;

})(window, jQuery);