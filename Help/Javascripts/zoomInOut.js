var cd3;
var myCamera;
var firstClick = true;

function scaleThis(event) {
    cd3 = this.getAnnots3D(0)[0].context3D;
    if ((cd3 !== null) && (typeof cd3 != "undefined")) {
        myCamera = cd3.scene.cameras.getByIndex(0);
    }
    else {
        firstClick = true;
    }
    if ((cd3 !== null) && (typeof cd3 != "undefined")) {
        if ((myCamera !== null) && (typeof myCamera != "undefined")) {
            if (firstClick === true) {
                firstClick = false;
                myCamera.targetPosition.set3(0.0, 0.0, 0.0);
            }
            if (event.target.name == "ZoomOut") {
                // console.println("target is " + myCamera.targetPosition + " " + myCamera.targetPositionLocal);
                scale(1.2);
            }
            else if (event.target.name == "ZoomIn") {
                scale(0.8);
            }
        }
    }
}

function scale(scaleFactor) {
    var cameraVector = new cd3.Vector3(myCamera.position);
    cameraVector.subtractInPlace(myCamera.targetPosition);
    cameraVector.scaleInPlace(scaleFactor);
    cameraVector.addInPlace(myCamera.targetPosition);
    myCamera.position.set(cameraVector);
}