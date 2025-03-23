@tool
extends EditorInspectorPlugin

#func _can_handle(object):
	#return object is CustomNode

func _parse_property(object, type, name, hint_type, hint_string, usage_flags, wide):

		pass
		
		
func _on_button_pressed(object):
	print("Button clicked for:", object)
