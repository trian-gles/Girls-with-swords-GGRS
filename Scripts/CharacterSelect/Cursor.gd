extends Sprite

var active: bool = false

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

#signals
signal CharacterSelected

func _ready():
	for nameOfCharacter in get_tree().get_nodes_in_group("Characters"):
		characters.append(nameOfCharacter)
	print(characters)
	texture = player1Text
	
func _input(event):
	if not active:
		return
	
	if(event.is_action_pressed("ui_right")):
		currentSelected += 1
		currentColumnSpot += 1
		
		if(currentColumnSpot > gridContainer.columns -1 && currentSelected < characters.size() - 1):
			position.x -= (currentColumnSpot - 1) * portraitOffset.x
			position.y += portraitOffset.y
			
			currentColumnSpot = 0
			currentRowSpot += 1	
			
		elif(currentColumnSpot > gridContainer.columns - 1 && currentSelected > characters.size() - 1):
			position.x -= (currentColumnSpot - 1) * portraitOffset.x
			position.y -= currentRowSpot * portraitOffset.y
			
			currentColumnSpot = 0
			currentRowSpot = 0
			currentSelected = 0

		else:
			position.x += portraitOffset.x
		
	elif(event.is_action_pressed("ui_left")):
		currentSelected -= 1
		currentColumnSpot -= 1
		
		if(currentColumnSpot < 0 && currentSelected > 0):
			position.x += (gridContainer.columns - 1) * portraitOffset.x
			position.y -= (amountOfRows) * portraitOffset.y
			
			currentColumnSpot = gridContainer.columns -1
			currentRowSpot -= 1	
		elif(currentColumnSpot < 0 && currentSelected < 0):
			position.x += (gridContainer.columns -1) * portraitOffset.x
			position.y += (amountOfRows -1) * portraitOffset.y
			
			currentColumnSpot = gridContainer.columns -1
			currentRowSpot = amountOfRows -1
			currentSelected = characters.size() - 1
			
		else:
			position.x -= portraitOffset.x
		
	if(event.is_action_pressed("ui_accept") and not event.is_echo()):
		if(CharacterSelectionManager.playerone == null):
			CharacterSelectionManager.playerone = CharacterSelectionManager.selectableCharacters[characters[currentSelected].name]
			texture = player2Text
		else:
			CharacterSelectionManager.playertwo = CharacterSelectionManager.selectableCharacters[characters[currentSelected].name]
			print(CharacterSelectionManager.playertwo)
			print(CharacterSelectionManager.playerone)
			
			active = false
			emit_signal("CharacterSelected")

			
		
