using Godot;
using System;
using System.Threading.Tasks;

public partial class Mage : Node3D
{
	public Vector3 MovementDirection = Vector3.Zero;
	public float MovingBlendPosition = 0f;
	public bool IsOnFloor = true;
	public bool IsAiming = false;
	public Vector3 AimDirection = Vector3.Zero;
	private AnimationTree animationTree;

	public const float TurnSpeed = 4.0f;
	private Vector3? lookAtGlobalPosition;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animationTree = GetNode<AnimationTree>("%AnimationTree");
	}

	[Signal]
	public delegate void IsShootingMissileEventHandler();

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		animationTree.Set("parameters/moving/blend_position", MovingBlendPosition);
		if (!IsAiming && MovementDirection.Length() > 0)
		{
			Vector3 planarMovementDirection = MovementDirection * new Vector3(1, 0, 1);
			if (!lookAtGlobalPosition.HasValue) lookAtGlobalPosition = GlobalPosition - planarMovementDirection;
			else lookAtGlobalPosition = lookAtGlobalPosition.Value.Lerp(GlobalPosition - planarMovementDirection, TurnSpeed * (float)delta);
			LookAt(lookAtGlobalPosition.Value);
		}

		if (IsAiming)
		{
			Vector3 planarAimingDirection = AimDirection * new Vector3(1, 0, 1);
			if (!lookAtGlobalPosition.HasValue) lookAtGlobalPosition = GlobalPosition - planarAimingDirection;
			else lookAtGlobalPosition = lookAtGlobalPosition.Value.Lerp(GlobalPosition - planarAimingDirection, TurnSpeed * (float)delta);
			LookAt(lookAtGlobalPosition.Value);
		}


		animationTree.Set("parameters/conditions/is_on_floor", IsOnFloor);
		animationTree.Set("parameters/conditions/is_floating", !IsOnFloor);
	}

	public void Fire()
	{
		AnimationNodeStateMachinePlayback stateMachine = (AnimationNodeStateMachinePlayback)animationTree.Get("parameters/playback");
		stateMachine.Travel("Fire");
	}

	public void ShootMissile()
	{
		EmitSignal("IsShootingMissile");
	}
}
