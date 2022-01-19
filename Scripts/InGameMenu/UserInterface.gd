extends Control

onready var scene_tree: = get_tree()
onready var pause_overlay: ColorRect = get_node("PauseOverlay")

signal QuitMainscene()

var paused: = false setget set_paused

func _ready():
	$PauseOverlay/PauseMenu/MainMenu.connect("QuitPressed", self, "on_quit_pressed")

func on_quit_pressed():
	scene_tree.paused = false
	emit_signal("QuitMainscene")

func _unhandled_input(event: InputEvent) -> void:
	if event.is_action_pressed("pause"):
		self.paused = not paused
		scene_tree.set_input_as_handled()
	
func set_paused(value: bool) -> void:
	paused = value
	scene_tree.paused = value
	pause_overlay.visible = value
	
#func _on_MainMenu_button_down():
#	scene_tree.paused = false
	

