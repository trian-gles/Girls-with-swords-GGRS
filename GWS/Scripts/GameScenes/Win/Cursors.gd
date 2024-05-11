extends Control


onready var cursors = get_children()


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


func SetCursor(index):
	for i in cursors.size():
		cursors[i].visible = false
		if i == index:
			cursors[i].visible = true
