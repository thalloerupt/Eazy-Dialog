class_name EazyDialogRuntime extends Node

const runtime_cs = preload("res://addons/eazy_dialog/components/EazyDialogRuntime.cs")
var runtime
var _name:Label
var _content:Label
var _item_list: ItemList
var _dialog_file:String

signal  dialog_start()
signal dialog_play(character:String,content:String)
signal dialog_selection_show(answers:Array)
signal dialog_end()


func _init(dialog_file:String,nameLabel:Label,contentLabel:Label,item_list: ItemList) -> void:
	_name = nameLabel
	_content = contentLabel
	_item_list = item_list
	_dialog_file = dialog_file
	
	runtime = runtime_cs.new()
	runtime.DialogueSignal.connect(_dialogue_signal)
	runtime.DialogueSelectionSignal.connect(_dialogue_selection_signal)
	runtime.DialogueEndSignal.connect(_dialogue_end_signal)
	if(!item_list.item_selected.has_connections()):
		item_list.item_selected.connect(_on_item_list_item_selected)
	runtime.PlayNext(_dialog_file,-1)
	call_deferred("emit_signal", "dialog_start")


func play()->void:
	runtime.PlayNext(_dialog_file,-1)

func _dialogue_signal(character:String,content:String)->void:
	_name.text = character
	_content.text = content
	dialog_play.emit(character,content)
	
func _dialogue_selection_signal(answers:Array)->void:
	_item_list.clear()
	_item_list.visible = true
	for answer in answers:
		_item_list.add_item(answer)
	dialog_selection_show.emit(answers)

	
func _dialogue_end_signal()->void:
	_item_list.clear()
	dialog_end.emit()
	self.queue_free()
	
func _on_item_list_item_selected(index:int)->void:
	
	runtime.PlayNext(_dialog_file,index)
	
	_item_list.visible = false
