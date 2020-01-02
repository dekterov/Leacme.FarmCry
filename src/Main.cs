// Copyright (c) 2017 Leacme (http://leac.me). View LICENSE.md for more information.
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Main : Spatial {

	public AudioStreamPlayer Audio { get; } = new AudioStreamPlayer();
	private List<StaticBody> anims;

	private void InitSound() {
		if (!Lib.Node.SoundEnabled) {
			AudioServer.SetBusMute(AudioServer.GetBusIndex("Master"), true);
		}
	}

	public override void _Notification(int what) {
		if (what is MainLoop.NotificationWmGoBackRequest) {
			GetTree().ChangeScene("res://scenes/Menu.tscn");
		}
	}

	public override void _Ready() {
		GetNode<WorldEnvironment>("sky").Environment.BackgroundColor = new Color(Lib.Node.BackgroundColorHtmlCode);
		InitSound();
		AddChild(Audio);

		var animalsPS = GD.Load<PackedScene>("res://scenes/Animals.tscn");
		var animals = animalsPS.Instance();

		AddChild(animals);

		anims = animals.GetChildren().Cast<StaticBody>().ToList();
		int i = 0;
		for (int x = 0; x < 3; x++) {
			for (int y = 0; y < anims.Count / 3; y++) {
				anims.ElementAt(i).Translate(new Vector3(x * 1.5f - 0.5f, 0, y));
				anims.ElementAt(i).RotateY(Mathf.Deg2Rad(-90));
				anims.ElementAt(i).RotateX(Mathf.Deg2Rad((float)GD.RandRange(0, 360)));
				anims.ElementAt(i).Scale *= 6;
				((MeshInstance)anims.ElementAt(i).GetChild(0).GetChild(0)).MaterialOverride = new SpatialMaterial() {
					AlbedoColor = Color.FromHsv(1, 1, 1, 0),
					EmissionEnabled = true,
					Emission = Color.FromHsv(GD.Randf(), 1, 1),
					EmissionEnergy = 1
				};
				i++;
			}
		}

		anims.ForEach(z => z.Connect("input_event", this, nameof(OnAnimalButton), new Godot.Collections.Array { z.Name.Split("Physics")[0].ToLower() }));

	}

	public void OnAnimalButton(Node camera, InputEvent @event, Vector3 click_pos, Vector3 click_normal, int shape_idx, string animalName) {

		if (@event is InputEventScreenTouch te && te.Pressed) {
			GD.Load<AudioStream>("res://assets/animals/" + animalName + ".ogg").Play(GetTree().Root.GetNode<Main>("Main").Audio);

		}
	}

	public override void _Process(float delta) {
		anims.ForEach(z => z.RotateX(delta / 5));
	}

}
