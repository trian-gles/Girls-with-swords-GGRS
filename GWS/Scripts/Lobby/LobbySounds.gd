extends AudioStreamPlayer

var sounds = {
	"scroll": preload("res://Sounds/ui_scroll.ogg"),
	"select": preload("res://Sounds/ui_select.ogg"),
	"cancel": preload("res://Sounds/ui_cancel.ogg")
}

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

func play_sound(sound_name):
	stream = sounds[sound_name]
	play();



func focus_entered():
	play_sound("scroll")
