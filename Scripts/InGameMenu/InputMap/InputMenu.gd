extends Control

#loading nodes
#onready var scene_tree: = get_tree()
#onready var buttoncheck_overlay: ColorRect = get_node("ConfigOverlay")

#load player select dropdown
export (NodePath) var playerselect_path
onready var playerselect = get_node(playerselect_path)
#load actionlist node inside scroll container
onready var _action_list = get_node("ConfigOverlay/Column/ScrollContainer/ActionList")

func _ready():
	#SIGNALS
	#connect ReturnToInGameMenu to hide overlay
	Events.connect("BackPressed", self, "hide_overlay")
	#changing profiles triggers rebuild
	$InputMapper.connect('profile_changed', self, 'rebuild')
	
	#called once on ready
	#init profile
	$ConfigOverlay/Column/PlayerMenu/ProfilesMenu.initialize($InputMapper, 0)
	#change to profile_id 0
	$InputMapper.change_profile($ConfigOverlay/Column/PlayerMenu/ProfilesMenu.selected, 0)

#main rebuild function (called in by profile_changed)
func rebuild(input_profile, is_customizable=false, id=0):
	#clear moves out of actionlist
	_action_list.clear()
	for input_action in input_profile.keys():
		var line = _action_list.add_input_line(input_action, \
			input_profile[input_action], is_customizable, id == 0)
		if is_customizable:
			line.connect('change_button_pressed', self, \
				'_on_InputLine_change_button_pressed', [input_action, line])
	
	
#choosing player calls InputMapper intialize and change_profile
func _on_PlayerSelect_item_selected(index):
	$ConfigOverlay/Column/PlayerMenu/ProfilesMenu.initialize($InputMapper, index)
	$InputMapper.change_profile($ConfigOverlay/Column/PlayerMenu/ProfilesMenu.selected, index)

#make key rebind menu active
func _on_InputLine_change_button_pressed(action_name, line):
	set_process_input(false)
		
	$ConfigOverlay/KeySelectMenu.open()
	var key_scancode = yield($ConfigOverlay/KeySelectMenu, "key_selected")
	$InputMapper.change_action_key(action_name, key_scancode)
	line.update_key(key_scancode)
	set_process_input(true)

#hide overlay when done
func hide_overlay():
	$ConfigOverlay.visible = false
