extends AudioStreamPlayer


# Declare member variables here. Examples:
# var a = 2
# var b = "text"

export(Array, AudioStreamOGGVorbis) var songs := []




	
	
func play_idx(i):
	stream = songs[i]
	play()
		


# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
