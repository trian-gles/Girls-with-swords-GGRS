extends Button
onready var scene_tree: = get_tree()

func _on_button_down():
	scene_tree.paused = false
	Events.emit_signal("MainMenuPressed")
