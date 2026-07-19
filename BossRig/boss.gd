extends CharacterBody3D


const MAX_HEALTH = 100
var current_health = MAX_HEALTH
var animation_tree: AnimationTree
@export var material: Material

func _ready() -> void:
	animation_tree = get_node("Rig_Large_MovementBasic/AnimationTree")

func take_damage():
	current_health -= 1


func attack():
	animation_tree.set("parameters/OneShot/request", AnimationNodeOneShot.ONE_SHOT_REQUEST_FIRE)
	%BossProjectileThrower.ThrowNewProjectile()
	print("attacking")

func move():
	print("rotation: ", rotation)
	move_and_slide()
	# material.set_shader_parameter("position", position); 
	# material.set_shader_parameter("Yrotation",global_rotation.y); 
