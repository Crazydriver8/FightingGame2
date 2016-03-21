using System;
using System.IO;

[Serializable]
public struct FrameInput{
	#region public instance properties
	public float horizontalAxis;
	public float horizontalAxisRaw;
	public float verticalAxis;
	public float verticalAxisRaw;
	public NetworkButtonPress buttons;
	public int selectedOption;
	#endregion

	#region public instance constructors
	public FrameInput(
		float horizontalAxis, 
		float horizontalAxisRaw, 
		float verticalAxis, 
		float verticalAxisRaw, 
		NetworkButtonPress buttons,
		int selectedOption = -1
	){
		this.horizontalAxis = horizontalAxis;
		this.horizontalAxisRaw = horizontalAxisRaw;
		this.verticalAxis = verticalAxis;
		this.verticalAxisRaw = verticalAxisRaw;
		this.buttons = buttons;
		this.selectedOption = selectedOption;
	}

	public FrameInput(FrameInput other) : this(
		other.horizontalAxis,
		other.horizontalAxisRaw,
		other.verticalAxis,
		other.verticalAxisRaw,
		other.buttons,
		other.selectedOption
	){}
	#endregion

	#region public instance methods
	public byte[] Serialize(){
		return FrameInput.Serialize(this);
	}
	#endregion

	#region public override methods
	public override string ToString (){
		return string.Format(
			"[FrameInput | horizontalAxis = {0} | horizontalAxisRaw = {1} | verticalAxis = {2} | verticalAxisRaw = {3} | buttons = {4} | selected option = {5}]",
			this.horizontalAxis,
			this.horizontalAxisRaw,
			this.verticalAxis,
			this.verticalAxisRaw,
			buttons,
			selectedOption
		);
	}
	#endregion

	#region public class methods
	public static void AddToStream(BinaryWriter writer, FrameInput frameInput){
		writer.Write(frameInput.horizontalAxis);
		writer.Write(frameInput.horizontalAxisRaw);
		writer.Write(frameInput.verticalAxis);
		writer.Write(frameInput.verticalAxisRaw);
		writer.Write((int)frameInput.buttons);
		writer.Write(frameInput.selectedOption);
	}


	public static FrameInput Deserialize(byte[] bytes){
		using (MemoryStream stream = new MemoryStream(bytes)){
			using (BinaryReader reader = new BinaryReader(stream)){
				return FrameInput.ReadFromStream(reader);
			}
		}
	}

	public static FrameInput ReadFromStream(BinaryReader reader){
		float horizontalAxis = reader.ReadSingle();
		float horizontalAxisRaw = reader.ReadSingle();
		float verticalAxis = reader.ReadSingle();
		float verticalAxisRaw = reader.ReadSingle();
		NetworkButtonPress buttons = (NetworkButtonPress)reader.ReadInt32();
		int selectedOption = reader.ReadInt32();

		return new FrameInput(
			horizontalAxis,
			horizontalAxisRaw,
			verticalAxis,
			verticalAxisRaw,
			buttons,
			selectedOption
		);
	}

	public static byte[] Serialize(FrameInput frameInput){
		using (MemoryStream stream = new MemoryStream()){
			using (BinaryWriter writer = new BinaryWriter(stream)){
				FrameInput.AddToStream(writer, frameInput);
				writer.Flush();
				return stream.ToArray();
			}
		}
	}
	#endregion
}
