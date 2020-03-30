using Godot;
using System;
using System.Collections.Generic;

public abstract class Building : Node2D
{
	
	// Enumeration : Type de batiment disponible
	public enum Type
	{
		SolarPanel,
		Storage
	}
	// Dictionaire : Stock les scnenes batiment en fonction du type de batiment
	public static Dictionary<Type, PackedScene> prefabs = new Dictionary<Type, PackedScene>
	{
		{Type.SolarPanel, GD.Load<PackedScene>("res://Assets/Objects/Buildings/SolarPanel/SolarPanel.tscn")},
		{Type.Storage, GD.Load<PackedScene>("res://Assets/Objects/Buildings/Storage/Storage.tscn")}
	};

	/*
		Object abstract:  Building

		/!\ Initialisation static : STRICTEMENT NECESSAIRE.
			- Utiliser la fonction Init()
			- Verification d'initialisation : le getter IsInit

		/!\ Classe Initialisées necessaire : None

		Description de l'object :
			Un batiment (Building) est un object placable dans une scene.
			Il possede de la vie et peut ainsi en perdre. 
			La classe Building est une definition generale d'un batiment et a donc d'autre classes qui herite de celui-ci.
			Les coordonnées manipulées dans cette classe sont strictement celle de Godot.

		Description des parametres:
			(static) Node parent : est la node dans laquelle les buildings vont etre instancier en tant qu'enfant de cette node.
			(static) int zIndex : est la profondeur 2D a laquelle les buildings vont etre instancier.
			(static) bool isInit : True si Building est initialisé, false sinon.
			Vector2 location : est la position du batiment dans la scene, si le batiment n'est pas placer alors : (-1,-1). 
			bool isPlaced : true si le batiment est placer dans la scene; false sinon.
			int health, maxGHealth : represente la vie du joueur et son maximum de vie.
	*/
	
	
	/// Initialise les variables pour le fonctionnement des batiments (OBLIGATOIRE)
	public static void Init(Node parent, int zIndex = -1)
	{
		isInit = true;
		Building.parent = parent;
		Building.zIndex = zIndex;
	}


	public Vector2 location;
	public bool isPlaced = false;

	public int healthMax = 100;
	public int health;

	public Building(int healthMax)
	{
		this.health = healthMax;
		this.healthMax = healthMax;
	}
	public override void _Ready()
	{
		Vector2 p = GetViewportTransform().origin * CurrentCamera.GetXZoom();
		Vector2 vecMin = Convertion.Location2World(p) * -1;
		prev_x_viewport = vecMin.x;
	}

	public override void _PhysicsProcess(float delta)
	{

		if (!isPlaced)
			return;
		/*Teleportation buildings*/
		Vector2 p = GetViewportTransform().origin * CurrentCamera.GetXZoom();
		int viewportSizeX = Mathf.FloorToInt(GetViewport().Size.x * CurrentCamera.GetXZoom());
		Vector2 vecMin = Convertion.Location2World(p) * -1;
		Vector2 vecMax = Convertion.Location2World(new Vector2(p.x * -1 + viewportSizeX, p.y));
		if (vecMin.x < 0)
		{
			if (!mirrored)
			{
				int i = (int) Mathf.Abs(vecMin.x / Chunk.size) + 1;
				if (Convertion.Location2World(Position).x >= (World.size - i) * Chunk.size)
				{
					//Position = Position - new Vector2(World.size * Chunk.size * World.BlockTilemap.CellSize.x, 0);
					Move(Position.x - World.size * Chunk.size * World.BlockTilemap.CellSize.x);
					mirrored = true;
				}
			}
			else if (-vecMin.x + prev_x_viewport >= 0.90f * World.size * Chunk.size)
			{
				prev_x_viewport = vecMin.x;
				int i = (int) Mathf.Abs(vecMin.x / Chunk.size) + 1;
				if (Convertion.Location2World(Position).x >= (World.size - i) * Chunk.size)
				{
					//Position = Position - new Vector2(World.size * Chunk.size * World.BlockTilemap.CellSize.x, 0);
					Move(Position.x -World.size * Chunk.size * World.BlockTilemap.CellSize.x);
					mirrored = false;
				}
			}
		}
		else if (vecMax.x >= World.size * Chunk.size)
		{
			if (!mirrored)
			{
				int i = (int) Mathf.Abs((vecMax.x - World.size * Chunk.size) / Chunk.size) + 1;
				if (Convertion.Location2World(Position).x <= i * Chunk.size)
				{
					//Position = Position + new Vector2(World.size * Chunk.size * World.BlockTilemap.CellSize.x, 0);
					Move(Position.x + World.size * Chunk.size * World.BlockTilemap.CellSize.x);
					mirrored = true;
				}
			}
			else if (vecMin.x - prev_x_viewport >= 0.90f * World.size * Chunk.size)
			{
				prev_x_viewport = vecMin.x;
				int i = (int) Mathf.Abs((vecMax.x - World.size * Chunk.size) / Chunk.size) + 1;
				if (Convertion.Location2World(Position).x <= i * Chunk.size)
				{
					//Position = Position + new Vector2(World.size * Chunk.size * World.BlockTilemap.CellSize.x, 0);
					Move(Position.x + World.size * Chunk.size * World.BlockTilemap.CellSize.x);
					mirrored = false;
				}
			}
		}
		else if (vecMax.x < World.size * Chunk.size && vecMin.x >= 0)
		{
			if (mirrored)
			{
				if (Convertion.Location2World(Position).x < 0)
				{
					//Position = Position + new Vector2(World.size * Chunk.size * World.BlockTilemap.CellSize.x, 0);
					Move(Position.x + World.size * Chunk.size * World.BlockTilemap.CellSize.x);
				}
				else
				{
					//Position = Position - new Vector2(World.size * Chunk.size * World.BlockTilemap.CellSize.x, 0);
					Move(Position.x - World.size * Chunk.size * World.BlockTilemap.CellSize.x);
				}

				mirrored = false;
			}
		}
	}

	public static Node parent;
	public static int zIndex = -1;
	private float prev_x_viewport;
	public bool mirrored = false;
	
	public int size = 4;
	public Vector2[] corners = new Vector2[4];

	private static bool isInit = false;
	public static bool IsInit => isInit;
	public static void IsInitBuildingTest(string funcName)
	{
		if (!isInit)
			throw new UninitializedException(funcName, "Building");
	}

	/// Place le batiment sur la map
	public void Place(Vector2 location)
	{
		IsInitBuildingTest("Place");
		if (isPlaced)
			return;
		/*if (location.x > World.size * Chunk.size)
		{
			location.x -= World.size * Chunk.size;
		}

		if (location.x < 0)
		{
			location.x += World.size * Chunk.size;
		}*/

		this.location = Convertion.World2Location(location);
		corners = SetCorners(location);
		isPlaced = true;
		ZIndex = zIndex;
		Position = this.location;
		parent.AddChild(this);

		
			
		return;
	}
	///déplace le batiment
	public void Move(float x)
	{
		IsInitBuildingTest("Move");
		this.location.x = x;
		//this.location.x = Convertion.World2Location(location).x;
		corners = SetCorners(Convertion.Location2World(location));
		ZIndex = zIndex;
		Position = this.location;
		
		return;
	}
	
	/// Enleve le batiment de la map
	public void Remove()
	{
		IsInitBuildingTest("Remove");
		if (!isPlaced)
			return;
		this.location = new Vector2(-1,-1);
		isPlaced = false;
		parent.RemoveChild(this);
		return;
	}

	// Détruit le batmiment
	public void Destroy()
	{
		Remove();
		Free();
	}

	/// Donne des dégats à la structure
	public void Damage(int value)
	{
		health -= value;
		if (health<0)
			health = 0;
	}

	/// Donne de la vie a la structure
	public void Healing(int value)
	{
		health += value;
		if (health>healthMax)
			health = healthMax;
	}
	
	private Vector2[] SetCorners(Vector2 location)
	{
		Vector2[] l = new Vector2[4]
		{
			new Vector2(location.x - size / 2, location.y - size / 2),
			new Vector2(location.x - size / 2, location.y + size / 2),
			new Vector2(location.x + size / 2, location.y + size / 2),
			new Vector2(location.x + size / 2, location.y - size / 2)
		};
		return l;
	}

}
