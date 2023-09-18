extends Camera2D


export var debug_mode : bool = true

var camera_rect = Rect2()
var viewport_rect = Rect2()

func _ready():
	viewport_rect = get_viewport_rect()
	
func adjust(p1_pos : Vector2, p2_pos : Vector2):
	camera_rect = Rect2(p1_pos, Vector2())
	camera_rect = camera_rect.expand(p2_pos)
	
	var desired_zoom = calculate_zoom(camera_rect, viewport_rect.size)
	var desired_offset = calculate_center(camera_rect, desired_zoom)
	
	offset = desired_offset#offset.linear_interpolate(desired_offset, 0.5)
	zoom = desired_zoom#zoom.linear_interpolate(desired_zoom, 0.5)
	
	
	
	var y_height = 270 * zoom.y
	var y_bottom = y_height / 2 + offset.y
	var y_below = 260 - y_bottom
	var y_above = offset.y - y_height / 2
	
	if y_above < 0:
		offset.y -= y_above
	
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

func calculate_center(rect: Rect2, desired_zoom: Vector2) -> Vector2:
	var center = Vector2(
		rect.position.x + rect.size.x / 2,
		rect.position.y + rect.size.y / 2
	)
	
	return center

func calculate_zoom(rect: Rect2, viewport_size: Vector2) -> Vector2:
	var max_zoom = max(
		max(0.6, rect.size.x / viewport_size.x + 0.06),
		max(0.6, rect.size.y / viewport_size.y)
	)
	if max_zoom > 1:
		max_zoom = 1
	
	return Vector2(max_zoom, max_zoom)
