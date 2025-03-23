@tool
class_name DialogBubble extends MarginContainer

# 自定义属性

var my_property: String = ""






# 处理 Inspector 变化
func _set(property, value):
	if property == "_character_name":
		my_property = value
		name = my_property
		return true
	notify_property_list_changed()
	return false

func _get(property):
	if property == "_character_name":
		return my_property
	return null

func _get_property_list():
	return [
		{
			"name": "_character_name",
			"type": TYPE_STRING,
			"hint": PROPERTY_HINT_NONE,
			"usage": PROPERTY_USAGE_DEFAULT
		}
	]
