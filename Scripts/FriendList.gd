extends OptionButton

const SAVE_DIR = "user://saves/"
var save_path = "user://save.dat"

func _ready():
	pass

func _on_Button_pressed():
	var data = {
		"Opponent Port" : "deez",
		"Oppopnent Ip" : "nutz",
		"Local Port" : "got eem!",
	}
	
	var dir = Directory.new()
	if !dir.dir_exists(SAVE_DIR):
		dir.make_dir_recursive(SAVE_DIR)
	
	var file = File.new()
	var error = file.open(save_path, File.WRITE)
	if error == OK:
		file.store_var(data)
		file.close()
	
	console_write("Friend added")

func _onLoadButton_pressed():
	var file = File.new()
	if file.file_exists(save_path):
		var error = file.open(save_path, File.READ)
		if error == OK:
			var player_data = file.get_var()
			file.close()
			
		console_write("Friends loaded")
		
func console_write(value):
	console_label.text += str(value) + "\n"
