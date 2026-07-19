using Godot;
using System;

public partial class MainMenu : Control
{
	[Export] Button startButton;
	[Export] Button settingsButton;
	[Export] Button quitButton;
	string mainSceneUID = "uid://deuoi6be6siu2";

    public override void _Ready()
	{
		startButton.Pressed += OnStartPressed;
		settingsButton.Pressed += OnSettingsPressed;
		quitButton.Pressed += OnQuitPressed;
	}

    private void OnStartPressed()
    {
		PackedScene mainScene = GD.Load<PackedScene>(mainSceneUID);

		GetParent().AddChild(mainScene.Instantiate<Node3D>());

		QueueFree();
    }

    private void OnSettingsPressed()
    {
		GD.Print("wait, you really thought we had time to make a settings screen? LOL");
    }

    private void OnQuitPressed()
    {
        GetTree().Quit();
    }
}
