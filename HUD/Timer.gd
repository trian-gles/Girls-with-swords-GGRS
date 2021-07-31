extends Label


onready var timer = $Timer

func _ready():
	timer.start()
	
func reset():
	timer.wait_time = 99
	timer.start()

func _process(_delta: float):
	text = String(int($Timer.time_left))
