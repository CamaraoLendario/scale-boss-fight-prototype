using Godot;

public partial class Missile : Area3D
{

	public const float Speed = 10f;
	public Vector3 Direction;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		Position += Direction * Speed * (float)delta;
	}

}
