extends Node2D

onready var OL_logo = $OL
onready var GL_logo = $GL
onready var SL_logo = $SL
onready var animation_player = $AnimationPlayer

# Declare member variables here. Examples:
# var a = 2
# var b = "text"


# Called when the node enters the scene tree for the first time.
func _ready():
	selected_char_logo(0)
	pass # Replace with function body.

func selected_char_logo(char_selected: int):
	print("Char selected" + str(char_selected))
	var i = 0
	for child in get_children():
		if (child is Sprite):
			child.visible = (char_selected == i)
		i = i + 1
	if char_selected == 0:
		animation_player.play("OL")
	if char_selected == 1:
		animation_player.play("GL")
	
		
