using Godot;
using System;

public partial class BossProjectileThrower : Node3D
{
	[Export] Vector3 defaultAimPos = Vector3.One;
	string bossProjectileUID = "uid://cgr88dn66r45u";

    public override void _Input(InputEvent @event)
    {
		if (!@event.IsReleased() &&  @event is not InputEventMouseMotion)
			ThrowNewProjectile();
    }
	void ThrowNewProjectile(Vector3 aimPosition)
	{
		PackedScene bossProjectileScene = GD.Load<PackedScene>(bossProjectileUID);
		BossProjectile bossProjectile = bossProjectileScene.Instantiate<BossProjectile>();
		
		CallDeferred(MethodName.AddChild, bossProjectile);
		bossProjectile.CallDeferred(BossProjectile.MethodName.Throw, aimPosition);
	}
	void ThrowNewProjectile()
	{
		PackedScene bossProjectileScene = GD.Load<PackedScene>(bossProjectileUID);
		BossProjectile bossProjectile = bossProjectileScene.Instantiate<BossProjectile>();
		
		CallDeferred(MethodName.AddChild, bossProjectile);
		bossProjectile.CallDeferred(BossProjectile.MethodName.Throw, defaultAimPos);
	}
}
