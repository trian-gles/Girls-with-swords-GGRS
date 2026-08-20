extends Control


var input_translation = {
	"": 0,
	"p": 1,
	"k": 2,
	"s": 3,
	"dash": 4,
	"string": 6,
	"special": 5,
	"up": 7,
	"right": 8,
	"down": 9,
	"left": 10,
	"air": 11,
	"hold": 12,
	"wait": 13,
	"downright": 14,
	"upright" : 15,
	"upleft" : 16,
	"downleft" : 17,
	"qcf" : 18,
	"qcb" : 19,
	"dp" : 20
}

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


func create(move):
	visible = true
	$Input1.frame = input_translation[move[0]]
	$Input2.frame = input_translation[move[1]]
	$Input3.frame = input_translation[move[2]]
	$VBoxContainer/Description.text = move[4]
