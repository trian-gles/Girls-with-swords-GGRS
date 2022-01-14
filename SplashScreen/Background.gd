extends Node2D


# Declare member variables here. Examples:
# var a = 2
# var b = "text"


# Called when the node enters the scene tree for the first time.
func _ready():
	
	_draw()
	
	pass # Replace with function body.
func _draw():
	var Col = Color(1,0,0)
	var rect = Rect2(Vector2(200,200),Vector2(200,200))
	draw_rect(rect,Col)
# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
