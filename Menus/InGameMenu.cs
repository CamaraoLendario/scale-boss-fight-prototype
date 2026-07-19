using Godot;
using System;

public partial class InGameMenu : Control
{
	[Export] Button resumeButton;
	[Export] Button restartButton;
	[Export] Button backToMainMenuButton;
	[Export] Button quitButton;
	string mainMenuUID = "uid://bth6efadg5djf";
	string mainSceneUID = "uid://pepkbefnshh0";
    public override void _Ready()
	{
		resumeButton.Pressed += OnResumePressed;		
		restartButton.Pressed += OnRestartPressed;
		backToMainMenuButton.Pressed += OnBackToMainMenuPressed;
		quitButton.Pressed += OnQuitPressed;		
	}

    public override void _Input(InputEvent @event)
	{
		if (Input.IsActionJustPressed("PauseGame"))
		{
			Visible = !Visible;
			GetTree().Paused = Visible;
		}
	}

    private void OnResumePressed()
    {
		GetTree().Paused = false;
		Visible = false;
    }

    private void OnRestartPressed()
    {
		if (!Visible) return;
		PackedScene mainScene = GD.Load<PackedScene>(mainSceneUID);
		GetParent().GetParent().CallDeferred(MethodName.AddChild, mainScene.Instantiate<Node3D>());
		OnResumePressed();
		GetParent().QueueFree();
    }

    private void OnBackToMainMenuPressed()
    {
		if (!Visible) return;
		PackedScene mainMenu = GD.Load<PackedScene>(mainMenuUID);
		GetParent().GetParent().CallDeferred(MethodName.AddChild, mainMenu.Instantiate<Control>());
		OnResumePressed();
		GetParent().QueueFree();
    }

    private void OnQuitPressed()
    {
		if (!Visible) return;
		GetTree().Quit();
    }
}
