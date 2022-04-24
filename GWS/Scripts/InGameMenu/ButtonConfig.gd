extends Button

func _on_button_down():
	Events.emit_signal("ButtonConfigPressed")
