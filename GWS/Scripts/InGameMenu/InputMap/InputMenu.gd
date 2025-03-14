extends Control

#loading nodes
#onready var scene_tree: = get_tree()
#onready var buttoncheck_overlay: ColorRect = get_node("ConfigOverlay")

#load player select dropdown
export (NodePath) var playerselect_path
onready var playerselect = get_node(playerselect_path)
#load actionlist node inside scroll container
export (NodePath) var p1_action_list_path 
export (NodePath) var p2_action_list_path
onready var p1_action_list = get_node(p1_action_list_path)
onready var p2_action_list = get_node(p2_action_list_path)

export (NodePath) var p1_profiles_menu_path
export (NodePath) var p2_profiles_menu_path
onready var p1_profiles_menu = get_node(p1_profiles_menu_path)
onready var p2_profiles_menu = get_node(p2_profiles_menu_path)

export (NodePath) var p1_keyselect_path
export (NodePath) var p2_keyselect_path
onready var p1_keyselect = get_node(p1_keyselect_path)
onready var p2_keyselect = get_node(p2_keyselect_path)

onready var value_data
onready var ControllerConfigValues

func _ready():
	
	#SIGNALS
	#connect ReturnToInGameMenu to hide overlay
# warning-ignore:return_value_discarded
	Events.connect("BackPressed", self, "hide_overlay")
	#changing profiles triggers rebuild
# warning-ignore:return_value_discarded
	$InputMapper.connect('profile_changed', self, 'rebuild')
	
	#called once on ready
	#populate p1/p2 profile menu
	p1_profiles_menu.initialize($InputMapper, 0)
	p2_profiles_menu.initialize($InputMapper, 0)
	
	var config_file = File.new()
	if config_file.open("user://ControllerConfig.json", File.READ)== OK:
		load_JSON()
	#else:
		#change to profile_id 0
	$InputMapper.change_profile(p1_profiles_menu.selected, 0)
	$InputMapper.change_profile(p2_profiles_menu.selected, 1)
	

#main rebuild function (called by profile_changed in InputMapper)
func rebuild(input_profile, is_customizable=false, id=0, player_id=0):

	if player_id == 0:
		p1_action_list.clear()
	else:
		p2_action_list.clear()	
	
	var prev_line
	for action_name in input_profile.keys():
		#input_profile is the full profile dict., need to call subarray [0] which is a scancode
		var current_action_list
		if player_id == 0:
			current_action_list = p1_action_list
		elif player_id ==1:
			current_action_list = p2_action_list
			
		#on actionlist node
		var line = current_action_list.add_input_line(action_name, \
			input_profile[action_name][0], is_customizable, id, player_id)
		if prev_line:
			prev_line.change_button.focus_neighbour_bottom = line.change_button.get_path()
		prev_line = line
		if is_customizable:
			#connect key customize function
			line.connect('change_button_pressed', self, \
				'_on_InputLine_change_button_pressed', [action_name, line,player_id])

#make key rebind menu active
func _on_InputLine_change_button_pressed(action_name, line,player_id):
	set_process_input(false)
	
	var key_arguments
	
	#show rebind dialog
	if player_id == 1:
		p2_keyselect.open()
		key_arguments = yield(p2_keyselect, "key_selected")	
	else:
		p1_keyselect.open()
		key_arguments = yield(p1_keyselect, "key_selected")
	
	#assign variables to arguments
	var key_scancode = key_arguments[0]
	var key_device = key_arguments[1]
	
	#pass arguments to change function
	#print("Sending ",action_name," to change function.")
	
	$InputMapper.change_action_key(action_name, key_scancode,key_device,player_id)
	line.update_key(key_scancode)
	
	
	set_process_input(true)
	

#hide overlay when done
func hide_overlay():
	$ConfigOverlay.visible = false


#save controller settings
var save_path = "user://ControllerConfig.json"


func save_JSON():
	var p1id = $InputMapper.get("current_profile_id")
	var p2id = $InputMapper.get("current_2p_profile_id")
	value_data ={		
		"P1" : p1id,
		"P2" : p2id,
		"P1CustomKeys" : $InputMapper.profile_keyboard,
		"P1CustomButtons" : $InputMapper.profile_fightstick,
		"P2CustomKeys" : $InputMapper.profile_2pkeyboard,
		"P2CustomButtons" : $InputMapper.profile_2pfightstick
	}
	
	#print("Saving Controller Config",value_data)
	var file
	file = File.new()
	file.open(save_path, File.WRITE)
	file.store_line(to_json(value_data))
	file.close()
	pass

func _on_ReturnToInGameMenu_button_down():
	save_JSON()

func _on_ReturnMainMenu_button_down():
	save_JSON()


func load_JSON():
	var config_file = File.new()
	if config_file.open("user://ControllerConfig.json", File.READ)== OK:
		var config_json = JSON.parse(config_file.get_as_text())
		config_file.close()
		ControllerConfigValues = config_json.result
	else:
		print("Error loading Config, not found")
	
	#print("Changing controller profiles to config values")
	#load p1 custom buttons
	for buttons in ControllerConfigValues["P1CustomButtons"]:
		$InputMapper.profile_fightstick[buttons] = ControllerConfigValues["P1CustomButtons"][buttons]
	$InputMapper.change_profile(1,0)	
	p1_profiles_menu.selected = 1
	#load p1 custom keys
	for keys in ControllerConfigValues["P1CustomKeys"]:
		$InputMapper.profile_keyboard[keys] = ControllerConfigValues["P1CustomKeys"][keys]
	$InputMapper.change_profile(0,0)	
	p1_profiles_menu.selected = 0
	
	#load p2custom buttons
	for buttons in ControllerConfigValues["P2CustomButtons"]:
		$InputMapper.profile_2pfightstick[buttons] = ControllerConfigValues["P2CustomButtons"][buttons]
	$InputMapper.change_profile(1,1)	
	p2_profiles_menu.selected = 1
	#load p2 custom keys
	#replace var
	for keys in ControllerConfigValues["P2CustomKeys"]:
		$InputMapper.profile_2pkeyboard[keys] = ControllerConfigValues["P2CustomKeys"][keys]
	#load new var
	$InputMapper.change_profile(0,1)	
	p2_profiles_menu.selected = 0
	
	#load last picked profiles
	p1_profiles_menu.selected = int(ControllerConfigValues["P1"])
	$InputMapper.change_profile(int(ControllerConfigValues["P1"]),0)
	p2_profiles_menu.selected = int(ControllerConfigValues["P2"])
	$InputMapper.change_profile(int(ControllerConfigValues["P2"]),1)


func _on_P1Reset_pressed():
	var keyboard_init = {
	'p': [KEY_Z,0],
	'k': [KEY_X,0],
	's': [KEY_C,0],
	'a': [KEY_A,0],
	'b': [KEY_S,0],
	'c': [KEY_D,0]
	}
	var fightstick_init = {
		'p': [JOY_SONY_SQUARE,0],
		'k': [JOY_SONY_TRIANGLE,0],
		's': [JOY_SONY_CIRCLE,0],
		'a': [JOY_SONY_X,0],
		'b': [JOY_R,0],
		'c': [JOY_R2,0]
	}
	if p1_profiles_menu.selected == 1:
		for moves in fightstick_init:
			$InputMapper.profile_fightstick[moves] = fightstick_init[moves]
		$InputMapper.change_profile(1,0)
	else:
		for moves in keyboard_init:
			$InputMapper.profile_keyboard[moves] = keyboard_init[moves]
		$InputMapper.change_profile(0,0)
	
func _on_P2Reset_pressed():
	var keyboard2p_init = {
		'pb': [KEY_J,0],
		'kb': [KEY_K,0],
		'sb': [KEY_L,0],
		'ab': [KEY_U,0],
		'bb': [KEY_I,0],
		'cb': [KEY_O,0]
	}
	var fightstick2p_init = {
		'pb': [JOY_SONY_SQUARE,1],
		'kb': [JOY_SONY_TRIANGLE,1],
		'sb': [JOY_SONY_CIRCLE,1],
		'ab': [JOY_SONY_X,1],
		'bb': [JOY_R,1],
		'cb': [JOY_R2,1]
	}
	if p2_profiles_menu.selected == 1:
		for moves in fightstick2p_init:
			$InputMapper.profile_2pfightstick[moves] = fightstick2p_init[moves]
		$InputMapper.change_profile(1,1)
	else:
		for moves in keyboard2p_init:
			$InputMapper.profile_2pkeyboard[moves] = keyboard2p_init[moves]
		$InputMapper.change_profile(0,1)
