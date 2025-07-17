extends AudioStreamPlayer


# Declare member variables here. Examples:
# var a = 2
# var b = "text"

export(Array, AudioStreamOGGVorbis) var songs := []




	
	
func play_random():
	stream = songs[randi() % len(songs)]
	play()
		


# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
