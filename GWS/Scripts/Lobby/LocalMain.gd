extends ToolButton


func _ready():
#	grab_focus()
	pass


func _on_AnimationPlayer_animation_finished(anim_name):
	if anim_name == "fade_in":
		grab_focus()
