@tool
extends GraphNode
const ITEM_DIALOG = preload("res://addons/eazy_dialog/components/item_dialog.tscn")
@onready var item_container: VBoxContainer = $VBoxContainer/ItemContainer
var index = 0
func _on_add_item_button_pressed() -> void:
	var item = ITEM_DIALOG.instantiate()
	item.name = "item_"+str(index)
	item_container.add_child(item)
