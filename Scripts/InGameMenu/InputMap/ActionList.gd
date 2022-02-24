extends Control

const InputLine = preload("res://Scenes/InputMap/InputLine.tscn")

func clear():
	for child in get_children():
		child.free()

#where key is the scancode held in subarray [0] of profile dict.
func add_input_line(action_name, key, is_customizable=false, keyboard_profile=true):
	var line = InputLine.instance()
	line.initialize(action_name, key, is_customizable, keyboard_profile)
	add_child(line)
	return line
