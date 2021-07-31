extends Camera2D

export(float, 0, 0.5) var zoom_offset : float = 0.2

export var debug_mode : bool = true

var camera_rect = Rect2()
var viewport_rect = Rect2()

func _ready():
	viewport_rect = get_viewport_rect()
	print(viewport_rect)
	
func adjust(p1_pos : Vector2, p2_pos : Vector2):
	camera_rect = Rect2(p1_pos, Vector2())
	camera_rect = camera_rect.expand(p2_pos)
	offset = calculate_center(camera_rect)
	zoom = calculate_zoom(camera_rect, viewport_rect.size)
	var y_height = 270 * zoom.y
	var y_bottom = y_height / 2 + offset.y
	var y_below = 260 - y_bottom
	if y_below < 0:
		offset.y += y_below
		
	var x_width = 480 * zoom.x
	var x_below = offset.x - x_width / 2
	if x_below < 0:
		offset.x -= x_below
		
	var x_right = x_width / 2 + offset.x
	var x_above = 480 - x_right
	if x_above < 0:
		offset.x += x_above

func calculate_center(rect: Rect2) -> Vector2:
	return Vector2(
		rect.position.x + rect.size.x / 2,
		rect.position.y + rect.size.y / 2
	)

func calculate_zoom(rect: Rect2, viewport_size: Vector2) -> Vector2:
	var max_zoom = max(
		max(0.6, rect.size.x / viewport_size.x + zoom_offset),
		max(0.6, rect.size.y / viewport_size.y + zoom_offset)
	)
	return Vector2(max_zoom, max_zoom)
	
