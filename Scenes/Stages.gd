extends Node2D


var desired_pos = -145;


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

func _physics_process(delta):
	if position.y < desired_pos:
		position.y += 5

func level_up():
	# Prevents this from happening multiple times during rollbacks
	if position.y >= desired_pos:
		desired_pos += 145 * 2
