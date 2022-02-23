extends HBoxContainer
onready var scene_tree: = get_tree()
signal change_button_pressed

var device_id = -1
onready var buttonicon = $Button1
onready var buttoniconalt = $Button2


func _ready():
	Input.connect("joy_connection_changed", self, "_joy_connection_changed")
#	if Input.get_connected_joypads().size() > 0:
#		self.device_id = Input.get_connected_joypads()[0]

func _joy_connection_changed(device_id:int, connected:bool):
	if connected:
		self.device_id = Input.get_connected_joypads()[0] 
		self.device_id = device_id
		if "XInput" in Input.get_joy_name(device_id):
			buttonicon.visible = true
			buttoniconalt.visible = false
		else:
			buttonicon.visible = false
			buttoniconalt.visible = true
			

func initialize(action_name, key, can_change, keyboard_profile:bool):
	var buttonicon = $Button1
	var buttoniconalt = $Button2
	buttonicon.visible = true
	
	if Input.get_connected_joypads().size() > 0:
		self.device_id = Input.get_connected_joypads()[0] 
		self.device_id = device_id
		if "XInput" in Input.get_joy_name(device_id):
			buttonicon.visible = true
			buttoniconalt.visible = false
		elif Input.get_joy_name(device_id) != "XInput":
			buttonicon.visible = false
			buttoniconalt.visible = true
			
	$Action.text = action_name.capitalize()
#	print(key)
	if keyboard_profile:
		$Key.text = OS.get_scancode_string(key)
		buttonicon.frame = key
		buttoniconalt.frame = key
	else:
		$Key.text = Input.get_joy_button_string(key)
		buttonicon.frame = key
		buttoniconalt.frame = key
	
	$ChangeButton.disabled = not can_change

func update_key(scancode):
	$Key.text = OS.get_scancode_string(scancode)
	$Key.text = Input.get_joy_button_string(scancode)
	buttonicon.frame = scancode
	buttoniconalt.frame = scancode
	scene_tree.set_input_as_handled()


func _on_ChangeButton_pressed():
	emit_signal('change_button_pressed')

