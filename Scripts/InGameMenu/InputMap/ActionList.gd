extends Control

const InputLine = preload("res://Scenes/InputMap/InputLine.tscn")

func clear():
	for child in get_children():
		child.free()

func add_input_line(action_name, key, is_customizable=false):
	var line = InputLine.instance()
	line.initialize(action_name, key, is_customizable)
	add_child(line)
	return line
