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
	"left": 10
}

func _ready():
	for goal in goals:
		goal.visible = false

func add_goal(text, input1 := "", input2:= "", input3:= ""):
	goals[total_goals].create(text, input_translation[input1], input_translation[input2], input_translation[input3])
	goals[total_goals].visible = true
	goals[total_goals].visible
	total_goals += 1
	
func success_goal(ptr):
	goals[ptr].finish()
	
func curr_goal(ptr):
	goals[ptr].current()
	
func fail_goal(ptr):
	goals[ptr].fail()
	
func success_all():
	$Swingin.visible = true
	
func finish():
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
		
