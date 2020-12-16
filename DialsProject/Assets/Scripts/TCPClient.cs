using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCPClient : MonoBehaviour {

	//class which reads memory and sets values
	//have seperate for server and clietn so i can test on same pc
	
	public BuildControl buildControl;
	public ReadGameData iL2GameDataClient;
	//user settings	
	public int portNumber = 11200;

	#region private members 	
	private TcpClient socketConnection; 	
	private Thread clientReceiveThread; 	
	#endregion  	
	// Use this for initialization 	

	void Awake()
    {
		if(buildControl.isServer)
        {
			//disable client script
			enabled = false;

			
        }
    }
	void Start () {
		ConnectToTcpServer();     
	}  	
	// Update is called once per frame
	void Update () {         
		if (Input.GetKeyDown(KeyCode.Space)) {             
			SendMessage();         
		}     
	}  	
	/// <summary> 	
	/// Setup socket connection. 	
	/// </summary> 	
	private void ConnectToTcpServer () { 		
		try {  			
			clientReceiveThread = new Thread (new ThreadStart(ListenForData)); 			
			clientReceiveThread.IsBackground = true; 			
			clientReceiveThread.Start();  		
		} 		
		catch (Exception e) { 			
			Debug.Log("On client connect exception " + e); 		
		} 	
	}  	
	/// <summary> 	
	/// Runs in background clientReceiveThread; Listens for incomming data. 	
	/// </summary>     
	private void ListenForData() { 		
		try { 			
			socketConnection = new TcpClient("192.168.1.75", 11200);  			
			//array to read stream in to
			Byte[] bytes = new Byte[1024];             
			while (true) { 				
				// Get a stream object for reading 				
				using (NetworkStream stream = socketConnection.GetStream()) { 					
					int length; 					
					// Read incomming stream into byte arrary. 					
					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) {					

						float[] floats= GetFloats(bytes);

						//set Il2 game data for client
						iL2GameDataClient.altitude = floats[0];
						iL2GameDataClient.mmhg = floats[1];
						iL2GameDataClient.airspeed = floats[2];


					} 				
				} 			
			}         
		}         
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		}     
	}  	
	/// <summary> 	
	/// Send message to server using socket connection. 	
	/// </summary> 	
	public void SendMessage() {
		Debug.Log("send client");
		if (socketConnection == null) {             
			return;         
		}  		
		try { 			
			// Get a stream object for writing. 			
			NetworkStream stream = socketConnection.GetStream(); 			
			if (stream.CanWrite) {                 
				string clientMessage = "This is a message from one of your clients."; 				
				// Convert string message to byte array.                 
				byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage); 				
				// Write byte array to socketConnection stream.                 
				stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);                 
				Debug.Log("Client sent his message - should be received by server");             
			}         
		} 		
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		}     
	}

	static float[] GetFloats(byte[] bytes)
	{
		var result = new float[bytes.Length / sizeof(float)];
		Buffer.BlockCopy(bytes, 0, result, 0, bytes.Length);
		return result;
	}
}