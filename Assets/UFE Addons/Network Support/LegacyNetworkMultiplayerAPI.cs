using UnityEngine;
//LOOK IN MULTIPLAYERAPI.CS FOR THINGS TO OVERRIDE
public class LegacyNetworkMultiplayerAPI : MultiplayerAPI{
	#region protected instance fields
	protected int _connectionId = -1;
	protected int _hostId = -1;
	protected NetworkPeerType? _previousPeerType = null;
	protected int _unreliableChannel = -1;
	#endregion

	#region public override properties
	public override int Connections{
    //public int Connections { 
		get{
			return Network.connections.Length;
		}
	}

	public override NetworkPlayer NetworkPlayer{
    //public NetworkPlayer NetworkPlayer { 
		get{
			return Network.player;
		}
	}
	
	public override float SendRate{
    //public float SendRate { 
		get{
			return Network.sendRate;
		}
		set{
			Network.sendRate = value;
		}
	}
	#endregion

	#region public override methods
	public override void Connect(string ip, int port){
    //public void Connect(string ip, int port) { 
		NetworkConnectionError error = Network.Connect(ip, port);

		if (error != NetworkConnectionError.NoError){
			//this.RaiseOnServerConnectionError(error);
		}
	}

	public override bool Disconnect(){
    //public bool Disconnect() { 
		Network.Disconnect();
		return true;
	}

	public override void FindLanGames(){
    //public void FindLanGames() { 
		//this.RaiseOnLanGamesDiscoveryError(NetworkConnectionError.ConnectionFailed);
	}

	public override NetworkState GetConnectionState(){
    //public NetworkState GetConnectionState() { 
        NetworkPeerType peerType = Network.peerType;
		if (peerType == NetworkPeerType.Client){
			return NetworkState.Client;
		}else if (peerType == NetworkPeerType.Server){
			return NetworkState.Server;
		}else{
			return NetworkState.Disconnected;
		}
	}

	public override void StartServer(int maxConnections, int listenPort, bool useNat){
    //public void StartServer(int maxConnections, int listenPort, bool useNat) {
        NetworkConnectionError error = Network.InitializeServer(maxConnections, listenPort, useNat);

		if (error != NetworkConnectionError.NoError){
			this.RaiseOnServerInitializationError(error);
		}
	}

    public override bool StopServer(){
    //public bool StopServer() { 
        Network.Disconnect();
		return true;
	}
    #endregion

    #region protected override methods
    protected override bool SendNetworkMessage(byte[] bytes){
    //protected bool SendNetworkMessage(byte[] bytes){
        this.GetComponent<NetworkView>().RPC("Legacy", RPCMode.Others, bytes);
		return true;
	}
	#endregion

	#region protected instance methods
	[RPC]
	public virtual void Legacy(byte[] bytes, NetworkMessageInfo msgInfo){
		//this.RaiseOnMessageReceived(bytes, msgInfo);
	}
	#endregion
	
	#region protected MonoBehaviour methods
	protected virtual void OnConnectedToServer(){
		//this.RaiseOnServerConnectionSuccess();
	}
	
	protected virtual void OnDisconnectedFromServer(NetworkDisconnection info){
		//this.RaiseOnDisconnection(info);
	}
	
	protected virtual void OnPlayerConnected(NetworkPlayer player){
		//this.RaiseOnPlayerConnectedToServer(player);
	}
	
	protected virtual void OnPlayerDisconnected(NetworkPlayer player){
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
		//this.RaiseOnPlayerDisconnectedFromServer(player);
	}
	
	protected virtual void OnServerInitialized(){
		//this.RaiseOnServerStarted();
	}
	
	protected virtual void Update(){
		if(
			Network.peerType == NetworkPeerType.Disconnected &&
			this._previousPeerType != null &&
			this._previousPeerType != NetworkPeerType.Disconnected
		){
			//this.RaiseOnServerStopped();
		}
		
		this._previousPeerType = Network.peerType;
	}
	#endregion
}
