using System;
using System.IO;
using System.Text;

public class SynchronizationMessage : NetworkMessage<GameState>{
	#region public override methods
	public SynchronizationMessage(int playerIndex, long currentFrame, GameState data) : 
	base(NetworkMessageType.Syncronization, playerIndex, currentFrame, data){}
	
	public SynchronizationMessage(byte[] serializedNetworkMessage) : base(serializedNetworkMessage){
		if (this.MessageType != NetworkMessageType.Syncronization){
			throw new System.FormatException(string.Format(
				"The message type was {0}, but it should have been {1}.",
				this.MessageType,
				NetworkMessageType.Syncronization
			));
		}
	}
	#endregion

	#region protected override methods
	protected override void AddToStream(BinaryWriter writer, GameState gameState){
		GameState.AddToStream(writer, gameState);
	}

	protected override GameState ReadFromStream(BinaryReader reader){
		return GameState.ReadFromStream(reader);
	}
	#endregion

	#region public override methods
	public override string ToString (){
		StringBuilder sb = new StringBuilder();

		sb	.Append("{")
			.Append("\"p1\"=\"")
			.Append(this.Data.p1Position)
			.Append("\", \"p2\"=\"")
			.Append(this.Data.p1Position)
			.Append("\"}");

		return string.Format(
			"[{0} | messageType = {1} | playerIndex = {2} | currentFrame = {3} | data = {4}]",
			this.GetType().ToString(),
			this.MessageType,
			this.PlayerIndex,
			this.CurrentFrame,
			sb.ToString()
		);
	}
	#endregion
}