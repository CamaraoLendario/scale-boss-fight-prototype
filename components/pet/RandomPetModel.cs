using Godot;
using Godot.Collections;

public partial class RandomPetModel : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Array<Node> children = GetChildren();
		int randomIndex = GD.RandRange(0, children.Count);
		var randomPet = (children[randomIndex] as Node3D);

		randomPet.Show();
		var animationPlayer = randomPet.GetChild<AnimationPlayer>(1);
		var dance = animationPlayer.GetAnimation("dance");
		dance.LoopMode = Animation.LoopModeEnum.Pingpong;
		animationPlayer.Play("dance");

	}

}
