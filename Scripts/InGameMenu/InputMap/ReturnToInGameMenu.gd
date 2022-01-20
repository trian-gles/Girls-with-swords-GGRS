extends Button

func _on_button_down():
	Events.emit_signal("BackPressed")
	Events.emit_signal("PausePressed")
	

