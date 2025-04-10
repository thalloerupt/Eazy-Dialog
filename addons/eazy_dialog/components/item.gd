
@tool
extends HBoxContainer



func _on_button_pressed() -> void:
	get_parent().remove_child(self)
