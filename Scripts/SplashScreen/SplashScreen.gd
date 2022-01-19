extends Node2D

# skip splash
func _input(event):
	if(event is InputEventKey):
		go_title_screen()
		
	if(event is InputEventJoypadButton):
		go_title_screen()

func go_title_screen():
	get_tree().change_scene("res://Scenes/Lobby.tscn")

#when animation finishes go to title
func _on_AnimationPlayer_animation_finished(_anim_name):
	go_title_screen()
