var myToolHandler = new ToolEventHandler(); 

runtime.disableTool("Rotate");
runtime.disableTool("Spin");
runtime.disableTool("Walk");
runtime.disableTool("Zoom");
runtime.disableTool("Pan");
runtime.disableTool("Fly");



//runtime.addCustomToolButton("panButton", "Pan Button", "tool button"); 

myToolHandler.onEvent = function(ToolEvent) 
{ 
	console.println("ToolEvent.toolName = " + ToolEvent.toolName); 
	
	if (ToolEvent.toolName == "panButton") 
	{ 
		runtime.removeCustomToolButton("panButton");
		console.println("You clicked the custom button 'My Button'"); 
	} 
} 
runtime.addEventHandler(myToolHandler);

