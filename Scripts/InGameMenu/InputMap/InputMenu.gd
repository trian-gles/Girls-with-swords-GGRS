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

#main rebuild function (called by profile_changed in InputMapper)
func rebuild(input_profile, is_customizable=false, id=0):
	#clear moves out of actionlist
	_action_list.clear()
	
	for action_name in input_profile.keys():
		#input_profile is the full profile dict., need to call subarray [0] which is a scancode
		var line = _action_list.add_input_line(action_name, \
			input_profile[action_name][0], is_customizable, id)
		if is_customizable:
			print("Move name sending to input change button pressed ",action_name)
			line.connect('change_button_pressed', self, \
				'_on_InputLine_change_button_pressed', [action_name, line])
	
	
#choosing player calls InputMapper intialize and change_profile
func _on_PlayerSelect_item_selected(index):
	$ConfigOverlay/Column/PlayerMenu/ProfilesMenu.initialize($InputMapper, index)
	$InputMapper.change_profile($ConfigOverlay/Column/PlayerMenu/ProfilesMenu.selected, index)

#make key rebind menu active
func _on_InputLine_change_button_pressed(action_name, line):
#	set_process_input(false)
#
	#show rebind dialog
	$ConfigOverlay/KeySelectMenu.open()
	#assign variables to arguments
	var key_scancode = yield($ConfigOverlay/KeySelectMenu, "key_selected")[0]
	var key_device = yield($ConfigOverlay/KeySelectMenu, "key_selected")[1]
	var player_id = playerselect.get_selected_id()
	#pass arguments to change function
	print("Sending ",action_name," to change function.")
	print(line)
	$InputMapper.change_action_key(action_name, key_scancode,key_device,player_id)
	
	line.update_key(key_scancode)
	set_process_input(true)

#hide overlay when done
func hide_overlay():
	$ConfigOverlay.visible = false
