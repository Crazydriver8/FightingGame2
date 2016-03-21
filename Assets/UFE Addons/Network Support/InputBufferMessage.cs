using System;
using System.IO;
using System.Text;

public class InputBufferMessage : NetworkMessage<FrameInput[]>{
	#region public override methods
	public InputBufferMessage(int playerIndex, long currentFrame, FrameInput[] data) : 
	base(NetworkMessageType.InputBuffer, playerIndex, currentFrame, data){}
	
	public InputBufferMessage(byte[] serializedNetworkMessage) : base(serializedNetworkMessage){
		if (this.MessageType != NetworkMessageType.InputBuffer){
			throw new System.FormatException(string.Format(
				"The message type was {0}, but it should have been {1}.",
				this.MessageType,
				NetworkMessageType.InputBuffer
			));
		}
	}
	#endregion

	#region protected override methods
	protected override void AddToStream(BinaryWriter writer, FrameInput[] buffer){
		writer.Write (buffer != null ? buffer.Length : 0);
		for (int i = 0; i < buffer.Length; ++i){
			writer.Write (buffer[i].horizontalAxis);
			writer.Write (buffer[i].horizontalAxisRaw);
			writer.Write (buffer[i].verticalAxis);
			writer.Write (buffer[i].verticalAxisRaw);
			writer.Write ((int)buffer[i].buttons);
			writer.Write (buffer[i].selectedOption);
		}
	}

	protected override FrameInput[] ReadFromStream(BinaryReader reader){
		FrameInput[] buffer = new FrameInput[reader.ReadInt32()];
		for (int i = 0; i < buffer.Length; ++i){
			buffer[i].horizontalAxis = reader.ReadSingle();
			buffer[i].horizontalAxisRaw = reader.ReadSingle();
			buffer[i].verticalAxis = reader.ReadSingle();
			buffer[i].verticalAxisRaw = reader.ReadSingle();
			buffer[i].buttons = (NetworkButtonPress)reader.ReadInt32();
			buffer[i].selectedOption = reader.ReadInt32();
		}
		
		return buffer;
	}
	#endregion

	#region public override methods
	public override string ToString (){
		StringBuilder sb = new StringBuilder();
		sb.Append("{");
		for (int i = 0; i < this.Data.Length; ++i){
			if (sb.Length > 0){
				sb.Append(", ");
			}

			sb.Append(this.Data[i].ToString());
		}
		sb.Append("}");

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