extends AudioStreamPlayer

var sounds = {
	"scroll": preload("res://Sounds/ui_scroll.ogg"),
	"select": preload("res://Sounds/ui_select.ogg"),
	"cancel": preload("res://Sounds/ui_cancel.ogg")
}

var can_scroll = false

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

func play_sound(sound_name):
	if sound_name == "scroll" and not can_scroll:
		return
	
	stream = sounds[sound_name]
	play();
	
func _input(event):
	if event.is_action_pressed("2") or event.is_action_pressed("8"):
		can_scroll = true

func _physics_process(delta):
	can_scroll = false

func focus_entered():
	play_sound("scroll")
