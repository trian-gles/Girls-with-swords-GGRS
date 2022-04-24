extends Button
#this button only appears in local game

func _on_button_down():
	Events.emit_signal("BackPressed")
	Events.emit_signal("PausePressed")
	

