extends VBoxContainer


var button_names = ['switch', 'reset', 'record', 'playback']


# Called when the node enters the scene tree for the first time.
func _ready():
	config_inputs()

func config_inputs():
	var children = get_children()
	var config_file = File.new()
	var ControllerConfigValues
	if config_file.open("user://ControllerConfig.json", File.READ)== OK:
		var config_json = JSON.parse(config_file.get_as_text())
		config_file.close()
		ControllerConfigValues = config_json.result
	else:
		print("Error loading Config, not found")
		
	var profile = int(ControllerConfigValues["P1"])
	var inputs
	if profile == 1:
		inputs = ControllerConfigValues["P1CustomButtons"]
	else:
		inputs = ControllerConfigValues["P1CustomKeys"]
	print(inputs)
	for i in range(len(children)):
		var button = button_names[i]
		children[i].set_inputs(button, inputs[button][0])

