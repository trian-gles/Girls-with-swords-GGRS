extends Label
var drawTextSpeed: int = 0

var chatterLimit: int = 56

#var chatList: Array = []

func _showChatter():
	if drawTextSpeed < chatterLimit:
		drawTextSpeed += 1
		self.visible_characters = drawTextSpeed
	
func _process(delta):
	_showChatter()
