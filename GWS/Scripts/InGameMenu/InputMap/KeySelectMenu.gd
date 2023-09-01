extends Panel

#this signal is recieved in InputMenu.gd
signal key_selected(scancode)

func _ready():
	set_process_input(false)

#process inputs and pass them via signal
func _input(event):
	if not event.is_pressed():
		return
	if event is InputEventJoypadButton:
		emit_signal("key_selected", event.button_index, event.device)
	elif event is InputEventKey:
		print(event.device)
		emit_signal("key_selected", event.scancode, event.device)

	close()

#for showing and hiding rebind menu
func open():
	show()
	set_process_input(true)

func close():
	hide()
	set_process_input(false)
	

