extends MarginContainer

const SAVE_DIR = "user://Config/"
var save_path = "user://AudioConfig.json"
var AudioConfigValues
var ControllerConfigValues
var VideoConfigValues

onready var value_data = {}


func _ready():
	load_JSON("AudioConfig")
	pass


func _on_BackButton_pressed(): #save current audio settings to config
	#read audio slider data
	value_data ={
		
	"MasterVolume" : $AVConfigButtons/MasterVolume.value,
	"MusicVolume" : $AVConfigButtons/MusicVolume.value,
	"GameFXVolume" : $AVConfigButtons/GameFXVolume.value,
	"UIVolume" : $AVConfigButtons/UIVolume.value
	
	}
	
	save_JSON()
	
func save_JSON():
	#print("Saving Audio Config",value_data)
	var file
	file = File.new()
	file.open(save_path, File.WRITE)
	file.store_line(to_json(value_data))
	file.close()
	

func load_JSON(savetype:String):
	var config_file = File.new()
	if config_file.open("user://"+savetype+".json", File.READ)== OK:
		var config_json = JSON.parse(config_file.get_as_text())
		config_file.close()
		if savetype == "AudioConfig":
			AudioConfigValues = config_json.result
#			print(AudioConfigValues)
			load_Values("Audio")
			
		else :
			ControllerConfigValues = config_json.result
	else:
		print("Error loading Config, not found")
		
func load_Values(configtype:String):
	if (configtype == "Audio"):
		for values in AudioConfigValues:
#			print(values,": ",AudioConfigValues[values])
			get_node("AVConfigButtons/"+values).value = AudioConfigValues[values]

	
