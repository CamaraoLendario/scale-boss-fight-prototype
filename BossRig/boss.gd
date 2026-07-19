extends CharacterBody3D

const MAX_HEALTH = 100
var current_health = MAX_HEALTH
var animation_tree: AnimationTree
@export var material: Material

func _ready() -> void:
	animation_tree = get_node("Rig_Large_MovementBasic/AnimationTree")

func take_damage():
	current_health -= 5
	animation_tree.set("parameters/hit/request", AnimationNodeOneShot.ONE_SHOT_REQUEST_FIRE)
	%Skeleton3D.SetHealth(current_health as float / MAX_HEALTH as float)
	print("Current health: ", current_health)

func attack(pos: Vector3):
	animation_tree.set("parameters/OneShot/request", AnimationNodeOneShot.ONE_SHOT_REQUEST_FIRE)
	%BossProjectileThrower.ThrowNewProjectile(pos)
	print("attacking")

func move():
	# print("rotation: ", rotation)
	move_and_slide()
	# material.set_shader_parameter("position", position); 
	# material.set_shader_parameter("Yrotation",global_rotation.y); 
