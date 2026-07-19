extends CharacterBody3D


const MAX_HEALTH = 100
var current_health = MAX_HEALTH


func take_damage():
	current_health -= 1


func attack():
	print("attacking")