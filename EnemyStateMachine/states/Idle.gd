extends State

@export var enemy: CharacterBody3D
@export var player: CharacterBody3D

var move_speed = 15
var wander_time = 0
var move_direction = null

func Enter():
	get_random_direction()

func Exit():
	pass

func get_random_direction():
	move_direction = Vector3(randf_range(-1, 1), 0, randf_range(-1, 1))
	wander_time = randf_range(1, 3)

func Process(delta: float):    
	print("processing idle state")    
	enemy.velocity = -move_direction * move_speed
	enemy.move()
	enemy.look_at(enemy.global_position + move_direction, Vector3.UP)
	print("velocity", enemy.velocity)    

	if(wander_time < 0):
		get_random_direction()

	var distance_to_player = player.global_position - enemy.global_position
	if(distance_to_player.length() < 40):
		print("close to enemy: CHASING")
		transition_state.emit("Idle", "Chasing")

	wander_time -= delta
