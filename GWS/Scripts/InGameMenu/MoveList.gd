extends Control
export var player_num : int = 0


# Declare member variables here. Examples:
# var a = 2
# var b = "text"


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


func fill_moves():
	var moves
	var globals = get_node("/root/Globals")
	if player_num == 0:
		moves = globals.get("P1CharacterMoves")
	else:
		moves = globals.get("P2CharacterMoves")

	var children = $Container.get_children()
	for child in children:
		child.visible = false
		
	if (moves == null):
		return;
	
	for i in range(len(moves)):
		children[i].create(moves[i])
		
