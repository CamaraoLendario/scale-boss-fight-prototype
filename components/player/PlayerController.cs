using System;
using Godot;

public partial class PlayerController : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 6f;
	public const float CameraTurnSpeed = 2f;
	public Vector3 CameraAimOffset = new Vector3(2f, -3f, -0.75f);
	[Export]
	private Mage mage;
	private Vector2 relativeMouseMovement;
	[Export]
	private Node3D cameraPivot;
	[Export]
	private Node3D cameraLookAt;
	[Export]
	private Node3D cameraAimLookAt;
	[Export]
	private Node3D cameraAimingOffset;
	[Export]
	private Camera3D camera;

	[Export]
	private Control Crosshair;

	[Export]
	private PackedScene MissileScene;

	[Export]
	private Node3D MissileSpawner;

	private float TargetCameraFov = 75;

	private bool _IsAiming;
	private bool IsAiming
	{
		get
		{
			return _IsAiming;
		}
		set
		{
			if (IsAiming && !value)
			{
				Crosshair.Hide();
				TargetCameraFov = 75;

			}

			if (!IsAiming && value)
			{
				Crosshair.Show();
				TargetCameraFov = 40;

			}
			_IsAiming = value;
		}
	}
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);
		if (@event is InputEventMouseMotion)
		{
			relativeMouseMovement = (@event as InputEventMouseMotion).Relative;
		}
	}

	public override void _Process(double delta)
	{

		IsAiming = Input.IsActionPressed("aim");
		if (IsAiming && Input.IsActionJustPressed("fire"))
		{
			mage.Fire();
		}

		camera.Fov = Mathf.Lerp(camera.Fov, TargetCameraFov, (float)delta);

	}
	public override void _PhysicsProcess(double delta)
	{
		HandleCameraMovement(delta);

		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		var directionCameraTransform = camera.GlobalRotation.Y;
		if (direction != Vector3.Zero)
		{
			direction = -direction.Rotated(Vector3.Up, directionCameraTransform);

			velocity.X = direction.X * Speed * (IsAiming ? 0.5f : 1f);
			velocity.Z = direction.Z * Speed * (IsAiming ? 0.5f : 1f);
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();

		HandleAnimation();
	}

	private void HandleAnimation()
	{
		var currentSpeed = new Vector2(Velocity.X, Velocity.Z).Length();
		mage.MovingBlendPosition = currentSpeed / Speed;
		mage.MovementDirection = new Vector3(Velocity.X, 0, Velocity.Z).Normalized();
		mage.IsOnFloor = IsOnFloor();
		mage.IsAiming = IsAiming;
		mage.AimDirection = -camera.GlobalBasis.Z;
	}

	private void HandleCameraMovement(double delta)
	{
		float fdelta = (float)delta;

		cameraAimingOffset.Position = cameraAimingOffset.Position.Lerp(IsAiming ? CameraAimOffset : Vector3.Zero, fdelta);

		Vector3 newCameraPivotRotation = cameraPivot.Rotation;
		newCameraPivotRotation += new Vector3(0, -relativeMouseMovement.X * CameraTurnSpeed * fdelta, 0);
		newCameraPivotRotation += new Vector3(-relativeMouseMovement.Y * CameraTurnSpeed * fdelta, 0, 0);

		newCameraPivotRotation.X = Mathf.Clamp(newCameraPivotRotation.X, -0.6f, 0.75f);
		cameraPivot.Rotation = newCameraPivotRotation;
		relativeMouseMovement = Vector2.Zero;

		camera.LookAt(IsAiming ? cameraAimLookAt.GlobalPosition : cameraLookAt.GlobalPosition);

	}

	public void ShootMissile()
	{
		Missile instance = MissileScene.Instantiate<Missile>();
		Vector3 missileDirection = MissileSpawner.GlobalPosition.DirectionTo(cameraAimLookAt.GlobalPosition);
		instance.Direction = missileDirection;
		GetParent().AddChild(instance);
		instance.GlobalPosition = MissileSpawner.GlobalPosition;
	}
}
