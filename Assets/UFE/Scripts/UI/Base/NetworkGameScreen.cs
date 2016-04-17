﻿using UnityEngine;
using System.Collections;
using System;
using System.Net;

public class NetworkGameScreen : UFEScreen{
	public virtual void GoToMainMenu(){
		UFE.StartMainMenuScreen();
	}

	public virtual void GoToHostGameScreen(){
		UFE.StartHostGameScreen();
	}

	public virtual void GoToJoinGameScreen(){
		UFE.StartJoinGameScreen();
	}

	public virtual string GetIp() {
		string hostName = System.Net.Dns.GetHostName();
		IPHostEntry ipHostEntry = System.Net.Dns.GetHostEntry(hostName);
		IPAddress[] ipAddresses = ipHostEntry.AddressList;
		
		return ipAddresses[ipAddresses.Length - 1].ToString();
	}
    public virtual void CopyIP()
    {
        TextEditor te = new TextEditor();
        string hostName = System.Net.Dns.GetHostName();
        IPHostEntry ipHostEntry = System.Net.Dns.GetHostEntry(hostName);
        IPAddress[] ipAddresses = ipHostEntry.AddressList;
        string address =  ipAddresses[ipAddresses.Length - 1].ToString();
        Debug.Log(address);
        te.text = address;
        te.SelectAll();
        te.Copy();
    }
}
