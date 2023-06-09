extends OptionButton

export(NodePath) var optionButtonPath: NodePath

class ResolutionDictionary:
	# Resolution dictionary entries using Vector2
	var resolutions = {
		"480p": Vector2(640, 480),
		"720p": Vector2(1280, 720),
		"1080p": Vector2(1920, 1080),
		"1440p": Vector2(2560, 1440),
		"4K": Vector2(3840, 2160)
	}

	func get_resolution(resolution: String) -> Vector2:
		if resolutions.has(resolution):
			return resolutions[resolution]
		else:
			print("Resolution not found!")
			return Vector2.ZERO

func _ready():
	var resolutionDict = ResolutionDictionary.new()
	var currentResolution = load_resolution_from_json(resolutionDict)
	if currentResolution == "":
		currentResolution = "1080p"
		save_resolution_to_json(currentResolution)
	apply_resolution(currentResolution, resolutionDict)

	if has_node(optionButtonPath):
		var optionButton = get_node(optionButtonPath)
		populate_option_button(optionButton, resolutionDict)
		optionButton.connect("item_selected", self, "_on_option_button_item_selected", [optionButton, resolutionDict])

		var resolutionKeys = resolutionDict.resolutions.keys()
		var selectedResolutionIndex = resolutionKeys.find(currentResolution)
		optionButton.selected = selectedResolutionIndex

	center_game_window()

func load_resolution_from_json(dict: ResolutionDictionary) -> String:
	var jsonPath = "user://resolution.json"
	var file = File.new()
	if file.file_exists(jsonPath):
		file.open(jsonPath, File.READ)
		var jsonString = file.get_as_text()
		file.close()

		var resolutionData = JSON.parse(jsonString)
		var currentResolution = resolutionData.result["resolution"]
		print("Loaded resolution from JSON: ", currentResolution)
		return currentResolution

	return ""

func apply_resolution(currentResolution: String, dict: ResolutionDictionary) -> void:
	var resolution = dict.get_resolution(currentResolution)

	# Apply the resolution to the game's window size
	OS.window_size = resolution
	print("Resolution applied: ", resolution)

func save_resolution_to_json(currentResolution: String) -> void:
	var resolutionData = {
		"resolution": currentResolution
	}

	var jsonString = JSON.print(resolutionData)
	var savePath = "user://resolution.json"

	var file = File.new()
	file.open(savePath, File.WRITE)
	file.store_string(jsonString)
	file.close()
	print("Resolution saved to JSON file.")

func populate_option_button(optionButton: OptionButton, dict: ResolutionDictionary) -> void:
	var resolutionKeys = dict.resolutions.keys()
	for key in resolutionKeys:
		optionButton.add_item(key)

func _on_option_button_item_selected(index: int, optionButton: OptionButton, dict: ResolutionDictionary) -> void:
	var selectedResolution = optionButton.get_item_text(index)
	apply_resolution(selectedResolution, dict)
	save_resolution_to_json(selectedResolution)
	center_game_window()

func center_game_window() -> void:
	var windowSize = OS.get_window_size()
	var screenSize = OS.get_screen_size()
	var newPosition = (screenSize - windowSize) / 2
	OS.set_window_position(newPosition)
	print("Game window centered.")
