extends Control


# Declare member variables here. Examples:
# var a = 2
# var b = "text"
var bar : ProgressBar
var super_hint

# Called when the node enters the scene tree for the first time.
func _ready():
	bar = $ProgressBar
	super_hint = $SuperHint

func set_meter(value : int):
	bar.value = value
	


func _on_ProgressBar_changed():
	var value = bar.value
	super_hint.visible = (value > 50)
	if value < 50:
		bar.modulate = Color(0, 255, 255, 255)
	elif value < 100:
		bar.modulate = Color(255, 0, 0, 255)
	else:
		bar.modulate = Color(255, 255, 0, 255)
