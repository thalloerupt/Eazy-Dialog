extends Node2D
const eazy_dialog_runtime_cs = preload("res://addons/eazy_dialog/components/EazyDialogRuntime.cs")
@onready var dialog_bar: Control = $MainCharacter/DialogBar
@onready var main: TextureRect = $MainCharacter/DialogBar/PanelContainer/HBoxContainer/Character
@onready var chicken: TextureRect = $MainCharacter/DialogBar/PanelContainer/HBoxContainer/Chicken
@onready var label: RichTextLabel = $MainCharacter/DialogBar/PanelContainer/HBoxContainer/VBoxContainer/RichTextLabel
const CHICKEN = preload("res://example/character/chicken.png")
const CHARACTER = preload("res://example/character/Character.png")

var eazy_dialog_runtime
var isEnterChicken = false
func _ready() -> void:
	eazy_dialog_runtime = eazy_dialog_runtime_cs.new()
	eazy_dialog_runtime.DialogueSignal.connect(dialog_signal_handel)
	eazy_dialog_runtime.DialogueEndSignal.connect(dialog_end_signal_handel)
func _process(delta: float) -> void:
	if Input.is_action_just_pressed("ui_accept") and isEnterChicken:
		eazy_dialog_runtime.PlayNext("res://eazy_dialog_data/Cat/Chicken/dialog_name.txt",0)

	if Input.is_action_just_pressed("ui_left_click") and isEnterChicken:
		print(isEnterChicken)
		eazy_dialog_runtime.PlayNext("res://eazy_dialog_data/Cat/Chicken/dialog_name.txt",0)


func dialog_signal_handel(character: String,content: Array):
	if(dialog_bar.visible == false):
		dialog_bar.visible = true
	if (character == "Cat"):
		main.texture = CHARACTER
		chicken.texture = null
	if (character == "Chicken"):
		main.texture = null
		chicken.texture = CHICKEN
	label.text = content[0]


func dialog_end_signal_handel():
	if(dialog_bar.visible == true):
		dialog_bar.visible = false

func _on_chicken_area_body_entered(body: Node2D) -> void:
	isEnterChicken = true
	


func _on_chicken_area_body_exited(body: Node2D) -> void:
	isEnterChicken = false
