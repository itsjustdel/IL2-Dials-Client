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
	
	public bool autoScan = false;
	public bool hostFound;

	public int portNumber = 11200;
	public bool waitingOnResponse;
	#region private members 	
	private TcpClient socketConnection; 	
	private Thread clientReceiveThread;

	public string hostName;
	public int ip4;
	public int ip3;

	const int socketTimeoutTime = 5;
	float timer = socketTimeoutTime;

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
		//	ConnectToTcpServer();     
		//SendMessage();
	}  	
	// Update is called once per frame
	void Update () {         
		if (Input.GetKeyDown(KeyCode.Space)) {             
			SendMessage();         
		}     
	}

    private void FixedUpdate()
    {
	//	if(!waitingOnResponse)
		//if(socketConnection.Connected)
			SendMessage();
	}
    /// <summary> 	
    /// Setup socket connection. 	
    /// </summary> 	
    private void ConnectToTcpServer () { 		
		try {
			Debug.Log("Looking for server");

			if (autoScan && !hostFound)
			{
				//check if anything  on socket

				hostName = "192.168." + ip3.ToString() + "." + ip4.ToString();
				Thread thread = new Thread(() => ListenForData(hostName));
				thread.IsBackground = true;
				thread.Start();//does this close automatically?

				//push to 255 and move ip3 up
				ip4++;
				if (ip4 > 255)
				{
					ip3++;
					ip4 = 0;
				}

				if (ip3 > 255)
				{
					Debug.Log("Did not find server");

					enabled = false;
				}
			}

			else
            {
				//use value entered by user in hostName
				Thread thread = new Thread(() => ListenForData(hostName));
				thread.IsBackground = true;
				thread.Start();//does this close automatically?
			}


		} 		
		catch (Exception e) { 			
			Debug.Log("On client connect exception " + e); 		
		} 	
	}  	
	/// <summary> 	
	/// Runs in background clientReceiveThread; Listens for incomming data. 	
	/// </summary>     
	private void ListenForData(string hostName) { 		
		try { 			
			socketConnection = new TcpClient(hostName, portNumber);
			//array to read stream in to
			Byte[] bytes = new Byte[1024];             
					
			using (NetworkStream stream = socketConnection.GetStream()) 
			{
				//is we have a network stream, save this hostname
				hostFound = true;

				int length; 					
				// Read incomming stream into byte arrary. 					
				while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) {

					Debug.Log("Reading received data");

					float[] floats= GetFloats(bytes);

					//set Il2 game data for client
					iL2GameDataClient.altitude = floats[0];
					iL2GameDataClient.mmhg = floats[1];
					iL2GameDataClient.airspeed = floats[2];

					Debug.Log("altitude = " + floats[0]);
					Debug.Log("mmhg = " + floats[1]);
					Debug.Log("airspeed = " + floats[2]);

						

				}		
			}
		}         
		catch (Exception ex) {
			socketConnection = null;
			//clientReceiveThread.Abort();
			Debug.Log("couldn't read");
			Debug.Log("Socket exception: " + ex);
			//ConnectToTcpServer();
		}     
	}  	
	/// <summary> 	
	/// Send message to server using socket connection. 	
	/// </summary> 	
	public void SendMessage() {
		//Debug.Log("send client");
		if (socketConnection == null || !socketConnection.Connected) 
		{
			if (timer > socketTimeoutTime)//timer value of timeout socket setting on server
			{
				Debug.Log("Attempting to connect to server");
				timer = 0f;
				ConnectToTcpServer();
			}
			else 
				timer += Time.deltaTime;

			return;
		}		  		
		try 
		{
			
			// Get a stream object for writing. 			
			NetworkStream stream = socketConnection.GetStream(); 			
			if (stream.CanWrite) {
				//string clientMessage = "This is a message from one of your clients."; 				
				// Convert string message to byte array.   
				
				//evcode 0x01 - instrument data
				byte[] clientMessageAsByteArray = new byte[1]{ 0x01 };// Encoding.ASCII.GetBytes(clientMessage); 				
				// Write byte array to socketConnection stream.                 
				stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);                 
				Debug.Log("Client sent his message - should be received by server");

				waitingOnResponse = true;
			}         
		} 		
		catch (Exception ex) 
		{
			//socketConnection = null;
			Debug.Log("Need to Reconnect");
			Debug.Log("Socket exception: " + ex);         
		}     
	}

	static float[] GetFloats(byte[] bytes)
	{
		var result = new float[bytes.Length / sizeof(float)];
		Buffer.BlockCopy(bytes, 0, result, 0, bytes.Length);
		return result;
	}
}