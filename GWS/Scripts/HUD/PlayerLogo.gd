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
	if char_selected == 0:
		#OL
		OL_logo.visible = true
		GL_logo.visible = false
		SL_logo.visible = false
		#play animation 
		animation_player.play("OL")
	if char_selected == 1:
		#GL
		OL_logo.visible = false
		GL_logo.visible = true
		SL_logo.visible = false
		#play animation 
		animation_player.play("GL")
	if char_selected == 2:
		#SL
		OL_logo.visible = false
		GL_logo.visible = false
		SL_logo.visible = true
		#play animation 
#		animation_player.play("GL")
	
		
