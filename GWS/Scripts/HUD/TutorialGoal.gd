extends Container

export var default_color: Color
export var finish_color: Color
export var curr_color: Color
export var fail_color: Color

var colorRect: ColorRect
# Declare member variables here. Examples:
# var a = 2
# var b = "text"


# Called when the node enters the scene tree for the first time.
func _ready():
	colorRect = $ColorRect

func create(description, input1, input2, input3):
	$Input1.frame = input1
	$Input2.frame = input2
	$Input3.frame = input3
	$Description.text = description
	
func finish():
	colorRect.color = finish_color
	
func current():
	colorRect.color = curr_color
	
func fail(): 
	colorRect.color = fail_color

func reset():
	colorRect.color = default_color
	

