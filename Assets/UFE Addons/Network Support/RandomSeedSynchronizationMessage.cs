using System;
using System.IO;

public class RandomSeedSynchronizationMessage : NetworkMessage<int>{
	#region public override methods
	public RandomSeedSynchronizationMessage(int playerIndex, long currentFrame, int randomSeed) : 
	base(NetworkMessageType.RandomSeedSynchronization, playerIndex, currentFrame, randomSeed){}
	
	public RandomSeedSynchronizationMessage(byte[] serializedNetworkMessage) : base(serializedNetworkMessage){
		if (this.MessageType != NetworkMessageType.RandomSeedSynchronization){
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