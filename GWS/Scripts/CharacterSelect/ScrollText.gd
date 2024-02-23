extends Label

var currentCharacter: int = 0
var delayBetweenCharacters: float = 0.05  # Adjust this value to control the delay between each character

var fullText: String = "SELECT YOUR LADY..."
var visibleText: String = ""

onready var characterTimer := $Timer

func _ready():
	characterTimer.connect("timeout", self, "_on_Timer_timeout")
	characterTimer.wait_time = delayBetweenCharacters
	characterTimer.autostart = true

func _on_Timer_timeout():
	if currentCharacter < fullText.length():
		visibleText += fullText[currentCharacter]
		currentCharacter += 1
		self.text = visibleText
		print("Current Text:", visibleText)  # Debugging message
		print("Current Character Index:", currentCharacter)  # Debugging message
	else:
		print("Text fully displayed.")
