tool
extends Position2D


# Declare member variables here. Examples:
# var a = 2
# var b = "text"

export var flip_h = false setget set_flip_h

		

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

func set_flip_h(new_flip):
	flip_h = new_flip
	for child in get_children():
		if "flip_h" in child:
			child.flip_h = new_flip
			for sub_child in child.get_children():
				if "flip_h" in sub_child:
					sub_child.flip_h = new_flip
				
# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
