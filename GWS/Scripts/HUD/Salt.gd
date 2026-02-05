extends Control


# Declare member variables here. Examples:
# var a = 2
# var b = "text"
var textProgress
var textRect
var textRectAnim

# Called when the node enters the scene tree for the first time.
func _ready():
	textProgress = $TextureProgress
	textRect = $TextureRect
	textRectAnim = $TextureRectAnim

func _on_TextureProgress_value_changed(value):
	textProgress.visible = (value != 100)
	textRect.visible = (value != 100)
	textRectAnim.visible = (value == 100)
	
