@tool
extends Node
@onready var new_button: Button = %newButton
@onready var dialog_edit: GraphEdit = %DialogEdit
@onready var text_edit: TextEdit = $NewCharacterDialog/TextEdit
@onready var new_character_dialog: ConfirmationDialog = $NewCharacterDialog
@onready var content: HSplitContainer = $MarginContainer/HSplitContainer/VBoxContainer/MarginContainer/Content
@onready var main_character_button: OptionButton = $NewSessionDialog/VBoxContainer/HBoxContainer/MainCharacterButton
@onready var secondary_character_button: OptionButton = $NewSessionDialog/VBoxContainer/HBoxContainer2/SecondaryCharacterButton
@onready var new_session_dialog: ConfirmationDialog = $NewSessionDialog
@onready var dialog_name_edit: LineEdit = $NewSessionDialog/VBoxContainer/HBoxContainer3/DialogNameEdit
@onready var file_dialog: FileDialog = $FileDialog

var start_node = preload("res://addons/eazy_dialog/components/start_node.tscn")
var dialog_node = preload("res://addons/eazy_dialog/components/dialog_node.tscn")
var end_node = preload("res://addons/eazy_dialog/components/end_node.tscn")
var index = 0
var csharp_compiler = preload("res://addons/eazy_dialog/components/Compiler.cs")
var main_character = ""
var secondary_character = ""
var dialog_name = ""

func _on_new_character_button_pressed() -> void:
	new_character_dialog.popup_centered()


func _on_new_button_pressed() -> void:
	var content = ""
	var config = ConfigFile.new()
	if config.load("res://eazy_dialog_data/data.cfg") == OK:
		content = config.get_value("EazyDialog", "Character","")
	print("Character:"+content)
	var parts = content.split("|")
	main_character_button.clear()
	secondary_character_button.clear()

	for part in parts:
		main_character_button.add_item(part)
		secondary_character_button.add_item(part)
	new_session_dialog.popup_centered()



func _on_add_start_node_pressed() -> void:
	var node = start_node.instantiate()
	dialog_edit.add_child(node)


func _on_add_dialog_node_pressed() -> void:
	var node:GraphNode = dialog_node.instantiate()
	node.name = "Dialog_"+str(index)
	node.title = "Dialog_"+str(index)
	var optionButton:OptionButton = node.get_node("HFlowContainer/HBoxContainer/OptionButton")
	print("测试"+main_character+secondary_character)
	optionButton.add_item(main_character)
	optionButton.add_item(secondary_character)
	dialog_edit.add_child(node)
	index+=1


func _on_add_end_node_pressed() -> void:
	dialog_edit.add_child(end_node.instantiate())


func _on_save_button_pressed() -> void:
	var  csharp_compiler_node = csharp_compiler.new()
	var dir = DirAccess.open("res://")
	_create_folder("res://eazy_dialog_data/"+main_character+"/"+secondary_character)
	csharp_compiler_node.ExportGraph(dialog_edit,"res://eazy_dialog_data/"+main_character+"/"+secondary_character+"/"+dialog_name+".txt")
	_refresh_filesystem()



func _on_new_character_dialog_confirmed() -> void:
	if (text_edit.text!=null):
		save_text_to_cfg(text_edit.text)



# 保存文本到 .cfg 文件
func save_text_to_cfg(character_name):
# 创建新的 ConfigFile 对象。
	var config = ConfigFile.new()

	var file_path = "res://eazy_dialog_data/data.cfg"
	var content = ""
	if config.load(file_path) == OK:
		content = config.get_value("EazyDialog", "Character","")
		if(content == ""):
			content += character_name
		else :
			content = content + "|"+character_name
	
	config.set_value("EazyDialog", "Character",content)
	#DirAccess.remove_absolute(file_path)
	config.save(file_path)
	_refresh_filesystem()


func _create_folder(path: String) -> void:
	var dir := DirAccess.open("res://")
	var current_path := "res://"
	for folder in path.replace("res://", "").split("/"):
		current_path += folder + "/"
		if not dir.dir_exists(current_path):
			dir.make_dir(current_path)
			print("Created folder: " + current_path)	

func _refresh_filesystem():
	var a = EditorInterface.get_resource_filesystem()
	a.scan_sources()


func _on_main_character_button_item_selected(index: int) -> void:
	main_character = main_character_button.get_item_text(index)
	print(main_character)

func _on_secondary_character_button_item_selected(index: int) -> void:
	secondary_character = secondary_character_button.get_item_text(index)
	print(secondary_character)


func _on_new_session_dialog_confirmed() -> void:
	dialog_name = dialog_name_edit.text
	var node:GraphNode = start_node.instantiate()
	var label:Label = node.get_node("HBoxContainer/Label")
	node.position_offset = dialog_edit.get_screen_position()
	label.text = dialog_name
	dialog_edit.add_child(node)
	main_character = main_character_button.get_item_text(main_character_button.get_selected_id())
	secondary_character = secondary_character_button.get_item_text(secondary_character_button.get_selected_id())

	if(!content.visible):
		content.visible = true


func _on_dialog_edit_connection_request(from_node: StringName, from_port: int, to_node: StringName, to_port: int) -> void:
	dialog_edit.connect_node(from_node,from_port,to_node,to_port)
	



func _on_open_button_pressed() -> void:
	file_dialog.popup_centered()


func _on_file_dialog_confirmed() -> void:
	var  csharp_compiler_node = csharp_compiler.new()
	#csharp_compiler_node.FileToGraph("res://eazy_dialog_data/Cat/Chicken/A.txt",dialog_edit)
	csharp_compiler_node.FileToGraph(file_dialog.current_path,dialog_edit)
