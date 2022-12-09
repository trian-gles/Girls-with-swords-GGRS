extends Label


var last_display = 0
export var dissapear_time : float = 1

# Called when the node enters the scene tree for the first time.
func _ready():
	visible = false

func display(frame):
	last_display = frame
	visible = true
	$Timer.wait_time = dissapear_time 
	$Timer.start()
	
func rollback(frame):
	if visible and frame < last_display:
		visible = false


func _on_Timer_timeout():
	visible = false
