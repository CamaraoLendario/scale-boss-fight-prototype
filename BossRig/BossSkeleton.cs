using Godot;
using Microsoft.VisualBasic;
using System;
using System.Linq;

[Tool]
public partial class BossSkeleton : Skeleton3D
{
	[Export] float Health
	{
		get
		{
			return health;
		}
		set
		{
			health = value;
			if (IsNodeReady()) SetHealth(value);
		}
	}
	float health;
	GpuParticles3D particles3D;
	ShaderMaterial material;
	Vector3[] BonePoss;
	Vector3[] BoneDirs;
	float[] BoneLens;
	float lenTotal = 0f;
	int boneCount;

    public override void _Ready()
	{
		boneCount = GetBoneCount() - 2;
		BonePoss = new Vector3[boneCount+2];
		BoneDirs = new Vector3[boneCount+2];
		BoneLens = new float[boneCount+2];
		
		particles3D = GetChild<GpuParticles3D>(0);
		material = particles3D.ProcessMaterial as ShaderMaterial;

		for(int i = 0; i < boneCount; i++) {
			int currentBoneIdx = i+2;
			Transform3D boneTransform = GetBoneGlobalPose(currentBoneIdx);
			int boneParent = GetBoneParent(currentBoneIdx);
			if(boneParent != -1)
				BoneLens[i] = (GetBoneGlobalPose(boneParent).Origin - boneTransform.Origin).Length();
			else
				BoneLens[i] = 0f;
			GD.Print($"Processing bone number {currentBoneIdx} | Name: {GetBoneName(currentBoneIdx)} | Length: {BoneLens[i]}");
			lenTotal += BoneLens[i];
		}

		//Hard coded extra bones for a better form
		BonePoss[boneCount] = GetBoneGlobalPose(2).Origin; // spine
		BonePoss[boneCount+1] = BonePoss[boneCount];

		BoneDirs[boneCount] = GetBoneGlobalPose(4).Origin - BonePoss[boneCount]; // upperarm.r
		BoneDirs[boneCount+1] = GetBoneGlobalPose(10).Origin - BonePoss[boneCount]; // upperarm.l
		
		BoneLens[boneCount] = BoneDirs[boneCount].Length();  
		BoneLens[boneCount+1] = BoneDirs[boneCount+1].Length();  
		lenTotal += BoneLens[boneCount] + BoneLens[boneCount+1];
		//End of Hard coding

		material.CallDeferred(ShaderMaterial.MethodName.SetShaderParameter, "BoneLens", BoneLens);
		material.CallDeferred(ShaderMaterial.MethodName.SetShaderParameter, "lenTotal", lenTotal);
	}
 

    public override void _Process(double delta)
	{
		for(int i = 0; i < boneCount; i++) {
			int currentBoneIdx = i+2;
			Transform3D boneTransform = GetBoneGlobalPose(currentBoneIdx);
			
			BonePoss[i] = boneTransform.Origin;
			int boneParent = GetBoneParent(currentBoneIdx);
			if(boneParent != -1)
				BoneDirs[i] = GetBoneGlobalPose(boneParent).Origin - boneTransform.Origin;
			else BoneDirs[i] = Vector3.Zero;
		}
		
		//Hard coded extra bones for a better form
		BonePoss[boneCount] = GetBoneGlobalPose(2).Origin; // spine
		BonePoss[boneCount+1] = BonePoss[boneCount];

		BoneDirs[boneCount] = GetBoneGlobalPose(4).Origin - BonePoss[boneCount]; // upperarm.r
		BoneDirs[boneCount+1] = GetBoneGlobalPose(10).Origin - BonePoss[boneCount]; // upperarm.l
		
		BoneLens[boneCount] = BoneDirs[boneCount].Length();  
		BoneLens[boneCount+1] = BoneDirs[boneCount+1].Length();  
		//End of Hard coding
		
		material.CallDeferred(ShaderMaterial.MethodName.SetShaderParameter, "BonePoss", BonePoss);
		material.CallDeferred(ShaderMaterial.MethodName.SetShaderParameter, "BoneDirs", BoneDirs);
	}

	// Used to set the particles Count and scale of the boss
	// 'health' should always be a number between 0 and 1
	void SetHealth(float health)
	{
		health = Mathf.Max(health, 0f);
		int particlesAmmount = (int) (health * 300f);

		foreach(Node child in GetChildren())
		{
			if (child is not GpuParticles3D particleEmitter) continue;
			
			particleEmitter.Amount = particlesAmmount;
		}

		material.SetShaderParameter("particlesAmmount", particlesAmmount);
		material.SetShaderParameter("scale", health * 10f);
	}
}
