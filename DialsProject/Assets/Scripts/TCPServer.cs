using System;
using System.Collections; 
using System.Collections.Generic; 
using System.Net; 
using System.Net.Sockets; 
using System.Text; 
using System.Threading; 
using UnityEngine;
using System.Linq;

public class TCPServer : MonoBehaviour {

	public BuildControl buildControl;
	public ReadGameData iL2GameDataServer;
	//user can change this in settings - to be implemented
	public int portNumber = 11200;
	
	#region private members 	
	/// <summary> 	
	/// TCPListener to listen for incomming TCP connection 	
	/// requests. 	
	/// </summary> 	
	private TcpListener tcpListener; 
	/// <summary> 
	/// Background thread for TcpServer workload. 	
	/// </summary> 	
	private Thread tcpListenerThread;  	
	/// <summary> 	
	/// Create handle to connected tcp client. 	
	/// </summary> 	
	private TcpClient connectedTcpClient;
	#endregion


	void Awake()
	{
		if (buildControl.isClient)
		{
			//disable server script
			enabled = false;

			
		}
	}

	// Use this for initialization
	void Start () { 		
		// Start TcpServer background thread 		
		tcpListenerThread = new Thread (new ThreadStart(ListenForIncomingRequests)); 		
		tcpListenerThread.IsBackground = true; 		
		tcpListenerThread.Start(); 	
	}  	
	
	// Update is called once per frame
	void Update () { 		
		if (Input.GetKeyDown(KeyCode.Space)) {             
			SendMessage();         
		} 	
	}

    private void FixedUpdate()
    {

		SendMessage();
    }

    /// <summary> 	
    /// Runs in background TcpServerThread; Handles incomming TcpClient requests 	
    /// </summary> 	
    private void ListenForIncomingRequests () { 		
		try { 			
			// Create listener on localhost port 8052. 			
			tcpListener = new TcpListener(IPAddress.Any, portNumber); 			
			tcpListener.Start();              
			Debug.Log("Server is listening");              
			Byte[] bytes = new Byte[1024];  			//how many should be initialised? number of sent bytes * sizeof(double)?
			while (true) { 				
				using (connectedTcpClient = tcpListener.AcceptTcpClient()) { 					
					// Get a stream object for reading 					
					using (NetworkStream stream = connectedTcpClient.GetStream()) { 						
						int length; 						
						// Read incomming stream into byte arrary. 						
						while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) { 							
							var incommingData = new byte[length]; 							
							Array.Copy(bytes, 0, incommingData, 0, length);  							
							// Convert byte array to string message. 							
							string clientMessage = Encoding.ASCII.GetString(incommingData); 							


							Debug.Log("client message received as: " + clientMessage); 		
							
							
						} 					
					} 				
				} 			
			} 		
		} 		
		catch (SocketException socketException) { 			
			Debug.Log("SocketException " + socketException.ToString()); 		
		}     
	}  	
	/// <summary> 	
	/// Send message to client using socket connection. 	
	/// </summary> 	
	public void SendMessage() {
		//Debug.Log("send server");
		if (connectedTcpClient == null) {             
			return;         
		}  		
		
		try { 			
			// Get a stream object for writing. 			
			NetworkStream stream = connectedTcpClient.GetStream(); 			
			if (stream.CanWrite) {
				
				//grab data
				float[] floats = new float[] { iL2GameDataServer.altitude , iL2GameDataServer.mmhg, iL2GameDataServer.airspeed};
							
				//convert to bytes
				byte[] bytes = GetBytes(floats);

				//send to client
				stream.Write(bytes, 0, bytes.Length);               
			//	Debug.Log("Server sent his message - should be received by client");           
			}       
		} 		
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		} 	
	}

	static byte[] GetBytes(float[] values)
	{
		var result = new byte[values.Length * sizeof(float)];
		Buffer.BlockCopy(values, 0, result, 0, result.Length);
		return result;
	}
}