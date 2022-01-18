extends HBoxContainer

signal change_button_pressed

func initialize(action_name, key, can_change):
	$Action.text = action_name.capitalize()
	#if key is InputEventJoypadButton:
	$Key.text = OS.get_scancode_string(key)
	#elif key is InputEventKey:
	$Key.text = Input.get_joy_button_string(key)
	$ChangeButton.disabled = not can_change

func update_key(scancode):
	$Key.text = OS.get_scancode_string(scancode)
	$Key.text = Input.get_joy_button_string(scancode)

func _on_ChangeButton_pressed():
	emit_signal('change_button_pressed')
