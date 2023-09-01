extends Control

const InputLine = preload("res://Scenes/InputMap/InputLine.tscn")

func clear():
	for child in get_children():
		child.free()

#where key is the scancode held in subarray [0] of profile dict.
func add_input_line(action_name, key, is_customizable=false, keyboard_profile=true, player_id=0):
	var inputline = InputLine.instance()
	inputline.initialize(action_name, key, is_customizable, keyboard_profile,player_id)
	add_child(inputline)
	return inputline
