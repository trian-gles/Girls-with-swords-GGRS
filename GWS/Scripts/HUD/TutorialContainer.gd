extends VBoxContainer

var total_goals = 0
onready var goals = get_children()
# Called when the node enters the scene tree for the first time.

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

func _ready():
	for goal in goals:
		goal.visible = false

func add_goal(text, input1 := "", input2:= "", input3:= "", input4:= ""):
	goals[total_goals].create(text, input_translation[input1], input_translation[input2], input_translation[input3], input_translation[input4])
	goals[total_goals].visible = true
	goals[total_goals].visible
	total_goals += 1
	
func success_goal(ptr):
	goals[ptr].finish()
	
func curr_goal(ptr):
	goals[ptr].current()
	
func fail_goal(ptr):
	goals[ptr].fail()
	$Swingin.text = "CHALLENGE FAILED! Press `reset` to restart \n`playback` for a demo"
	$Swingin.visible = true
	
func success_all():
	$Swingin.text = "SWINGIN! Press `reset` to continue"
	$Swingin.visible = true
	
func playback_finished():
	$Swingin.text = "Press `reset` to give it a try"
	$Swingin.visible = true
	
	
func finish():
	$Swingin.text = "SWINGIN! Press `reset` to exit"
	$Complete.visible = true
	
	
func reset():
	total_goals = 0
	$Swingin.visible = false
	$Complete.visible = false
	for goal in goals:
		if goal == $Swingin: continue
		
		if goal == $Complete: continue
		goal.visible = false
		goal.reset();
		
