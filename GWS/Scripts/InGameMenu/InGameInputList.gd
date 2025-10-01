extends VBoxContainer


export var player = 1


var button_names_both = [[
	"p", "k", "s", "a", "c"
],[
	"pb", "kb", "sb", "ab", "cb"
]]

var button_frames = ["", "p", "k", "s", "c", "a", "b"]


# Called when the node enters the scene tree for the first time.
func _ready():
	var children = get_children()
	var config_file = File.new()
	var ControllerConfigValues
	if config_file.open("user://ControllerConfig.json", File.READ)== OK:
		var config_json = JSON.parse(config_file.get_as_text())
		config_file.close()
		ControllerConfigValues = config_json.result
	else:
		print("Error loading Config, not found")
		
	var profile = int(ControllerConfigValues["P%s" % player])
	var inputs
	if profile == 1:
		inputs = ControllerConfigValues["P%sCustomButtons" % player]
	else:
		inputs = ControllerConfigValues["P%sCustomKeys" % player]
	
	var button_names = button_names_both[player - 1]
	for i in range(len(children)):
		var button = button_names[i]
		children[i].set_inputs(button_frames.find(button[0]), inputs[button][0])
