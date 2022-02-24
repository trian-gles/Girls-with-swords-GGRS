extends Control

var cursor: Sprite

func _ready():
	cursor = get_node("CanvasLayer/Cursor")
	cursor.connect("CharacterSelected", self, "_close_screen")
	
func _close_screen():
	queue_free()
	
func save_state() -> Dictionary:
	var state_dict = {}
	# state_dict["p1_cursor_pos"] = cursor.p1_cursor
	# state_dict["p2_cursor_pos"] = cursor.p2_cursor
	# state_dict["p1_selected"] = cursor.p1_selected
	# state_dict["p2_selected"] = cursor.p2_selected
	return state_dict
	
func load_state(buf: PoolByteArray):
	var state_dict: Dictionary = bytes2var(buf) 
	
	# cursor.p1_cursor = state_dict["p1_cursor_pos"]
	# ...
	
func advance_frame(p1inputs: int, p2inputs: int):
	var all_inputs = []
	var player_num = 1
	for num in [p1inputs, p2inputs]:
		var prefix = "p" + str(player_num) + "_"
		if num & 1:
			all_inputs.append(prefix + "up")
		if num & 2:
			all_inputs.append(prefix + "down")
		if num & 4:
			all_inputs.append(prefix + "left")
		if num & 8:
			all_inputs.append(prefix + "right")
		if num & 16:
			all_inputs.append(prefix + "p")
		if num & 32:
			all_inputs.append(prefix + "k")
		if num & 64:
			all_inputs.append(prefix + "s")
		player_num += 1
	
	# $CanvasLaver/Cursor.handle_inputs(all_inputs)

