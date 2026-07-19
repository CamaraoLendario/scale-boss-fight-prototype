extends State

@export var enemy: CharacterBody3D
@export var player: CharacterBody3D
var move_speed = 5
var move_direction = null
var random_attack_time = 0.0

func Enter():
	get_player_position()
	random_attack_time = randf_range(1, 3)

func Exit():
	pass

func get_player_position():
	move_direction = enemy.global_position - player.global_position

func Process(delta: float):    
	print("processing chasing state")    
	enemy.velocity = -move_direction * move_speed * delta
	enemy.look_at(Vector3(move_direction.x, 0, move_direction.z), Vector3.UP)
	enemy.move()

	if move_direction.length() > 40:
		transition_state.emit("Chasing", "Idle")

	random_attack_time -= delta
	if random_attack_time <= 0:
		random_attack_time = randf_range(1, 3)
		enemy.attack()
	
	get_player_position()
