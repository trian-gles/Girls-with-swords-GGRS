extends ReferenceRect

export var default_color: Color
export var finish_color: Color
export var curr_color: Color
export var fail_color: Color
# Declare member variables here. Examples:
# var a = 2
# var b = "text"


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

func create(input1, input2, description):
	$Input1.frame = input1
	$Input2.frame = input2
	$Description.text = description
	

