@tool
extends SS2D_Shape
@export_placeholder("ActionProperty") var _copyPoints: String = "" : set = _copy_data


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _copy_data(value: String) -> void:
	var array = []
	pass
