extends State

@export var enemy: CharacterBody3D
@export var player: CharacterBody3D
var move_speed = 100
var move_direction = null

func Enter():
	get_player_position()

func Exit():
	pass

func get_player_position():
	move_direction = player.position - enemy.position

func Process(delta: float):    
	print("processing chasing state")    
	enemy.velocity = move_direction * move_speed * delta
	enemy.move_and_slide()
	get_player_position()

	if move_direction.length() > 10:
		transition_state.emit("Chasing", "Idle")
