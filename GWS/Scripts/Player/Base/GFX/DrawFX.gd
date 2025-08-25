extends Node2D

var start = Vector2(0, 0)
var end = Vector2(0, 0)

var draw_frames = 0

onready var rng = RandomNumberGenerator.new()
# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.
	
func slash(pos: Vector2):
	rng.seed = int(pos.x)
	var rotation = (rng.randf_range(0, 1) * PI)
	var mod = Vector2(1000, 0).rotated(rotation)
	end = pos + mod
	start = pos - mod
	draw_frames = 20	

func _physics_process(delta):
	update()
	if draw_frames > 0:
		draw_frames = draw_frames - 1

func _draw():
	
	if draw_frames > 0:
		draw_line(start, end, Color.white, draw_frames / 3)
