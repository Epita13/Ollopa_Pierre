using Godot;
using System;

public class MenuPlay : Node2D
{

	public void Play()
	{
		GetTree().ChangeScene("res://Assets/Scenes/SceneGeneration/SceneGeneration.tscn");
	}
}
