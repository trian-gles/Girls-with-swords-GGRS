extends Node2D

func _ready():
	center_game_window()

# skip splash
func _input(event):
	if(event is InputEventKey):
		go_title_screen()
		
	if(event is InputEventJoypadButton):
		go_title_screen()

func go_title_screen():
	get_tree().change_scene("res://Scenes/Lobby/LobbyRedesign.tscn")

#when animation finishes go to title
func _on_AnimationPlayer_animation_finished(_anim_name):
	go_title_screen()

func center_game_window() -> void:
	var windowSize = OS.get_window_size()
	var screenSize = OS.get_screen_size()
	var newPosition = (screenSize - windowSize) / 2
	OS.set_window_position(newPosition)
