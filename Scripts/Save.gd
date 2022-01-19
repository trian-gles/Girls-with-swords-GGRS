extends Node

onready var OpponentPort =$OpponentPort
onready var OpponentIp = $OpponentIp
onready var LocalPort = $LocalPort


const SAVE_DIR = "user://saves/"
var save_path = "user://save.dat"

func _ready():
	pass

func _on_AddFriend_button_down():
	var data = {
		"Friend Name" : "Your first friend",
		"Opponent Port" : "deez",
		"Opponent Ip" : "nutz",
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
	
	console_write("Friend added","have","fun!")

func _on_LoadFriend_button_down():
	var file = File.new()
	if file.file_exists(save_path):
		var error = file.open(save_path, File.READ)
		if error == OK:
			var player_data = file.get_var()
			file.close()
			console_write(player_data["Opponent Port"],player_data["Opponent Ip"],player_data["Local Port"])

func console_write(port,ip,homeport):
	OpponentPort.clear()
	OpponentIp.clear()
	LocalPort.clear()
	OpponentPort.text += str(port) 
	OpponentIp.text += str(ip)
	LocalPort.text += str(homeport)
	
