extends Node

signal profile_changed(new_profile, is_customizable)

var current_profile_id = 0
var profiles = {
	0: 'profile_keyboard',
	1: 'profile_fightstick',
	2: 'profile_microdash',
	3: 'profile_custom'
}

var profile_keyboard = {
	'p': KEY_Z,
	'k': KEY_X,
	's': KEY_C
}
var profile_microdash = {
	'p': JOY_BUTTON_0,
	'k': JOY_BUTTON_3,
	's': JOY_BUTTON_4,
}
var profile_fightstick = {
	'p': JOY_BUTTON_2,
	'k': JOY_BUTTON_3,
	's': JOY_BUTTON_5,
}
var profile_custom = profile_keyboard

func change_profile(id):
	current_profile_id = id
	var profile = get(profiles[id])
	var is_customizable = true if id == 3 else false
	
	for action_name in profile.keys():
		change_action_key(action_name, profile[action_name])
	emit_signal('profile_changed', profile, is_customizable)
	return profile

func change_action_key(action_name, key_scancode):
	erase_action_events(action_name)

	var new_button = InputEventJoypadButton.new()
	new_button.set_button_index(key_scancode)
	InputMap.action_add_event(action_name, new_button)
	var new_key = InputEventKey.new()
	new_key.set_scancode(key_scancode)
	InputMap.action_add_event(action_name, new_key)
	get_selected_profile()[action_name] = key_scancode

func erase_action_events(action_name):
	var input_events = InputMap.get_action_list(action_name)
	for event in input_events:
		InputMap.action_erase_event(action_name, event)

func get_selected_profile():
	return get(profiles[current_profile_id])

func _on_ProfilesMenu_item_selected(ID):
	change_profile(ID)
