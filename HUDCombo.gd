extends Label

export var dissapear_time : float = 0.5

func combo(combo_num : int):
	text = "x" + String(combo_num)
	$Timer.stop()

func off():
	$Timer.wait_time = dissapear_time 
	$Timer.start()

func _on_Timer_timeout():
	text = ""
