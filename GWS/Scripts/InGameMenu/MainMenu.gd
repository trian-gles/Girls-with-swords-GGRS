extends Button

onready var scene_tree: = get_tree()

func _ready():
# warning-ignore:return_value_discarded
	Events.connect("PausePressed", self, "focused")

func focused():
	grab_focus()
