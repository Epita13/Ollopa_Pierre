using Godot;
using System;

public class Block
{
	public enum Type 
	{
		Air = -1,
		Stone = 0,
		Grass = 1,
		Dirt = 2
	}
	public static int GetIDTile(Type type)
	{
		return (int) type;
	}


	/*
		Object :  Block

		/!\ Initialisation static : NON NECESSAIRE (Ps : A mettre a jour).
		/!\ Classe Initialisées necessaire : None

		Description de l'object :
			Un block est comme sont nom l'indique un block du monde.
			Un block d'Air est egalement un block.
			Chaque type de block posseble un id correspondant a l'id dans le tileset (a mettre a jour dans l'enumeration Type).
			Les coordonnées manipulées dans cette classe sont strictement globale (World).

		Description des parametres:
			int x,y : sont la position du block dans le Monde.
			Block.Type type : est le type de block correspondant au block lui meme
			bool isAutoGenerated : True si le block est issue de la generation automatique (et le reste meme si le type de block change)
								   False sinon.
	*/

	public int x,y;
	public Type type;
	public bool isAutoGenerated;

	public Block(Type type, int x, int y, bool isAutoGenerated=false){
		this.x = x;
		this.y = y;
		this.type = type;
		this.isAutoGenerated = isAutoGenerated;
	}

	



	/*Static Fonctions*/
	public static bool IsAir(Block b) => b.type==Block.Type.Air;
	public static bool IsNotAir(Block b) => b.type!=Block.Type.Air;


}