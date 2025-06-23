extends CanvasLayer

# Declare member variables here. Examples:
# var a = 2
# var b = "text"

	
	
func set_text(txt):
	$Label.text = txt
	get_tree().paused = true


func _process(delta):
	for inp in ["p", "k", "s"]:
		if Input.is_action_pressed(inp):
			get_tree().paused = false
			queue_free()
