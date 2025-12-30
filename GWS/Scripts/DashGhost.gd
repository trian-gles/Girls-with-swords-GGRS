extends Sprite

var activate_frame = 0

func _ready():
	visible = false
	
func run(frame):
	visible = true
	$Tween.interpolate_property(self, "modulate:a", 1.0, 0.0, 2, 3, 1)
	$Tween.start()
	activate_frame = frame
	
func Rollback(frame):
	if frame < activate_frame:
		visible = false
	
func _on_Tween_tween_completed(object, key):
	visible = false
	
