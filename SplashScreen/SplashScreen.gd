extends Node2D


# Declare member variables here. Examples:
# var a = 2
# var b = "text"


# Called when the node enters the scene tree for the first time.
func _input(event):
	if(event is InputEventKey):
		go_title_screen()


# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
func go_title_screen():
	get_tree().change_scene("res://Lobby/Lobby.tscn")

func _on_AnimationPlayer_animation_finished(anim_name):
	go_title_screen()
	pass # Replace with function body.
