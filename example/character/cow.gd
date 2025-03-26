class_name cow extends CharacterBody2D
@onready var main_character: CharacterBody2D = $"../MainCharacter"
@onready var dialog_bar: Control = $"../MainCharacter/DialogBar"
const eazy_dialog_runtime_cs = preload("res://addons/eazy_dialog/components/EazyDialogRuntime.cs")
var eazy_dialog_runtime

@onready var chicken: TextureRect = $"../MainCharacter/DialogBar/PanelContainer/HBoxContainer/Chicken"
@onready var label: RichTextLabel = $"../MainCharacter/DialogBar/PanelContainer/HBoxContainer/VBoxContainer/RichTextLabel"
@onready var main: TextureRect = $"../MainCharacter/DialogBar/PanelContainer/HBoxContainer/Character"
@onready var selection: ItemList = $"../MainCharacter/DialogBar/PanelContainer/HBoxContainer/VBoxContainer/Selections"

const CHARACTER = preload("res://example/character/Character.png")
const COW = preload("res://example/character/cow.png")

func _ready() -> void:
	eazy_dialog_runtime = eazy_dialog_runtime_cs.new()
	eazy_dialog_runtime.DialogueSignal.connect(dialog_signal_handel)
	eazy_dialog_runtime.DialogueEndSignal.connect(dialog_end_signal_handel)

func _process(delta: float) -> void:
	if position.distance_to(main_character.position) < 100 and (Input.is_action_just_pressed("ui_left_click") or Input.is_action_just_pressed("ui_accept")):
		eazy_dialog_runtime.PlayNext("res://eazy_dialog_data/Cat/Cow/Hello2.txt",-1)


func dialog_signal_handel(character: String,content :String ,answers: Array):
	if(dialog_bar.visible == false):
		dialog_bar.visible = true
	if (character == "Cat"):
		main.texture = CHARACTER
		chicken.texture = null
	if (character == "Cow"):
		main.texture = null
		chicken.texture = COW
		
	label.text = content
	if(answers.size() != 1):
		for answer  in answers:
			selection.add_item(answer)
	


func dialog_end_signal_handel():
	selection.clear()
	if(dialog_bar.visible == true):
		dialog_bar.visible = false
