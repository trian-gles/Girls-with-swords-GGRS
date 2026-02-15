extends Node2D

var background = preload("res://Scenes/Backgrounds/Office.tscn")

var background_scenes = [
	preload("res://Scenes/Backgrounds/Office.tscn"),
	preload("res://Scenes/Backgrounds/Cathedral.tscn"),
	preload("res://Scenes/Backgrounds/Forest.tscn"),
	preload("res://Scenes/Backgrounds/Clocktower.tscn")
]

var background_nodes = []

var bkg_index = 0
var configured = false

func _ready():
	for i in range(len(background_scenes)):
		background_nodes.append(background_scenes[i].instance())
	
func set_bkg(index: int):
	configured = true
	add_child(background_nodes[index])
	bkg_index = index
	
func quit():
	if configured:
		remove_child(background_nodes[bkg_index])
