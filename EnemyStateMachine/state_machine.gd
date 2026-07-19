extends Node
class_name StateMachine

@export var initial_state : State = null

var current_state: State = null
var states: Dictionary = {}

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	for child in get_children():
		if child is State:
			states[child.name.to_lower()] = child
			child.transition_state.connect(change_state)

	initial_state.Enter()
	current_state = initial_state

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	current_state.Process(delta)

func change_state(prevState: String, nextState: String) -> void:
	states[prevState.to_lower()].Enter()
	states[nextState.to_lower()].Enter() 
	current_state = states[nextState.to_lower()]
	
