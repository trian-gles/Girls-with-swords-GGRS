extends Control


# Declare member variables here. Examples:
# var a = 2
# var b = "text"


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


func set_inputs(name, button_frame):
	$InGameInput.text = name
	$ControllerInput.frame = button_frame
