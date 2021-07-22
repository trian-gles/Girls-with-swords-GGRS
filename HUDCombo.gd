extends Label

export var dissapear_time : float = 0.5

func combo(combo_num : int):
	text = "x" + String(combo_num)
	$Timer.stop()
	print(name + "Turning on combo to " + str(combo_num))

func off():
	print(name + "turning off combo")
	$Timer.wait_time = dissapear_time 
	$Timer.start()

func _on_Timer_timeout():
	text = ""
