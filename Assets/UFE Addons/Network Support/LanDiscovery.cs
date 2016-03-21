using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class LanDiscovery : MonoBehaviour{
	private int server_port = 5000;
	private string server_ip;


	// multicast
	private int startup_port = 5100;
	private IPAddress group_address = IPAddress.Parse ("224.0.0.224");
	private UdpClient udp_client;
	private IPEndPoint remote_end;



	public bool isServer = true;

	#region MonoBehaviour methods
	protected virtual void Start(){
		// loaded elsewhere
		if (this.isServer){
			this.StartGameServer();
		}else{
			this.StartGameClient();
		}
	}
	#endregion

	#region public instance methods
	public virtual void StartGameServer(){
		// the Unity3d way to become a server
		NetworkConnectionError init_status = Network.InitializeServer (10, server_port, false);
		Debug.Log ("status: " + init_status);
		
		this.StartBroadcast();
	}
	
	public virtual void StartGameClient(){
		// multicast receive setup
		this.remote_end = new IPEndPoint(IPAddress.Any, startup_port);
		this.udp_client = new UdpClient (remote_end);
		this.udp_client.JoinMulticastGroup (group_address);
		
		// async callback for multicast
		udp_client.BeginReceive (new AsyncCallback (ServerLookup), null);

		this.StartCoroutine(this.MakeConnection());
	}




	/******* client functions *******/
	public virtual IEnumerator MakeConnection(){
		// continues after we get server's address
		while (this.server_ip == null){
			yield return null;
		}
		
		while (Network.peerType == NetworkPeerType.Disconnected){
			Debug.Log ("connecting: " + this.server_ip +":"+ this.server_port);
			
			// the Unity3d way to connect to a server
			NetworkConnectionError error = Network.Connect (this.server_ip, this.server_port);
			
			Debug.Log ("status: " + error);
			yield return new WaitForSeconds (1);
		}
	}
	
	/******* broadcast functions *******/
	public virtual void ServerLookup(IAsyncResult ar){
		// receivers package and identifies IP
        // byte[] receiveBytes = this.udp_client.EndReceive(ar, ref this.remote_end);
		
		this.server_ip = remote_end.Address.ToString ();
		Debug.Log ("Server: " + this.server_ip);
	}
	
	public virtual IEnumerator StartBroadcast(){
		// multicast send setup
		this.udp_client = new UdpClient();
		this.udp_client.JoinMulticastGroup (group_address);
		this.remote_end = new IPEndPoint(group_address, startup_port);
		
		// sends multicast
		while (true)
		{
			byte[] buffer = Encoding.ASCII.GetBytes ("GameServer");
			this.udp_client.Send(buffer, buffer.Length, remote_end);
			
			yield return new WaitForSeconds (1);
		}
	}
	#endregion
}




