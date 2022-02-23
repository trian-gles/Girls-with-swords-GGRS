extends Node

#loading player select node
export (NodePath) var playerselect_path
onready var playerselect = get_node(playerselect_path)

#sent up to InputMenu
signal profile_changed(new_profile, is_customizable, id)

#init 1p & 2p profiles
var current_profile_id = 0
var current_2p_profile_id = 0

#p1 dictionaries
var profile_keyboard = {
	'p': [KEY_Z,0],
	'k': [KEY_X,0],
	's': [KEY_C,0]
}
var profile_microdash = {
	'p': [JOY_BUTTON_0,0],
	'k': [JOY_BUTTON_3,0],
	's': [JOY_BUTTON_4,0]
}
var profile_fightstick = {
	'p': [JOY_BUTTON_2,0],
	'k': [JOY_BUTTON_3,0],
	's': [JOY_BUTTON_5,0]
}
#p2 dictionaries
var profile_2pkeyboard = {
	'pb': [KEY_J,0],
	'kb': [KEY_K,0],
	'sb': [KEY_L,0]
}
var profile_2pmicrodash = {
	'pb': [JOY_BUTTON_0,1],
	'kb': [JOY_BUTTON_3,1],
	'sb': [JOY_BUTTON_4,1]
}
var profile_2pfightstick = {
	'pb': [JOY_BUTTON_2,1],
	'kb': [JOY_BUTTON_3,1],
	'sb': [JOY_BUTTON_5,1]
}
#init custom profiles
var profile_custom = profile_fightstick
var profile_2pcustom = profile_2pfightstick
#p1 profile list
var player1_profiles = {
	0: profile_keyboard,
	1: profile_fightstick,
	2: profile_microdash,
	3: profile_custom
}
#p2 profile list
var player2_profiles = {
	0: profile_2pkeyboard,
	1: profile_2pfightstick,
	2: profile_2pmicrodash,
	3: profile_2pcustom
}

#triggered by ProfilesMenu, where ID is chosen profile
func _on_ProfilesMenu_item_selected(ID):
	#get whether p1 or p2
	var player_id = playerselect.get_selected_id()
	#pass arguments to change function
	change_profile(ID, player_id)
	
#main function that carries out profile change
func change_profile(profile_id, player_id):
#set variables
	#check which player, then retrieve profile
	var profile
	if player_id == 0:
		current_profile_id = profile_id
		profile = player1_profiles[profile_id]
	else:
		current_2p_profile_id = profile_id
		profile = player2_profiles[profile_id]
	
	#check if remappable custom profile
	var is_customizable = true if profile_id == 3 else false
	
#loop and add set profiles bindings
	for action_name in profile.keys():
		change_action_key(action_name, profile[action_name][0], profile[action_name][1], player_id)
	#send signal up to InputMenu
	emit_signal('profile_changed', profile, is_customizable, profile_id)
	return profile

#called by change_profile above
func change_action_key(action_name, key_scancode, device_id, player_id):
	erase_action_events(action_name)
	
	var new_button = InputEventJoypadButton.new()
	new_button.set_button_index(key_scancode)
	new_button.device = device_id
	InputMap.action_add_event(action_name, new_button)
	
	var new_key = InputEventKey.new()
	new_key.set_scancode(key_scancode)
	InputMap.action_add_event(action_name, new_key)
	
	get_selected_profile(player_id)[action_name] = key_scancode

#clears old action events called by change_action_key above
func erase_action_events(action_name):
	var input_events = InputMap.get_action_list(action_name)
	for event in input_events:
		InputMap.action_erase_event(action_name, event)
#called by change_action_key above
func get_selected_profile(player_id):
	if player_id == 0:
		return player1_profiles[current_profile_id]
	else: 
		return player2_profiles[current_2p_profile_id]


