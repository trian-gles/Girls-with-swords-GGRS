extends Button

onready var scene_tree: = get_tree()

func _ready():
	Events.connect("PausePressed", self, "focused")
	
func _on_button_down():
	scene_tree.paused = false
	Events.emit_signal("MainMenuPressed")

func focused():
	grab_focus()
