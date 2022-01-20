extends HBoxContainer

signal change_button_pressed

var device_id = -1

func initialize(action_name, key, can_change, keyboard_profile:bool):
	$Action.text = action_name.capitalize()
	print(key)
	if keyboard_profile:
		$Key.text = OS.get_scancode_string(key)
		$Button1.frame = key
	else:
		$Key.text = Input.get_joy_button_string(key)
		$Button1.frame = key
	
	$ChangeButton.disabled = not can_change

func update_key(scancode):
	$Key.text = OS.get_scancode_string(scancode)
	$Key.text = Input.get_joy_button_string(scancode)
	$Button1.frame = scancode

func _on_ChangeButton_pressed():
	emit_signal('change_button_pressed')
