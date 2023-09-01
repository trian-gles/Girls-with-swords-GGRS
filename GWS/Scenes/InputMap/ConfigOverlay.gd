extends ColorRect

var save_path = "user://ControllerConfig.json"

func _ready():
	pass

func _on_ReturnToInGameMenu_pressed():
	save_JSON()
func _on_ReturnMainMenu_pressed():
	pass
func _on_PlayerSelect_item_selected(index):
	pass # Replace with function body.

func save_JSON():
	
	print("Saving Controller Config",value_data)
	var file
	file = File.new()
	file.open(save_path, File.WRITE)
	file.store_line(to_json(value_data))
	file.close()
	pass
