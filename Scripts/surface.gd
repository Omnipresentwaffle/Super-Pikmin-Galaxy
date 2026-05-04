extends Line2D

## Resource that holds shape point geometry (aka point array).
@export_placeholder("ActionProperty") var _refresh: String = "" : set = _refresh_action

func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

	
func _refresh_action(value: String) -> void:
	
