extends CPUParticles2D

onready var hit = $ShieldHit
var rightPos = 18
var leftPos = -18
var crouchPos = 2
var standPos = -10
var crouching = false

# Declare member variables here. Examples:
# var a = 2
# var b = "text"


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

func _physics_process(delta):
	if crouching:
		position.y = crouchPos
	else:
		position.y = standPos
	
	if owner.facingRight:
		position.x = rightPos
		hit.direction.x = -1
	else:
		position.x = leftPos
		hit.direction.x = 1
# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
