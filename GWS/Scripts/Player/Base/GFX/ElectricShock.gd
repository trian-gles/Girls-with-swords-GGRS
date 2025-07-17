extends Node2D


# Declare member variables here. Examples:
# var a = 2
var point_count = 30
var radius = 16
var random_radius = 6
var points = PoolVector2Array()


# Called when the node enters the scene tree for the first time.
func _ready():
	gen_points()
	
func gen_points():
	points = PoolVector2Array()
	for i in range(point_count):
		var angle = 2 * PI * i / point_count
		var rand_angle = randf() * 2 * PI
		var rand_rad = randf() * random_radius
		var height = sin(angle) * radius + sin(rand_angle) * rand_rad
		var width = cos(angle) * radius + cos(rand_angle) * rand_rad
		points.append(Vector2(height, width))
	points.append(points[0])
	update()


func _draw():
	draw_polyline(points, Color.white, 1.1, false)


func _on_Timer_timeout():
	gen_points()
