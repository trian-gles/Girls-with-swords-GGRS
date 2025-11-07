tool
extends Node2D



export var character = 0 setget set_character

var names = [
	"OFFICE LADY",
	"GASLIGHT",
	"SNAIL LADY",
	"HAT LADY"
]

var playstyles = [
	"Versatile",
	"Long Range",
	"Control",
	"Pixie"
]

var difficulty = [
	"*",
	"*",
	"**",
	"***"
]

var facts = [
	["Occupation:\nExecutive Assistant", 
	"Hometown:\nTokyo, Japan", 
	"Relationship Status:\nSingle", 
	"Fav Season of Friends:\n7"],
	
	["Occupation:\nr/romancebooks Mod", 
	"Hometown:\nBerlin, Germany", 
	"Hobby:\nImprov Theater", 
	"Attachment Style:\nAnxious"],
	
	["Occupation:\nInfluencer", 
	"Hometown:\nLos Angeles, USA", 
	"Charitable cause:\ngastropod sufferage", 
	"Marathon PB:\n2 weeks"],
	
	["Occupation:\nEbay reseller", 
	"Hometown:\nUnder your bed", 
	"Fav Stephen King Novel:\nIlliterate", 
	"Celebrity Lookalike:\nOL"],
]
func _ready():
	do_set_character(character)

# Called when the node enters the scene tree for the first time.
func set_character(c):
	if Engine.is_editor_hint():
		do_set_character(c)
	else:
		character = c

func do_set_character(c):
	character = c
	$Difficulty.text = "Difficulty: " + difficulty[c]
	$Style.text = "Style: " + playstyles[c]
	$Name.fullText = names[c]
	var fact_arr = $Facts.get_children()
	for i in range(len(fact_arr)):
		fact_arr[i].text = facts[c][i]
		
	for child in $Portraits.get_children():
		child.visible = false
	$Portraits.get_children()[c].visible = true
	
	for child in $SelectedChar.get_children():
		if "visible" in child:
			child.visible = false
	$SelectedChar.get_children()[c].visible = true
		


# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
