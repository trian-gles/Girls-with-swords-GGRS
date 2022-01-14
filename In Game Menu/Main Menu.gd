extends Button

export(String, FILE) var next_scene_path: =""

func _on_button_down():
	get_tree().change_scene(next_scene_path)
	
