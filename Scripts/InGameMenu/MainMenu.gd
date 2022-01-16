extends Button


export(String, FILE) var next_scene_path: =""

func _on_button_down():
	#get_tree().change_scene(next_scene_path)
	emit_signal("LobbyReturn")
	

#focus for menu
func _on_PauseOverlay_visibility_changed():
	grab_focus()
 
