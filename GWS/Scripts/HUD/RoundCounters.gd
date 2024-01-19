extends HBoxContainer


onready var animation_player = $AnimationPlayer



# Called when the node enters the scene tree for the first time.
func _ready():
	animation_player.play("Init")
#	win_counter_up(1)


# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
func win_counter_up(rounds_won: int):
	if rounds_won == 1:
		animation_player.play("RoundOneWin")
	if rounds_won == 2:
		animation_player.play("RoundTwoWin")
	
