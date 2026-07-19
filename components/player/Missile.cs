using System;
using Godot;

public partial class Missile : RigidBody3D
{

	public const float Force = 40f;
	public Vector3 Direction;
	public Vector3 PlayerVelocity;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ApplyCentralImpulse((Direction * Force) + PlayerVelocity);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		foreach (Node3D body in GetCollidingBodies())
		{
			GD.Print("Colliding ", body);
			if (body.IsInGroup("Enemy"))
			{
				body.Call("take_damage");
				SelfDestruct();
			}
		}
	}

	public void SelfDestruct()
	{
		QueueFree();
	}
}
