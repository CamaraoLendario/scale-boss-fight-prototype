using Godot;
using System;

public partial class BossProjectileThrower : Node3D
{
	string bossProjectileUID = "uid://cgr88dn66r45u";

	void ThrowNewProjectile(Vector3 aimPosition)
	{
		PackedScene bossProjectileScene = GD.Load<PackedScene>(bossProjectileUID);
		BossProjectile bossProjectile = bossProjectileScene.Instantiate<BossProjectile>();
		bossProjectile.GlobalPosition = GlobalPosition;

		GetTree().GetFirstNodeInGroup("MainWorld").CallDeferred(MethodName.AddChild, bossProjectile);
		bossProjectile.CallDeferred(BossProjectile.MethodName.Throw, aimPosition);
	}
	
}
