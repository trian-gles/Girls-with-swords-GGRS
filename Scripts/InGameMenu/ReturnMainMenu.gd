extends Button

#this button only appears from main menu, doesn't need to pause!
#for unpausing before leaving
onready var scene_tree: = get_tree()

func _on_button_down():
	scene_tree.paused = false
	
	#signal shows menu buttons again
	Events.emit_signal("MainMenuPressed")
