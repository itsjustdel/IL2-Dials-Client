using System;
//using System.Collections;
//using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
//using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

using System.Collections;
//using UnityEngine.Android;
public class UDPClient : MonoBehaviour
{

	//class which reads memory and sets values
	//have seperate for server and clietn so i can test on same pc

	public BuildControl buildControl;
	public AirplaneData iL2GameDataClient;

	public MenuHandler menuHandler;
	public RotateNeedle rN;

	//user settings		

	public bool connected = false;
	public float autoScanTimeScale = 1f;
	public float standardFixedTime = 0.02f;
	public bool autoScan = false;
	public bool hostFound;
	public bool udpReceived = false;
	public int udpTimeout = 2;
	
	//user can insert from menu, if empty, autoscan happens
	public string userIP;
	//user can overwrite this
	public int portNumber = 11200;
	public bool waitingOnResponse;


	public DateTime timerOfLastReceived;
	public bool testPrediction = false;

	#region private members 	
	//private TcpClient socketConnection;


	public string hostName;
	public int ip4;
	public int ip3;

	public int socketTimeoutTime = 5;
	public float timer = 5f;
	public float connectionTimer = 0f;

	public float handshakeInterval = 3f;
	public float handshakeTimer = 0f;

	bool localScanAttempted = false;//127.0.0.1 internal loopback scan

	#endregion
	//UdpClient listener;// = new UdpClient(listenPort)

	Thread threadListen;
	Thread threadSender;
	void Awake()
	{
		if (buildControl.isServer)
		{
			//disable client script
			enabled = false;
		}

		//so if statement fires on first frame
		handshakeTimer = handshakeInterval;

	
	}


	private void Start()
	{
		//will need to re run this on port change
	//	listener = new UdpClient(portNumber);
		//listenEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
		//start udp sender test
		threadSender = new Thread(() => UDPSender());
		threadSender.IsBackground = true;
		threadSender.Start();//does this close automatically?
		
		
	//	threadListen = new Thread(() => UDPListener());
	//	threadListen.IsBackground = true;
	//	threadListen.Start();//does this close automatically?
		

		timerOfLastReceived = DateTime.Now.AddSeconds(-udpTimeout);
		//StartCoroutine("Listener");
	}

	void OnApplicationQuit()
	{
		if(threadSender != null)
			threadSender.Abort();
		if(threadListen != null)
			threadListen.Abort();
	}
	public void Update()
	{
		//wait before scanning
		if (menuHandler.stopwatch.ElapsedMilliseconds < 5)
			return;

		//LED control
		
		//var seconds = (DateTime.Now - timerOfLastReceived).TotalSeconds;
		
		double seconds = (DateTime.Now - timerOfLastReceived).TotalSeconds;
		//Debug.Log("seconds = " + seconds);
		if (seconds > udpTimeout)
			connected = false;
		else
		{
		//	Debug.Log("setting true");
			connected = true;
		}
		
	}

	void UDPSender()
	{
		byte[] data = System.Text.Encoding.ASCII.GetBytes("I am the client");
		string ipAddress = "192.168.1.2";
		int sendPort = 11200;

		//UdpClient udpClient = new UdpClient(11200); //this binds! we only want the server to bind the port, not tihs(the client)
		UdpClient udpClient = new UdpClient();
		udpClient.Connect("192.168.1.2", 11200);
		try
		{
			
			while (true)
			{
				

				// Sends a message to the host to which you have connected.
				byte[] sendBytes = System.Text.Encoding.ASCII.GetBytes("Is anybody there?");

				udpClient.Send(sendBytes, sendBytes.Length);


				//IPEndPoint object will allow us to read datagrams sent from any source.
				IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

				// Blocks until a message returns on this socket from a remote host.
				byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);

				ProcessPackage(receiveBytes);

				udpReceived = true;

				timerOfLastReceived = DateTime.Now;

				string returnData = System.Text.Encoding.ASCII.GetString(receiveBytes);

				// Uses the IPEndPoint object to determine which of these two hosts responded.
				Debug.Log("This is the message you received " +
											 returnData.ToString());
				Debug.Log("This message was sent from " +
											RemoteIpEndPoint.Address.ToString() +
											" on their port number " +
											RemoteIpEndPoint.Port.ToString());

				Thread.Sleep(500);
			}


			
		}

		catch (Exception e)
		{
			Console.WriteLine(e.ToString());
			udpClient.Close();
			//break;
		}

		udpClient.Close();
	}


	void UDPListener()
	{
		int listenPort = portNumber;
		UdpClient listener = new UdpClient(listenPort);
		{

			try
			{
				IPEndPoint listenEndPoint = new IPEndPoint(IPAddress.Any, listenPort);
				while (true)
				{
					udpReceived = false;
					byte[] receivedData = listener.Receive(ref listenEndPoint);
					Debug.Log(receivedData);

					ProcessPackage(receivedData);


					udpReceived = true;

					timerOfLastReceived = DateTime.Now;

				}
			}
			catch (Exception ex)
			{
				Debug.Log(ex.ToString());
			}
		}
	}

	void ProcessPackage(byte[] bytes)
    {
		int p = 0;

		//set length sent from server	
		int floatArrayLength = 14;
		int floatArrayLengthBytes = 4 * floatArrayLength; //4 bytes for float * array length
														  //float array
		float[] floats = GetFloats(bytes, p, floatArrayLength);

		//check for Nan, infinity etc
		DataCheck(floats);

		if (!testPrediction)
		{
			//set Il2 game data for client
			iL2GameDataClient.altitude = floats[0];
			iL2GameDataClient.mmhg = floats[1];
			iL2GameDataClient.airspeed = floats[2];
			//save previous heading before asigning new heading - needed for turn co-ordinator needle
			iL2GameDataClient.headingPreviousPrevious = iL2GameDataClient.headingPrevious;
			iL2GameDataClient.headingPrevious = iL2GameDataClient.heading;
			iL2GameDataClient.heading = floats[3];
			iL2GameDataClient.pitch = floats[4];
			iL2GameDataClient.rollPrev = iL2GameDataClient.roll;
			iL2GameDataClient.roll = floats[5];
			iL2GameDataClient.verticalSpeed = floats[6];
			iL2GameDataClient.turnCoordinatorBall = floats[7];
			iL2GameDataClient.turnCoordinatorNeedle = floats[8];
			iL2GameDataClient.rpms[0] = floats[9];
			iL2GameDataClient.rpms[1] = floats[10];
			iL2GameDataClient.rpms[2] = floats[12];
			iL2GameDataClient.rpms[3] = floats[13]; //support for 4 engines (you never know!)
		}
		p += floatArrayLengthBytes;

		//version number
		//receiving server version from stream (server -> client)
		iL2GameDataClient.serverVersion = BitConverter.ToSingle(bytes, p);
		p += sizeof(float);

		//plane type string size
		uint stringSize = BitConverter.ToUInt32(bytes, p);
		p += sizeof(uint);
		//plane type string
		string planeType = System.Text.Encoding.UTF8.GetString(bytes, p, (int)stringSize);
		iL2GameDataClient.planeType = planeType;
		//p += 64;//chosen max string size (by me)

	}

	void DataCheck(float[] floats)
    {
        // check for nan infinity etc
        for (int i = 0; i < floats.Length; i++)
        {
			if(float.IsNaN( floats[i])  || float.IsInfinity (floats[i]) || float.IsNegativeInfinity(floats[i]))
            {
				floats[i] = 0;
            }
        }
    }

	static float[] GetFloats(byte[] bytes, int offset, int floatArrayLength)
	{
		try
		{
			var result = new float[floatArrayLength];
			Buffer.BlockCopy(bytes, offset, result, 0, floatArrayLength * 4); //* 4 for 4byte floats

			return result;

		}
		catch (Exception ex)
		{
			Debug.Log("Exceptiom" + ex);
			return null;
		}

	}


}