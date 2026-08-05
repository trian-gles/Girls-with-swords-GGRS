extends Control

onready var scene_tree: = get_tree()
onready var pause_overlay: ColorRect = $PauseOverlay
var paused: = false setget set_paused

func _ready():
# warning-ignore:return_value_discarded
	Events.connect("ButtonConfigPressed", self, "hide_in_game_menu")
# warning-ignore:return_value_discarded
	Events.connect("BackPressed", self, "show_in_game_menu")
	
func _unhandled_input(event: InputEvent) -> void:
	if event.is_action_pressed("pause") and Globals.get("mode") != 2:
		Events.emit_signal("PausePressed")
		self.paused = not paused
		scene_tree.set_input_as_handled()
		
	
func set_paused(value: bool) -> void:
	paused = value
	scene_tree.paused = value
	visible = value
	if (value):
		var in_training = get_node("/root/Globals").call("CheckTrainingMode")
		$PauseOverlay/PauseMenu/AutoBlock.visible = in_training
		$PauseOverlay/PauseMenu/AutoTech.visible = in_training
		$P1Inputs/P1Controls.config_inputs()
		$P2Inputs/P1Controls.config_inputs()
		$SpecialInputs/Container.config_inputs()
		
func _on_AutoBlock_toggled(button_pressed):
	if is_inside_tree():
		get_node("/root/Globals").call("SetAlwaysBlock", button_pressed)


func _on_AutoTech_toggled(button_pressed):
	if is_inside_tree():
		get_node("/root/Globals").call("SetAutoTech", button_pressed)


func _on_MainMenu_button_down():
	set_paused(false)
	Events.emit_signal("MainMenuPressed")
	$PauseOverlay/PauseMenu/AutoBlock.pressed = false
	$PauseOverlay/PauseMenu/AutoTech.pressed = false
