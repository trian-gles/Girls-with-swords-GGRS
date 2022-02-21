extends Sprite
#object array
var characters = []

#integer
var currentSelected = 0
var currentColumnSpot = 0
var currentRowSpot = 0

#exports
export (Texture) var player1Text
export (Texture) var player2Text
export (int) var amountOfRows = 1
export (Vector2) var portraitOffset

#objects
onready var gridContainer = get_parent().get_node("GridContainer")

func _ready():
	for nameOfCharacter in get_tree().get_nodes_in_group("Characters"):
		characters.append(nameOfCharacter)
	print(characters)
	texture = player1Text
	
func _process(delta):
	if(Input.is_action_just_pressed("ui_right")):
		currentSelected += 1
		currentColumnSpot += 1
		
		if(currentColumnSpot > gridContainer.columns -1 && currentSelected < characters.size() - 1):
			position.x -= (currentColumnSpot - 1) * portraitOffset.x
			position.y += portraitOffset.y
			
			currentColumnSpot = 0
			currentRowSpot += 1	
#		elif(currentColumnSpot > gridContainer.columns - 1 && currentSelected > characters.size() - 1):
#			position.x -= (currentColumnSpot - 1) * portraitOffset.x
#			position.y -=
#
		else:
			position.x += portraitOffset.x
		
	elif(Input.is_action_just_pressed("ui_left")):
		currentSelected -= 1
		currentColumnSpot -= 1
		
		if(currentColumnSpot < 0):
			position.x += (gridContainer.columns - 1) * portraitOffset.x
			position.y -= (amountOfRows) * portraitOffset.y
			
			currentColumnSpot = gridContainer.columns -1
			currentRowSpot -= 1	
		else:
			position.x -= portraitOffset.x
		
