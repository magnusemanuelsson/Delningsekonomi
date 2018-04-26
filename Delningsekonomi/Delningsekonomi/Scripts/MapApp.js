/*
 * Copyright 2017 Google Inc. All rights reserved.
 *
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this
 * file except in compliance with the License. You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
 * ANY KIND, either express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */

// Style credit: https://snazzymaps.com/style/1/pale-dawn
const mapStyle = [
    {
        "featureType": "administrative",
        "elementType": "all",
        "stylers": [
            {
                "visibility": "on"
            },
            {
                "lightness": 33
            }
        ]
    },
    {
        "featureType": "landscape",
        "elementType": "all",
        "stylers": [
            {
                "color": "#f2e5d4"
            }
        ]
    },
    {
        "featureType": "poi.park",
        "elementType": "geometry",
        "stylers": [
            {
                "color": "#c5dac6"
            }
        ]
    },
    {
        "featureType": "poi.park",
        "elementType": "labels",
        "stylers": [
            {
                "visibility": "on"
            },
            {
                "lightness": 20
            }
        ]
    },
    {
        "featureType": "road",
        "elementType": "all",
        "stylers": [
            {
                "lightness": 20
            }
        ]
    },
    {
        "featureType": "road.highway",
        "elementType": "geometry",
        "stylers": [
            {
                "color": "#c5c6c6"
            }
        ]
    },
    {
        "featureType": "road.arterial",
        "elementType": "geometry",
        "stylers": [
            {
                "color": "#e4d7c6"
            }
        ]
    },
    {
        "featureType": "road.local",
        "elementType": "geometry",
        "stylers": [
            {
                "color": "#fbfaf7"
            }
        ]
    },
    {
        "featureType": "water",
        "elementType": "all",
        "stylers": [
            {
                "visibility": "on"
            },
            {
                "color": "#acbcc9"
            }
        ]
    }
];

var currentLat;
var currentLong;
var currentRadius;

function initialize() {
    currentLat = startLat;
    currentLong = startLng;
    currentRadius = startRadius;
    initMap();
    initAutocomplete();
}

// Escapes HTML characters in a template literal string, to prevent XSS.
// See https://www.owasp.org/index.php/XSS_%28Cross_Site_Scripting%29_Prevention_Cheat_Sheet#RULE_.231_-_HTML_Escape_Before_Inserting_Untrusted_Data_into_HTML_Element_Content
function sanitizeHTML(strings) {
    const entities = { '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' };
    let result = strings[0];
    for (let i = 1; i < arguments.length; i++) {
        result += String(arguments[i]).replace(/[&<>'"]/g, (char) => {
            return entities[char];
        });
        result += strings[i];
    }
    return result;
}

var map;

function initMap() {

    // Create the map.
    map = new google.maps.Map(document.getElementsByClassName('map')[0], {
        zoom: 14,
        center: { lat: currentLat, lng: currentLong },
        styles: mapStyle,
        zoomControl: false,
        mapTypeControl: false,
        scaleControl: false,
        streetViewControl: false,
        rotateControl: false,
        fullscreenControl: false
    });

    var centerCircle = new google.maps.Circle({
        center: new google.maps.LatLng(currentLat, currentLong),
        strokeColor: 'DeepSkyBlue',
        strokeOpacity: 0.8,
        strokeWeight: 2,
        fillOpacity: 0, 
        radius: currentRadius,
        map: map,
        zIndex: -1001
    });

    var centerMarker = new google.maps.Marker({
        position: new google.maps.LatLng(currentLat, currentLong),
        icon: {
            path: google.maps.SymbolPath.CIRCLE,
            scale: 8,
            strokeColor: 'White',
            fillColor: 'DeepSkyBlue',
            fillOpacity: 1,
            strokeWeight: 2
        },
        draggable: false,
        map: map,
        zIndex: -1000
    });

    map.fitBounds(centerCircle.getBounds());

    // Load the stores GeoJSON onto the map.
    // map.data.loadGeoJson('Scripts/points.json');
    map.data.addGeoJson(myAPIpoints);

    // Define the custom marker icons, using the store's "category".
    map.data.setStyle(feature => {
        return {
            icon: {
                url: `Images/${feature.getProperty('category')}.png`,
                scaledSize: new google.maps.Size(64, 64)
            }
        };
    });

    const apiKey = 'AIzaSyDeZ0LKWiLMNWqQGI_L5lLzo8ZN9Sa8AHU';
    var maxWidthResponsive = 100vw;
    const infoWindow = new google.maps.InfoWindow({ maxWidth: 75vw });
    infoWindow.setOptions({ pixelOffset: new google.maps.Size(0, -64) });

    // Show the information for a store when its marker is clicked.
    map.data.addListener('click', event => {

        const category = event.feature.getProperty('category');
        const name = event.feature.getProperty('name');
        const description = event.feature.getProperty('description');
        const hours = event.feature.getProperty('hours');
        const phone = event.feature.getProperty('phone');
        const position = event.feature.getGeometry().get();
        //<img style="float:left; width:64px;  margin-top:30px" src="Images/${category}.png">
        const content = sanitizeHTML`
            <div id="tooltip" style="margin-left:24px; margin-bottom:20px;">
            <h2>${name}</h2><p>${description}</p>
            <p><b>Open:</b> ${hours}<br/><b>Phone:</b> ${phone}</p>
            </div>
            `;

        infoWindow.setContent(content);
        infoWindow.setPosition(position);
        infoWindow.open(map);
    });

}

function openBurger() {
    document.getElementById("mySidenav").style.width = "250px";
}

function closeBurger() {
    document.getElementById("mySidenav").style.width = "0";
}



function initAutocomplete() {


    // Create the search box and link it to the UI element.
    var input = document.getElementById('pac-input');
    var searchBox = new google.maps.places.Autocomplete(input);

    // Bias the SearchBox results towards current map's viewport.
    map.addListener('bounds_changed', function () {
    searchBox.setBounds(map.getBounds());
    });
    
    var markers = [];
    // Listen for the event fired when the user selects a prediction and retrieve
    // more details for that place.
    google.maps.event.addListener(searchBox, 'place_changed', function () {
        var place = searchBox.getPlace();
        document.getElementById('city2').value = place.name;
        document.getElementById('cityLat').value = place.geometry.location.lat();
        document.getElementById('cityLng').value = place.geometry.location.lng();
        currentLat = place.geometry.location.lat();
        currentLong = place.geometry.location.lng();
        console.log("latitude: " + place.geometry.location.lat() + "  longitude: " + place.geometry.location.lng());
        google.maps.event.addDomListener(window, 'load', initialize);
        document.getElementById("form").submit();
    });
}