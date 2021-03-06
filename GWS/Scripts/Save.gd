extends Node

export (NodePath) var dropdown_path
onready var dropdown = get_node(dropdown_path)

export (NodePath) var confirmremove_path
onready var confirmremove = get_node(confirmremove_path)

onready var FriendName = $FriendName
onready var OpponentIp = $OpponentIp
const SAVE_DIR = "user://friends/"
var save_path = "user://friends/friendlist.txt"

func _ready():
	dropdown.connect("item_selected", self, "on_item_selected")
	confirmremove.connect("confirmed", self, "removefriend")
	checkforfiles()

func checkforfiles():
	var dir = Directory.new()
	if !dir.dir_exists(SAVE_DIR):
		dir.make_dir_recursive(SAVE_DIR)	
	updatefriendlist()
		

func updatefriendlist():
	dropdown.clear()
	var file = File.new()
	if file.file_exists(save_path):
		var error = file.open(save_path, File.READ)
		if error == OK:
			var player_data = parse_json(file.get_line())
			player_data = player_data.keys()
			for players in player_data:
				dropdown.add_item(players)
	else:
		var defaultfriend = {}
		file.open(save_path, File.WRITE)
		file.store_line(to_json(defaultfriend))
	file.close()
	
func _on_AddFriend_button_down():
	#grabbing data from linedit boxes
	var friendtext = FriendName.text
	var addedfriend = {
		"Friend Name" : FriendName.text,
		"Opponent Ip" : OpponentIp.text
	}
	var file = File.new()
	#grabbing current list from file
	file.open(save_path, File.READ)
	var friendlist = parse_json(file.get_line())
	#adding friend to current list
	friendlist[friendtext] = addedfriend
	#writing new list to file
	file.open(save_path, File.WRITE)
	file.store_line(to_json(friendlist))
	file.close()
	#update dropdown with newlist
	updatefriendlist()

#dropdown calls this function
func on_item_selected(id):
#	print("Friend selected signal emitting")
	var selectedfriend = (dropdown.get_item_text(id))
	loadselectedfriend(selectedfriend)
#finds friend key in file
func loadselectedfriend(id):
	var file = File.new()
	if file.file_exists(save_path):
		var error = file.open(save_path, File.READ)
		if error == OK:
			var player_data = parse_json(file.get_line())
#			print("This is player_data printing ", str(player_data))
#			print("This is the passed argument ", str(id))
			var loadedfriend = player_data[id]
			file.close()
			console_write(loadedfriend["Friend Name"],loadedfriend["Opponent Ip"])
#writes friend data to linedit boxes
func console_write(friendname,ip):
	FriendName.clear()
	OpponentIp.clear()
	FriendName.text += str(friendname)
	OpponentIp.text += str(ip)
	
func _on_RemoveFriend_pressed():
	#getting currently selected friend
	var selectedid = dropdown.get_selected_id()
	var selectedfriend = (dropdown.get_item_text(selectedid))
	
	#show remove confirmation dialog
	confirmremove.visible = true
	confirmremove.get_cancel().grab_focus()
	confirmremove.dialog_text = ("Do you really want to remove " + str(selectedfriend))
	
func removefriend():
	var selectedid = dropdown.get_selected_id()
	var selectedfriend = (dropdown.get_item_text(selectedid))
	
	#loading and removing friend from dict
	var file = File.new()
	file.open(save_path, File.READ)
	var friendlist = parse_json(file.get_line())
	friendlist.erase(selectedfriend)
	#update file with new dict
	var error = file.open(save_path, File.WRITE)
	if error == OK:
		file.store_line(to_json(friendlist))
		file.close()
	#reflect changes in dropdown
	updatefriendlist()
	
#register friend when only one entry
func _on_FriendList_pressed():
	if dropdown.get_selected() >= 0:
		var selectedid = dropdown.get_selected_id()
		var selectedfriend = (dropdown.get_item_text(selectedid))
		loadselectedfriend(selectedfriend)

#return focus to netplay buttons
func _on_ConfirmationDialog_hide():
	print(str("exited confirmation dialog"))
	$NetPlayButtons/Host.grab_focus()
