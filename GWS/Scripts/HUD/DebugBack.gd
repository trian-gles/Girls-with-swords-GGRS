extends ColorRect


# Declare member variables here. Examples:
# var a = 2
# var b = "text"


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


func set_text(txt):
	$DebugTextLabel.text = txt
	set_size($DebugTextLabel.get_minimum_size())
