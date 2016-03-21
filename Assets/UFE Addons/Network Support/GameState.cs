using UnityEngine;
using System;
using System.IO;

[Serializable]
public struct GameState : IEquatable<GameState>{
	#region public instance properties
	public Vector3 p1Position;
	public Vector3 p2Position;
	#endregion

	#region public instance constructors
	public GameState(Vector3 p1Position, Vector3 p2Position){
		this.p1Position = p1Position;
		this.p2Position = p2Position;
	}
	#endregion

	#region IEquatable<GameState> implementation
	public override bool Equals(object obj){
		return (obj is GameState) && this.Equals((GameState)obj);
	}

	public override int GetHashCode (){
		unchecked{
			return 11 * this.p1Position.GetHashCode() + 13 * this.p2Position.GetHashCode();
		}
	}

	public bool Equals(GameState other){
		return this.p1Position.Equals(other.p1Position) && this.p2Position.Equals(other.p2Position);
	}
	#endregion

	#region public instance methods
	public byte[] Serialize(){
		return GameState.Serialize(this);
	}
	#endregion

	#region public override methods
	public override string ToString (){
		return string.Format("[GameState | p1Position = {0} | p2Position = {1}]", this.p1Position, this.p2Position);
	}
	#endregion

	#region public class methods
	public static void AddToStream(BinaryWriter writer, GameState gameState){
		writer.Write(gameState.p1Position.x);
		writer.Write(gameState.p1Position.y);
		writer.Write(gameState.p1Position.z);
		writer.Write(gameState.p2Position.x);
		writer.Write(gameState.p2Position.y);
		writer.Write(gameState.p2Position.z);
	}


	public static GameState Deserialize(byte[] bytes){
		using (MemoryStream stream = new MemoryStream(bytes)){
			using (BinaryReader reader = new BinaryReader(stream)){
				return GameState.ReadFromStream(reader);
			}
		}
	}

	public static GameState ReadFromStream(BinaryReader reader){
		return new GameState(
			new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
			new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle())
		);
	}

	public static byte[] Serialize(GameState gameState){
		using (MemoryStream stream = new MemoryStream()){
			using (BinaryWriter writer = new BinaryWriter(stream)){
				GameState.AddToStream(writer, gameState);
				writer.Flush();
				return stream.ToArray();
			}
		}
	}
	#endregion
}
