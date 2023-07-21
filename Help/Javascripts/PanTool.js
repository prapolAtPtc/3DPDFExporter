var  mouse_XPrevious = 0.0;
var  mouse_YPrevious = 0.0;

// if "myButton" tool button exist then remove "myButton" tool button from toolbar
//runtime.removeCustomToolButton("panButton");
// add "myButton" tool button in toolbar
//addCustomToolButton(name, label, type)
runtime.addCustomToolButton("panButton", "Pan Button", "tool button"); 
var mouseEvent  = new MouseEventHandler();
    mouseEvent.onMouseDown    = true;
    mouseEvent.onMouseMove    = true;
    mouseEvent.onMouseUp      = true;
mouseEvent.onEvent     = function( event )
{	
	if(event.currentTool=="panButton")
	{ 
        	var camera = scene.cameras.getByIndex(0);
		
			if( event.rightButtonDown  )
			{
		    				
				var xDiff   = -( event.mouseX - mouse_XPrevious )*0.09;
				var yDiff   = ( event.mouseY - mouse_YPrevious )*0.09;
				var constcamerapos = camera.position.subtract(camera.targetPosition);
				 
				console.println("Xdiff= "+xDiff);
				console.println("yDiff= "+yDiff);

				
				var Upvector=new Vector3(camera.up.x,camera.up.y,camera.up.z);
				Upvector.normalize();
		        console.println("Upvector before= "+camera.up);
				 console.println("UpvectorLocal before= "+camera.upLocal);
				var viewVector= camera.targetPosition.subtract(camera.position);
				viewVector.normalize();
				//console.println("viewVector= "+viewVector);
				
				var rightVector=viewVector.cross(Upvector);
				rightVector.normalize();
		        console.println("viewVector.length= "+viewVector.length);
				
				
				rightVector.x*=xDiff*viewVector.length;
				rightVector.y*=xDiff*viewVector.length;
				rightVector.z*=xDiff*viewVector.length;
				
				Upvector.x*=yDiff*viewVector.length;
				Upvector.y*=yDiff*viewVector.length;
				Upvector.z*=yDiff*viewVector.length;
				
				//console.println("Before camera.targetPosition= "+ camera.targetPosition);
				//console.println("Before camera.position= "+ camera.position);
				var translate=Upvector.add(rightVector);
				
				camera.position.addInPlace(translate);
			    camera.targetPosition.subtractInPlace(translate);
				//camera.up.set(camera.targetPosition.x, camera.targetPosition.y, camera.targetPosition.z + viewVector.length);
				console.println("Upvector after= "+camera.up);
				console.println("UpvectorLocal after= "+camera.upLocal);
				camera.roll=0;

				
			}
		
	}
	
	mouse_XPrevious = event.mouseX;
	mouse_YPrevious = event.mouseY;
}
runtime.addEventHandler( mouseEvent );