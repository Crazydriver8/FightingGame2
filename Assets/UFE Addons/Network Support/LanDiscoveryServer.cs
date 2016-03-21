using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class LanDiscoveryServer : MonoBehaviour {
	#region public instance properties
	public float timeBetweenBroadcasts = 5f;
	public int remotePort = 19784;

	protected int localPort = 1234; // ???
	#endregion

	#region protected instance fields
	protected UdpClient sender;
	#endregion

	#region MonoBehaviour methods
	protected virtual void Start(){
		this.sender = new UdpClient (localPort, AddressFamily.InterNetwork);
		IPEndPoint groupEP = new IPEndPoint(IPAddress.Broadcast, remotePort);
		this.sender.Connect (groupEP);
		
		//SendData ();
		InvokeRepeating("SendData", 0f, this.timeBetweenBroadcasts);
	}
	#endregion

	#region protected instance methods
	protected virtual void SendData (){
		byte[] message = new byte[0];
		this.sender.Send(message, message.Length);
	}
	#endregion
}




