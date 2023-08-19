extends Control


var snails_to_draw = []

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

func draw_snail(glob_x: int, color: Color):
	var coor = Vector2(glob_x / 480 * rect_size.x, 0);
	snails_to_draw.append([coor, color])

func _draw():
	for arr in snails_to_draw:
		draw_circle(arr[0], 5, arr[1])
