extends Node


# This script is required to make slash effects happen at absolute locations


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


func slash(pos: Vector2):
	$DrawFX.slash(pos)
