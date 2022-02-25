extends Control

onready var animationplayer = $CanvasLayer/AnimationPlayer

func _ready():
	var cursor = get_node("CanvasLayer/Cursor")
	cursor.connect("CharacterSelected", self, "_close_screen")
	
	
func _close_screen():
	queue_free()

