using System;
//using System.Collections;
//using System.Collections.Generic;
using System.Net.Sockets;
//using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Android;
public class TCPClient : MonoBehaviour {

	//class which reads memory and sets values
	//have seperate for server and clietn so i can test on same pc
	
	public BuildControl buildControl;
	public AirplaneData iL2GameDataClient;

	public MenuHandler menuHandler;
	public RotateNeedle rN;
	//user settings	

	public bool connected = false;
	public bool autoScan = false;
	public bool hostFound;
	public bool tcpReceived = false;

	//user can insert from menu, if empty, autoscan happens
	public string userIP;
	//user can overwrite this
	public int portNumber = 11200;
	public bool waitingOnResponse;

	
	public float timerOfLastReceived = 0f;
	public bool testPrediction = false;

	#region private members 	
	private TcpClient socketConnection; 	
	

	public string hostName;
	public int ip4;
	public int ip3;

	public int socketTimeoutTime = 5;	
	public float timer = 5f;
	public float connectionTimer = 0f;


	bool localScanAttempted = false;//127.0.0.1 internal loopback scan

	#endregion



	void Awake()
    {
		if (buildControl.isServer)
        {
			//disable client script
			enabled = false;
        }		
    }

    public void FixedUpdate()
	{
		//wait for as econd before scanning
		if (menuHandler.stopwatch.ElapsedMilliseconds < 5000)
			return;


		if(hostFound)
        {
			if(!autoScan)
				menuHandler.scanDebug.GetComponent<Text>().text = "Connected: " + userIP.ToString() + " : " + portNumber; //causes exception - because of async?
			else
				menuHandler.scanDebug.GetComponent<Text>().text = "Connected: " + hostName.ToString() + " : " + portNumber; //causes exception
		}

		if (menuHandler.ipFieldOpen || menuHandler.portFieldOpen)
        {
			//wait til input is finished if scanning. cpu high 
			//Debug.Log("waiting on ip input");
			return;
        }

		//check if we should autoscan
		//if ipaddress is empty, then we should
		if (string.IsNullOrEmpty( userIP))
		{
			autoScan = true;
			//if we are autoscanning, we don't need to use socket timeout time
			//socketTimeoutTime = 5;
		}
		else
		{
			autoScan = false;
			//if we are not autoscanning, set the the timeout time to 5 so we don't bombard the server
		//	socketTimeoutTime = 5;
			//remove autoscan debug text field
			//menuHandler.scanDebug.GetComponent<Text>().text = null;

		}
		

		SendMessage();
	}
    /// <summary> 	
    /// Setup socket connection. 	
    /// </summary> 	
    private void ConnectToTcpServer () 
	{ 		
		try {
			

			if (autoScan && !hostFound)
			{
				//Debug.Log("Looking for server");
				//check if anything  on socket

				//look for local connection before going to wifi
				if (!localScanAttempted)
				{
					hostName = "127.0.0.1";
				}
				else
				{
					hostName = "192.168." + ip3.ToString() + "." + ip4.ToString();
				}

				Thread thread = new Thread(() => ListenForData(hostName));
				thread.IsBackground = true;
				thread.Start();//does this close automatically?

				//let user know we are scanning	
				if(autoScan)
					menuHandler.scanDebug.GetComponent<Text>().text = "Scanning IP: " + hostName.ToString(); ;

				if (localScanAttempted)
				{
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


						//restart -if people have strange ips they probably know about it and can use direct connection option
						ip3 = 0;
						ip4 = 0;

					}
				}

				//first frame we attempt local scan, then on to wifi
				localScanAttempted = true;

			}

			else
            {
				Debug.Log("Starting new thread");

				menuHandler.scanDebug.GetComponent<Text>().text = "Attempting Connection: " + userIP.ToString() +" : " + portNumber ;
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

				//unpack received stream
				//program version float
				//planetype string size
				//planetype string data size
				//float array containing instrument/dial values

				int length; 					
				// Read incoming stream into byte arrary. 					
				while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) 
				{

					int p = 0;

					//set length sent from server
					int floatArrayLength = 4 * 8;
					//float array
					float[] floats = GetFloats(bytes, p, floatArrayLength);

					//set Il2 game data for client
					iL2GameDataClient.altitude = floats[0];
					iL2GameDataClient.mmhg = floats[1];
					iL2GameDataClient.airspeed = floats[2];
					iL2GameDataClient.heading = floats[3];
					iL2GameDataClient.pitch = floats[4];
					iL2GameDataClient.roll = floats[5];
					iL2GameDataClient.verticalSpeed = floats[6];
					iL2GameDataClient.turnCoordinatorBall = floats[7];
					p += floatArrayLength;

					//Debug.Log("Reading received data");
					//version number
					//receiving server version from stream (server -> client)
					iL2GameDataClient.serverVersion = BitConverter.ToSingle(bytes,p);
					


					//Debug.Log("Version Number = " + versionNumber);
					p += sizeof(float);

					//plane type string size
					uint stringSize = BitConverter.ToUInt32(bytes,p);
					p += sizeof(uint);
					//plane type string
					string planeType = System.Text.Encoding.UTF8.GetString(bytes, p, (int)stringSize);
					iL2GameDataClient.planeType = planeType;
					p += 64;//chosen max string size (by me)
					
					

					//stopping glitching and setting rest position off centre - move this
					//if (iL2GameDataClient.airspeed < 50 || float.IsNaN( iL2GameDataClient.airspeed )) //do we even want this? why not have needle at 0?
					//iL2GameDataClient.airspeed = 50;
					if (float.IsNaN(iL2GameDataClient.airspeed))
						iL2GameDataClient.airspeed = 0f;

					//Debug.Log("altitude = " + floats[0]);
					//Debug.Log("mmhg = " + floats[1]);
					//Debug.Log("airspeed = " + floats[2]);

						//save rotation of needles				
					if (!testPrediction)
						tcpReceived = true; 

					//keep a track of last receieved time
					timerOfLastReceived = 0f;

					connected = true;
					connectionTimer = 0f;

				//	Debug.Log("Stream Length = " + length);
				}

				
				
			}
		}         
		catch (Exception ex) {


			if (connectionTimer >= socketTimeoutTime)
			{
				

				connected = false;


			}
			//no stream, server might not be sending anything
			//hostFound = false;
			//socketConnection = null;
			//clientReceiveThread.Abort();
			//Debug.Log("couldn't read");
			//Debug.Log("Socket exception: " + ex);
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
			connectionTimer += Time.fixedDeltaTime;

			if (autoScan)
			{
				//this creates a small timer for autoscanning
				float thisTimer = 0f; //.1f;
				if (hostFound)
					//if we already found the host and are looking to reconnect, we need to be careful and use the timeout
					thisTimer = socketTimeoutTime;

				

				if (timer >= thisTimer)//timer value of timeout socket setting on server
				{
					//Debug.Log("Sending to server - autoscan");
					timer = 0f;
					ConnectToTcpServer();
				}
				else
					timer += Time.fixedDeltaTime;

				return;
			}
			else
			{
				//use timer so we don't overload server socket - if not connected
				float thisTimer = 0;
				if (!hostFound)
					thisTimer = socketTimeoutTime;

				if (timer >= thisTimer)//timer value of timeout socket setting on server
				{
					Debug.Log("Sending to server");
					timer = 0f;
					ConnectToTcpServer();
				}
				else
					timer += Time.fixedDeltaTime;

				return;
			}
		}		  		
		try 
		{
			if(timerOfLastReceived >= socketTimeoutTime)
            {
				Debug.Log("last received time out, resetting");
				//stop sending requests to server, and start to look for another server socket. server may have reset
				socketConnection = null;
				
				if(autoScan)
                {
					//restart? or keep last found host
					//ip3 = 0;
					//ip4 =0;
					hostFound = false;
				}
				else
                {
					//set this so reconnecting to server doesn't spam the socket
					hostFound = false;
				}

				
				return;
            }

			// Get a stream object for writing. 			
			NetworkStream stream = socketConnection.GetStream(); 			
			if (stream.CanWrite) {
				//string clientMessage = "This is a message from one of your clients."; 				
				// Convert string message to byte array.   
				
				//evcode 0x01 - instrument data
				byte[] clientMessageAsByteArray = new byte[1]{ 0x01 };// Encoding.ASCII.GetBytes(clientMessage); 				
				// Write byte array to socketConnection stream.                 
				stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);                 
				//Debug.Log("Client sent his message - should be received by server");

				timerOfLastReceived += Time.fixedDeltaTime;

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

	static float[] GetFloats(byte[] bytes, int offset, int floatArrayLength)
	{
		try
		{
			var result = new float[bytes.Length / sizeof(float)];
			Buffer.BlockCopy(bytes, offset, result, 0, floatArrayLength);

			return result;

		}
		catch(Exception ex)
        {
			return null;
        }
	
	}



}