using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class BossProjectile : Node3D
{
	List<GpuParticles3D> particlesNodes = [];
	ShaderMaterial material;
	Area3D area3D;
	bool IsExploding = false;

	[Export]
	public PackedScene petScene;
	public override void _Ready()
	{
		ShaderMaterial newMaterial = GetNode<Node3D>("Particles").GetChild<GpuParticles3D>(0).ProcessMaterial.Duplicate(true) as ShaderMaterial;
		material = newMaterial;
		foreach (Node3D child in GetNode<Node3D>("Particles").GetChildren())
		{
			if (child is not GpuParticles3D particles) continue;
			particlesNodes.Add(particles);
			particles.ProcessMaterial = material;
		}

		area3D = GetNode<Area3D>("Area3D");
		area3D.BodyEntered += OnCollision;
		//area3D.AreaEntered += OnCollision;
	}

	private void OnCollision(Node3D body)
	{
		if (!body.IsInGroup("Boss"))
			Explode();

	}

	void Throw(Vector3 target)
	{
		Tween tween = CreateTween();
		Vector3 initialPosition = GlobalPosition;
		Vector3 travelVec = target - initialPosition;

		tween.TweenMethod(Callable.From((float tweenedValue) =>
		{
			if (!IsExploding)
			{
				GlobalPosition = initialPosition + travelVec * tweenedValue;
				GlobalPosition += Vector3.Up * Mathf.Sin(tweenedValue * Mathf.Pi) * 5f;
			}
		}), 0f, 1f, 1f/* travelVec.Length()/10f */);

		tween.Finished += () =>
		{
			Explode();
		};
	}

	public void Explode()
	{
		if (IsExploding) return;

		IsExploding = true;

		foreach (GpuParticles3D particles in particlesNodes)
		{
			particles.OneShot = true;
			particles.Restart();
		}

		material.CallDeferred(ShaderMaterial.MethodName.SetShaderParameter, "exploding", true);

		PrepareForDeletion();

		(area3D.GetChild<CollisionShape3D>(0).Shape as SphereShape3D).Radius = 5f; // default radius is 2.5f
		CallDeferred(BossProjectile.MethodName.CheckHit);

		if (petScene != null)
		{
			for (int i = 0; i < 4; i++)
			{
				var instance = petScene.Instantiate();
				GetParent().AddChild(instance);
				(instance as Node3D).GlobalPosition = GlobalPosition;
			}

		}

	}

	async void PrepareForDeletion()
	{
		area3D.SetDeferred(Area3D.PropertyName.Monitorable, false);
		area3D.SetDeferred(Area3D.PropertyName.Monitoring, false);

		await ToSignal(GetTree().CreateTimer(1.5f), Timer.SignalName.Timeout);

		QueueFree();
	}

    void CheckHit()
    {
		if (area3D.Monitoring)
			foreach(Node3D body in area3D.GetOverlappingBodies())
			{
				if (body.IsInGroup("Player"))
				{
					GD.Print("PLAYER HIT!");
					PrepareForDeletion();
				}
			}
    }

	void Reset()
	{
		foreach (GpuParticles3D particles in particlesNodes)
		{
			particles.OneShot = false;
			particles.Restart();
		}

		material.CallDeferred(ShaderMaterial.MethodName.SetShaderParameter, "exploding", false);
	}
}
