using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;

public class LanDiscoveryClient : MonoBehaviour{
	#region public instance properties
	public int remotePort = 19784;
	#endregion

	#region protected instance fields
	protected UdpClient receiver;
	#endregion

	#region public instance methods
	public void StartReceivingIP ()
	{
		try {
			if (this.receiver == null) {
				this.receiver = new UdpClient (remotePort);
				this.receiver.BeginReceive (new AsyncCallback (ReceiveData), null);
			}
		} catch (SocketException e) {
			Debug.Log (e.Message);
		}
	}
	#endregion

	#region protected instance methods
	protected void ReceiveData(IAsyncResult result){
		//IPEndPoint receiveIPGroup = new IPEndPoint(IPAddress.Any, remotePort);

		//byte[] received;
		if (receiver != null) {
			//received = receiver.EndReceive (result, ref receiveIPGroup);
		} else {
			return;
		}

		this.receiver.BeginReceive(new AsyncCallback(this.ReceiveData), null);
		//string receivedString = Encoding.ASCII.GetString (received);
	}
	#endregion
}




