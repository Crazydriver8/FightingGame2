using System;
using System.IO;

public class RandomSeedSynchronizedMessage : NetworkMessage<int>{
	#region public override methods
	public RandomSeedSynchronizedMessage(int playerIndex, long currentFrame, int randomSeed) : 
	base(NetworkMessageType.RandomSeedSynchronized, playerIndex, currentFrame, randomSeed){}
	
	public RandomSeedSynchronizedMessage(byte[] serializedNetworkMessage) : base(serializedNetworkMessage){
		if (this.MessageType != NetworkMessageType.RandomSeedSynchronized){
			throw new System.FormatException(string.Format(
				"The message type was {0}, but it should have been {1}.",
				this.MessageType,
				NetworkMessageType.RandomSeedSynchronization
			));
		}
	}
	#endregion

	#region protected override methods
	protected override void AddToStream(BinaryWriter writer, int data){
		writer.Write(data);
	}

	protected override int ReadFromStream(BinaryReader reader){
		return reader.ReadInt32();
	}
	#endregion
}