extends Control

onready var scene_tree: = get_tree()
onready var buttoncheck_overlay: ColorRect = get_node("ConfigOverlay")

signal QuitMainscene()

var paused: = false setget set_paused


func _unhandled_input(event: InputEvent) -> void:
	if event.is_action_pressed("buttonconfig") and Globals.get("mode") != 2:
		self.paused = not paused
		scene_tree.set_input_as_handled()
	
	
func set_paused(value: bool) -> void:
	paused = value
	scene_tree.paused = value
	buttoncheck_overlay.visible = value
	


onready var _action_list = get_node("ConfigOverlay/Column/ScrollContainer/ActionList")

func _ready():
	$ConfigOverlay/Column/ReturnMainMenu.connect("QuitPressed", self, "on_quit_pressed")
	
	$InputMapper.connect('profile_changed', self, 'rebuild')
	$ConfigOverlay/Column/ProfilesMenu.initialize($InputMapper)
	$InputMapper.change_profile($ConfigOverlay/Column/ProfilesMenu.selected)
	
func on_quit_pressed():
	scene_tree.paused = false
	emit_signal("QuitMainscene")
	#queue_free()
	
func rebuild(input_profile, is_customizable=false, id=0):
	_action_list.clear()
	for input_action in input_profile.keys():
		var line = _action_list.add_input_line(input_action, \
			input_profile[input_action], is_customizable, id == 0)
		if is_customizable:
			line.connect('change_button_pressed', self, \
				'_on_InputLine_change_button_pressed', [input_action, line])

func _on_InputLine_change_button_pressed(action_name, line):
	set_process_input(false)
	
	$KeySelectMenu.open()
	var key_scancode = yield($KeySelectMenu, "key_selected")
	$InputMapper.change_action_key(action_name, key_scancode)
	line.update_key(key_scancode)
	
	set_process_input(true)

#func _input(event):
#	if event.is_action_pressed('ui_cancel'):
#		get_tree().change_scene("res://Scenes/MainScene.tscn")

#func _on_PlayButton_pressed():
#	get_tree().change_scene("res://Scenes/MainScene.tscn")
