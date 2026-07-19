using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class BossProjectile : Node3D
{
    [ExportToolButton("Explode")]
    public Callable ExplodeButton => Callable.From(Explode);

    [ExportToolButton("Reset")]
    public Callable ResetButton => Callable.From(Reset);	
	
	List<GpuParticles3D> particlesNodes = []; 
    ShaderMaterial material;
	public override void _Ready()
	{
		material = GetNode<Node3D>("Particles").GetChild<GpuParticles3D>(0).ProcessMaterial as ShaderMaterial;
		material = material.DuplicateDeep() as ShaderMaterial;
		foreach (Node3D child in GetNode<Node3D>("Particles").GetChildren())
		{
			if (child is not GpuParticles3D particles)continue;
			particlesNodes.Add(particles);
			particles.ProcessMaterial = material;
		}
	}

	void Throw(Vector3 target)
	{
		Tween tween = CreateTween();
		Vector3 initialPosition = Position;
		Vector3 travelVec = target - initialPosition;

		tween.TweenMethod(Callable.From((float tweenedValue) =>
		{
			Position = initialPosition + travelVec*tweenedValue;
			Position += Vector3.Up * Mathf.Sin(tweenedValue * Mathf.Pi) * 5f;
		}), 0f, 1f, 1f);

		tween.Finished += () =>
		{
			Explode();
		};
	}

	void Explode()
	{
		foreach (GpuParticles3D particles in particlesNodes)
		{
			particles.OneShot = true;
			particles.Restart();
		}

		material.CallDeferred(ShaderMaterial.MethodName.SetShaderParameter, "exploding", true);
		if (!Engine.IsEditorHint())
		{
			GetChild<GpuParticles3D>(0).Finished += QueueFree;
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
