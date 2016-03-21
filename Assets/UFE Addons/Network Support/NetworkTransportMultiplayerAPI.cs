//using UnityEngine;
//using UnityEngine.Networking;
//
//public class NetworkTransportMultiplayerAPI : MultiplayerAPI{
//	#region protected instance fields
//	protected int _connectionId = -1;
//	protected int _hostId = -1;
//	protected int _unreliableChannel = -1;
//	#endregion
//
//	#region public override methods
//	public override void Connect(string ip, int port){
//		byte error;
//		ConnectionConfig config = new ConnectionConfig();
//		this._unreliableChannel = config.AddChannel(QosType.StateUpdate);
//
//		HostTopology topology = new HostTopology(config, 1);
//		this._hostId = NetworkTransport.AddHost(topology, port);
//		this._connectionId = NetworkTransport.Connect(this._hostId, ip, port, 0, out error);
//
//		if (error == (byte)NetworkError.Ok){
//			// TODO: report the operation as successful
//		}else{
//			// TODO: report the operation as failure
//		}
//	}
//
//	public override bool Disconnect(){
//		byte error;
//		return NetworkTransport.Disconnect(this._hostId, this._connectionId, out error);
//	}
//
//	public override bool Initialize (string uuid){
//		bool ok = base.Initialize(uuid);
//
//		if (ok){
//			NetworkTransport.Init();
//		}
//
//		return ok;
//	}
//
//	public override bool SendSocketMessage(byte[] bytes){
//		byte error;
//
//		return NetworkTransport.Send(
//			this._hostId, 
//			this._connectionId, 
//			this._unreliableChannel, 
//			bytes, 
//			bytes.Length, 
//			out error
//		);
//	}
//
//	public override void StartServer(int maxConnections, int listenPort, bool useNat){
//		ConnectionConfig config = new ConnectionConfig();
//		this._unreliableChannel = config.AddChannel(QosType.StateUpdate);
//
//		HostTopology topology = new HostTopology(config, maxConnections);
//		this._hostId = NetworkTransport.AddHost(topology, listenPort);
//	}
//
//	public override bool StopServer(){
//		bool ok = NetworkTransport.RemoveHost(this._hostId);
//
//		if (ok){
//			this._hostId = -1;
//			this._connectionId = -1;
//			this._unreliableChannel = 1;
//		}
//
//		return ok;
//	}
//	#endregion
//
//	#region protected MonoBehaviour methods
//	protected override void Update(){
//		int remoteHostId;
//		int remoteConnectionId;
//		int remoteChannelId;
//		int dataSize;
//		byte error;
//
//		int bufferSize = 1024;
//		byte[] buffer = new byte[1024];
//
//		NetworkEventType networkEventType = NetworkTransport.Receive(
//			out remoteHostId, 
//			out remoteConnectionId, 
//			out remoteChannelId, 
//			buffer, 
//			bufferSize, 
//			out dataSize, 
//			out error
//		);
//
//
//
//		switch (networkEventType) {
//		case NetworkEventType.Nothing:
//			break;
//		case NetworkEventType.ConnectEvent:
//			//Debug.Log("incoming connection event received");
//			break;
//		case NetworkEventType.DataEvent:
//			//Debug.Log("incoming message event received: " + message);
//			break;
//		case NetworkEventType.DisconnectEvent:
//			//Debug.Log("remote client event disconnected");
//			break;
//		}
//	}
//	#endregion
//}
