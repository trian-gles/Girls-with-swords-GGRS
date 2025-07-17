extends CanvasLayer

var held_keys = []

func _ready():
	for inp in ["p", "k", "s"]:
		if Input.is_action_pressed(inp):
			held_keys.append(inp)
	
func set_text(txt):
	$Label.text = txt
	get_tree().paused = true


func _process(delta):
	for inp in ["p", "k", "s"]:
		if Input.is_action_just_pressed(inp) and not inp in held_keys:
			get_tree().paused = false
			queue_free()
		if Input.is_action_just_released(inp):
			held_keys.erase(inp)
