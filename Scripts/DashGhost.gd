extends Sprite


func _ready():
	$Tween.interpolate_property(self, "modulate:a", 1.0, 0.0, 2, 3, 1)
	$Tween.start()
	
	

func _on_Tween_tween_completed(object, key):
	queue_free()
	
