extends Control


# Declare member variables here. Examples:
# var a = 2
# var b = "text"


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


func set_level(level):
	$TextureProgress.value = level
	$TextureProgress.visible = (level != 100)
	$TextureRect.visible = (level != 100)
	$TextureRectAnim.visible = (level == 100)
