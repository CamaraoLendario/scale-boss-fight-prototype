using Godot;
using Microsoft.VisualBasic;
using System;
using System.Linq;

[Tool]
public partial class BossSkeleton : Skeleton3D
{
	[Export] GpuParticles3D particles3D;
	Vector3[] BonePoss;
	Vector3[] BoneDirs;
	int boneCount;
	ShaderMaterial material;

    public override void _Ready()
	{
		boneCount = GetBoneCount()-2;
		BonePoss = new Vector3[boneCount];
		BoneDirs = new Vector3[boneCount];
		material = particles3D.ProcessMaterial as ShaderMaterial;
		for(int i = 0; i < GetBoneCount(); i++)
		{
			GD.Print($"processing bone number {i}, name is: {GetBoneName(i)}");
		}
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
		
		material.CallDeferred(ShaderMaterial.MethodName.SetShaderParameter, "BonePoss", BonePoss);
		material.CallDeferred(ShaderMaterial.MethodName.SetShaderParameter, "BoneDirs", BoneDirs);
	}



}
