extends Control


var menu_transition_time := 0.5

var menu_origin_position := Vector2.ZERO
var menu_origin_size := Vector2.ZERO

var current_menu
var menu_stack := []

onready var mainmenu = $MainMenu
onready var localmenu = $LocalMenu
onready var netplaymenu = $NetPlayMenu
onready var avconfigmenu = $AVConfigMenu
onready var tween = $Tween
onready var anim = $AnimationPlayer

const MUSIC_BUS = "Music"
const FX_BUS = "GameFX"
const UI_BUS = "UI"




func _ready():
	menu_origin_position = Vector2(0 , 0)
	menu_origin_size = get_viewport_rect().size
	current_menu = mainmenu
	anim.play("fade_in")

func move_to_next_menu(next_menu_id: String):
	var next_menu = get_menu_from_menu_id(next_menu_id)
	tween.interpolate_property(current_menu, "rect_global_position", current_menu.rect_global_position, Vector2(-menu_origin_size.x, 0), menu_transition_time)
	tween.interpolate_property(next_menu, "rect_global_position", next_menu.rect_global_position, menu_origin_position, menu_transition_time)
	tween.start()
	menu_stack.append(current_menu)
	current_menu = next_menu
	$MainMenu.visible = false
	
	
func move_to_previous_menu():
	var previous_menu = menu_stack.pop_back()
	if previous_menu != null:
		tween.interpolate_property(previous_menu, "rect_global_position", previous_menu.rect_global_position, menu_origin_position, menu_transition_time)
		tween.interpolate_property(current_menu, "rect_global_position", current_menu.rect_global_position, Vector2(menu_origin_size.x, 0), menu_transition_time)
		tween.start()
		current_menu = previous_menu
		$MainMenu.visible = true
		$LocalMenu.visible = false
		$NetPlayMenu.visible = false
		$MainMenu/CenterContainer/MainMenuButtons/Local.grab_focus()
		
	

func get_menu_from_menu_id(menu_id: String) -> Control:
	match menu_id:
		"mainmenu":
			return mainmenu
		"localmenu":
			return localmenu
		"netplaymenu":
			return netplaymenu
		"avconfigmenu":
			return avconfigmenu
		_:
			return mainmenu
	

func _on_Local_pressed():
	$LocalMenu.visible = true
	move_to_next_menu("localmenu")
	$LocalMenu/LocalButtons/Local.grab_focus()

func _on_BackButton_pressed():
	move_to_previous_menu()

func _on_NetPlay_pressed():
	$NetPlayMenu.visible = true
	move_to_next_menu("netplaymenu")
	$NetPlayMenu/Entries/NetPlayButtons/Host.grab_focus()
	
func _on_AVConfig_pressed():
	$AVConfigMenu.visible = true
	move_to_next_menu("avconfigmenu")
	$AVConfigMenu/AVConfigButtons/BackButton.grab_focus()
	
	
	


func _on_MasterVolume_value_changed(value):
	# var linearValue = linear2db(abs(value))
	# print(linearValue)
	
	if value == -50:
		AudioServer.set_bus_mute(0,true)
	else:
		AudioServer.set_bus_mute(0,false)
		AudioServer.set_bus_volume_db(0,value)


func _on_MusicVolume_value_changed(value):	
	var BusInt = AudioServer.get_bus_index(MUSIC_BUS)
	AudioServer.set_bus_volume_db(BusInt,value)

func _on_GameFXVolume_value_changed(value):
	var BusInt = AudioServer.get_bus_index(FX_BUS)
	AudioServer.set_bus_volume_db(BusInt,value)

func _on_UIVolume_value_changed(value):
	var BusInt = AudioServer.get_bus_index(UI_BUS)
	AudioServer.set_bus_volume_db(BusInt,value)
