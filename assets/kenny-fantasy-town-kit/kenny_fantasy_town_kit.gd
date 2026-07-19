@tool
extends Node3D

func _ready():
	for i in get_child_count():
		var child = get_child(i)
		var mesh = child.get_child(0)
		child.remove_child(mesh)
		add_child(mesh)
		mesh.owner = self
		#child.queue_free()
		mesh.create_trimesh_collision()