extends Node


var AudioConfig
var ControllerConfig

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


func load_JSON(savetype:String):
	var config_file = File.new()
	if config_file.open("res://Saves/"+savetype+".json", File.READ)== OK:
		var config_json = JSON.parse(config_file.get_as_text())
		config_file.close()
		if savetype == "audio":
			AudioConfig = config_json.result
		else :
			ControllerConfig = config_json.result
	else:
		print("Error loading Config, not found")
	
func save_AudioConfig():
	pass
	
