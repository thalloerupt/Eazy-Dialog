@tool
extends EditorPlugin


const MainPanel =preload("res://addons/eazy_dialog/main.tscn")
var main_panel_instance
var inspector_plugin




func _enter_tree():
	main_panel_instance = MainPanel.instantiate()
	# Add the main panel to the editor's main viewport.
	EditorInterface.get_editor_main_screen().add_child(main_panel_instance)
	# Hide the main panel. Very much required.
	_make_visible(false)
	var dir = DirAccess.open("res://")
	if not dir.dir_exists("res://eazy_dialog_data"):
		dir.make_dir("res://eazy_dialog_data")
	save_text_to_cfg()

		


func _exit_tree():
	if main_panel_instance:
		main_panel_instance.queue_free()




func _has_main_screen():
	return true


func _make_visible(visible):
	if main_panel_instance:
		main_panel_instance.visible = visible


func _get_plugin_name():
	return "Eazy Dialog"


func _get_plugin_icon():
	# Must return some kind of Texture for the icon.
	return EditorInterface.get_editor_theme().get_icon("Node", "EditorIcons")


# 保存文本到 .cfg 文件
func save_text_to_cfg():
# 创建新的 ConfigFile 对象。
	var config = ConfigFile.new()
	if config.load("res://eazy_dialog_data/data.cfg") != OK:
		config.set_value("EazyDialog", "Character", "")
		config.save("res://eazy_dialog_data/data.cfg")
		_refresh_filesystem()

func _refresh_filesystem():
	var a = EditorInterface.get_resource_filesystem()
	a.scan() 
