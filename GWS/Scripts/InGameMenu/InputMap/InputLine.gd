extends HBoxContainer
onready var scene_tree: = get_tree()
signal change_button_pressed

var device_id = -1
var buttonicon
var buttoniconalt
onready var change_button = $ChangeButton

func _ready():
	pass

func _joy_connection_changed(new_device_id:int, connected:bool):
	if connected:
		device_id = Input.get_connected_joypads()[0] 
		device_id = new_device_id
		if "XInput" in Input.get_joy_name(device_id):
			buttonicon.visible = true
			buttoniconalt.visible = false
		else:
			buttonicon.visible = false
			buttoniconalt.visible = true
			

func initialize(action_name, key, can_change, keyboard_profile:bool,player_id:int):
	buttonicon = $Buttons/Button1
	buttoniconalt = $Buttons/Button2
	buttonicon.visible = true
	
	if Input.get_connected_joypads().size() > 0:
		if (Input.get_connected_joypads().size() > 1 and player_id == 1):
			device_id = Input.get_connected_joypads()[1] 
		else:
			device_id = Input.get_connected_joypads()[0] 
		if "XInput" in Input.get_joy_name(device_id):
			buttonicon.visible = true
			buttoniconalt.visible = false
		elif Input.get_joy_name(device_id) != "XInput":
			buttonicon.visible = false
			buttoniconalt.visible = true
	#change action names to move names
	if action_name == "p" or action_name == "pb":
		action_name = "Punch"
	if action_name == "k" or action_name == "kb":
		action_name = "Kick"
	if action_name == "s" or action_name == "sb":
		action_name = "Slash"
		
	if action_name == "a" or action_name == "ab":
		action_name = "Special"
	if action_name == "b" or action_name == "bb":
		action_name = "String"
		
	if action_name == "c" or action_name == "cb":
		action_name = "Dash"
		
	$Action.text = action_name.capitalize()
#	print(key)
	if keyboard_profile:
#		$Key.text = OS.get_scancode_string(key)
		buttonicon.frame = key
		buttoniconalt.frame = key
	else:
#		$Key.text = Input.get_joy_button_string(key)
		buttonicon.frame = key
		buttoniconalt.frame = key
	
	$ChangeButton.disabled = not can_change
	
# warning-ignore:return_value_discarded
	Input.connect("joy_connection_changed", self, "_joy_connection_changed")

func update_key(scancode):
#	$Key.text = OS.get_scancode_string(scancode)
#	$Key.text = Input.get_joy_button_string(scancode)
	buttonicon.frame = scancode
	buttoniconalt.frame = scancode
	
	scene_tree.set_input_as_handled()

func _on_ChangeButton_pressed():
	emit_signal('change_button_pressed')

