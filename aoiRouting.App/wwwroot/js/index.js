var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
let map;
let meMarker;
let pinMarkers = {};
let pins = {};
let mousedUp;
let mapNetObject;
let meIcon, centroidIcon, pendingIcon, completedIcon;
let PinPopup;
const meSize = 50;
const pinIconSize = 40;
function getLocation(netObject) {
    function returnPosition(position) {
        return __awaiter(this, void 0, void 0, function* () {
            yield netObject.invokeMethodAsync('SetLocation', position.coords.latitude, position.coords.longitude);
        });
    }
    function positionError(error) { }
    if (navigator.geolocation) {
        navigator.geolocation.watchPosition(returnPosition, positionError);
        return true;
    }
    else {
        return false;
    }
}
function initMap(netObject, elementId, lat, lon, zoom) {
    initResources();
    mapNetObject = netObject;
    let latLng = new google.maps.LatLng(lat, lon);
    let options = {
        zoom: zoom,
        center: latLng,
        mapTypeId: 'hybrid',
        streetViewControl: false,
        scaleControl: true,
        zoomControl: true,
        mapTypeControl: false,
        fullscreenControl: false,
        clickableIcons: false,
        tilt: 0
    };
    let mapElement = document.getElementById(elementId);
    map = new google.maps.Map(mapElement, options);
    pinMarkers = {};
    pins = {};
    mousedUp = false;
    map.addListener('mousedown', (event) => {
        mousedUp = false;
        setTimeout(() => __awaiter(this, void 0, void 0, function* () {
            if (mousedUp === false) {
                let lat = event.latLng.lat();
                let lon = event.latLng.lng();
                yield netObject.invokeMethodAsync("OnLongPress", lat, lon);
            }
        }), 500);
    });
    map.addListener('mouseup', () => mousedUp = true);
    map.addListener('dragstart', () => mousedUp = true);
    //TO-DO: INTEGRATE WATER console.log(isWaterLocation(40.547132, -74.391260));
}
function initResources() {
    meIcon =
        {
            url: "/assets/icons/man.svg",
            scaledSize: new google.maps.Size(meSize, meSize),
            origin: new google.maps.Point(0, 0),
            anchor: new google.maps.Point(meSize / 2, meSize / 2)
        };
    centroidIcon =
        {
            url: "/assets/icons/centroid.svg",
            scaledSize: new google.maps.Size(pinIconSize, pinIconSize),
            origin: new google.maps.Point(0, 0),
            anchor: new google.maps.Point(pinIconSize / 2, pinIconSize / 2)
        };
    pendingIcon =
        {
            url: "/assets/icons/pending.svg",
            scaledSize: new google.maps.Size(pinIconSize, pinIconSize),
            origin: new google.maps.Point(0, 0),
            anchor: new google.maps.Point(pinIconSize / 2, pinIconSize / 2)
        };
    completedIcon =
        {
            url: "/assets/icons/collected.svg",
            scaledSize: new google.maps.Size(pinIconSize, pinIconSize),
            origin: new google.maps.Point(0, 0),
            anchor: new google.maps.Point(pinIconSize / 2, pinIconSize / 2)
        };
    PinPopup = class extends google.maps.OverlayView {
        constructor(marker, content) {
            super();
            this.marker = marker;
            this.position = marker.getPosition();
            let closeContainer = document.createElement('div');
            closeContainer.classList.add("popup-bubble");
            const closeIcon = document.createElement("ion-icon");
            closeIcon["name"] = "close";
            closeIcon.onclick = () => { this.setMap(null); };
            closeContainer.appendChild(content);
            closeContainer.appendChild(closeIcon);
            closeContainer.setAttribute('style', 'border: thin solid white;');
            const bubbleAnchor = document.createElement("div");
            bubbleAnchor.classList.add("popup-bubble-anchor");
            bubbleAnchor.appendChild(closeContainer);
            this.containerDiv = document.createElement("div");
            this.containerDiv.classList.add("popup-container");
            this.containerDiv.appendChild(bubbleAnchor);
            PinPopup.preventMapHitsAndGesturesFrom(this.containerDiv);
        }
        onAdd() {
            this.getPanes().floatPane.appendChild(this.containerDiv);
        }
        onRemove() {
            if (this.containerDiv.parentElement) {
                this.containerDiv.parentElement.removeChild(this.containerDiv);
            }
        }
        draw() {
            const divPosition = this.getProjection().fromLatLngToDivPixel(this.position);
            const display = "block";
            if (display === "block") {
                this.containerDiv.style.left = divPosition.x + "px";
                this.containerDiv.style.top = divPosition.y + "px";
            }
            if (this.containerDiv.style.display !== display) {
                this.containerDiv.style.display = display;
            }
        }
    };
}
function moveMeMarker(lat, lon) {
    let pos = new google.maps.LatLng(lat, lon);
    if (meMarker == undefined) {
        meMarker = new google.maps.Marker({
            position: pos,
            map: map,
            icon: meIcon
        });
    }
    else {
        meMarker.setPosition(pos);
    }
}
let curInfoWindow;
let curInfoWindowId;
function previewPin(id) {
    let pin = pins[id];
    let marker = pinMarkers[id];
    let content = document.createElement("div");
    content.className = "google-map-info";
    content.innerHTML = `<h2>${pin.pointID}</h2>`;
    content.onclick = () => pinDetails(id);
    curInfoWindowId = id;
    curInfoWindow = new PinPopup(marker, content);
    curInfoWindow.setMap(map);
}
function isWaterLocation(latitude, longitude) {
    return __awaiter(this, void 0, void 0, function* () {
        const url = `https://isitwater-com.p.rapidapi.com/?latitude=${latitude}&longitude=${longitude}`;
        const options = {
            method: 'GET',
            headers: {
                'X-RapidAPI-Key': 'KEY-HERE',
                'X-RapidAPI-Host': 'isitwater-com.p.rapidapi.com'
            }
        };
        try {
            const response = yield fetch(url, options);
            const result = yield response.json();
            console.log(result);
            console.log(result.water);
            console.log(result.water == 'water');
            return result.water == 'water';
        }
        catch (error) {
            console.error(error);
            return false;
        }
    });
}
function placePin(pin) {
    pins[pin.id] = pin;
    let i;
    if (pin.collected != null) {
        i = completedIcon;
    }
    else {
        i = pendingIcon;
    }
    let marker = new google.maps.Marker({
        position: { lat: pin.lat, lng: pin.lon },
        map: map,
        icon: i,
        draggable: true
    });
    marker.addListener("click", () => previewPin(pin.id));
    marker.addListener("dragend", () => {
        pin.lat = marker.getPosition().lat();
        pin.lon = marker.getPosition().lng();
        //mapNetObject.invokeMethodAsync('UpdateLocation', pin);
    });
    pinMarkers[pin.id] = marker;
}
function pinDetails(id) {
    mapNetObject.invokeMethodAsync('ViewDetails', id);
}
function haversine_distance(a, b) {
    var R = 6371.0710; // Radius of the Earth in kilometers
    var rlat1 = a.lat * (Math.PI / 180); // Convert degrees to radians
    var rlat2 = b.lat * (Math.PI / 180); // Convert degrees to radians
    var difflat = rlat2 - rlat1; // Radian difference (latitudes)
    var difflon = (b.lng - a.lng) * (Math.PI / 180); // Radian difference (longitudes)
    var d = 2 * R * Math.asin(Math.sqrt(Math.sin(difflat / 2) * Math.sin(difflat / 2) + Math.cos(rlat1) * Math.cos(rlat2) * Math.sin(difflon / 2) * Math.sin(difflon / 2)));
    return d;
}
function findNearestPins(n) {
    n = Math.min(n, Object.keys(pins).length);
    var meMarkerCoords = { lat: meMarker.getPosition().lat(), lng: meMarker.getPosition().lng() };
    var sortedPins = Object.keys(pins).sort(function (a, b) {
        var aCoords = { lat: pins[a].lat, lng: pins[a].lon };
        var bCoords = { lat: pins[b].lat, lng: pins[b].lon };
        return haversine_distance(meMarkerCoords, aCoords) - haversine_distance(meMarkerCoords, bCoords);
    });
    var nearestPins = sortedPins.slice(0, n);
    var coordinates = nearestPins.map(function (pinId) {
        var pinMarker = pinMarkers[pinId];
        return { lat: pinMarker.getPosition().lat(), lng: pinMarker.getPosition().lng() };
    });
    var optimizedRoute = [meMarkerCoords];
    var currentPoint = meMarkerCoords;
    while (coordinates.length > 0) {
        var closestPin;
        var minDistance = Number.MAX_VALUE;
        for (var i = 0; i < coordinates.length; i++) {
            var distance = haversine_distance(currentPoint, coordinates[i]);
            if (distance < minDistance) {
                minDistance = distance;
                closestPin = i;
            }
        }
        optimizedRoute.push(coordinates[closestPin]);
        currentPoint = coordinates[closestPin];
        coordinates.splice(closestPin, 1);
    }
    optimizedRoute.push(meMarkerCoords);
    for (var i = 0; i < optimizedRoute.length - 1; i++) {
        var line = new google.maps.Polyline({
            path: [new google.maps.LatLng(optimizedRoute[i].lat, optimizedRoute[i].lng),
                new google.maps.LatLng(optimizedRoute[i + 1].lat, optimizedRoute[i + 1].lng)],
            map: map
        });
    }
}
let longPressIndicator;
function placeLongPressIndicator(lat, lon) {
    let pos = { lat: lat, lng: lon };
    if (longPressIndicator == undefined) {
        longPressIndicator = new google.maps.Marker({
            position: pos,
            map: map,
            icon: {
                url: "/assets/icons/centroid.svg",
                scaledSize: new google.maps.Size(meSize, meSize),
                origin: new google.maps.Point(0, 0),
                anchor: new google.maps.Point(meSize / 2, meSize),
            }
        });
    }
    else {
        longPressIndicator.setPosition(pos);
    }
}
function removeLongPressIndicator() {
    if (longPressIndicator != undefined) {
        longPressIndicator.setMap(null);
        longPressIndicator = undefined;
    }
}
//# sourceMappingURL=index.js.map