extends VBoxContainer


export var player = 0

var p1_controls = [
	[1, 1],
	[2, 2],
	[3, 3],
	[4, 4],
	[5, 5],
]

var p2_controls = [
	[1, 1],
	[2, 2],
	[3, 3],
	[4, 4],
	[5, 5],
]


# Called when the node enters the scene tree for the first time.
func _ready():
	var children = get_children()
	for i in range(len(children)):
		children[i].set_inputs(p1_controls[i][0], p1_controls[i][1])
