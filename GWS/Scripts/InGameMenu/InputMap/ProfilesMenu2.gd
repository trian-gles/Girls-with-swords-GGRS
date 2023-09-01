extends OptionButton

#load inputmapper node
export (NodePath) var inputmapper_path
onready var inputmapper = get_node(inputmapper_path)

#connect profile selection to inputmapper and trigger redraw of bindings
func _ready():
	#sent via signal instead
	#	#selection triggers function in inputmapper.gd
#	self.connect("item_selected", inputmapper, "_on_ProfilesMenu_item_selected")
	pass
	
var profile_names = {
	0: "Keyboard",
	1: "Controller"
}

func initialize(input_mapper, player_id):
	#clear to avoid duplicates
	clear()
	# set to p2
	var profiles
	profiles = input_mapper.player2_profiles
	
	#loop and add names from profile list		
	for profile_index in profiles:
		var profile_name = profile_names[profile_index].capitalize()
		add_item(profile_name, profile_index)
		
#controller focuses buttons
#func _on_ConfigOverlay_visibility_changed():
#	grab_focus()
