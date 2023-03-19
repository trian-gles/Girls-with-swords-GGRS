extends Control


# Declare member variables here. Examples:
# var a = 2
# var b = "text"
var bar : ProgressBar

# Called when the node enters the scene tree for the first time.
func _ready():
	bar = $ProgressBar

func set_meter(value : int):
	bar.value = value
	if value < 50:
		bar.modulate = Color(0, 255, 255, 255)
	elif value < 100:
		bar.modulate = Color(255, 0, 0, 255)
	else:
		bar.modulate = Color(255, 255, 0, 255)
