using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UDPClient : MonoBehaviour
{
    //class which reads memory and sets values	

    public BuildControl buildControl;
    public AirplaneData airplaneData;
    public MenuHandler menuHandler;
    public RotateNeedle rN;
    public SlaveManager slaveManager;
    public bool connected = false;
    public float autoScanTimeScale = 1f;
    public float standardFixedTime = 0.02f;
    public int sendRate = 16;
    public bool autoScan = false;
    public bool hostFound;
    public bool udpReceived = false;
    public int udpTimeout = 2;
    public int portNumber = 11200;
    public bool waitingOnResponse;
    public DateTime timerOfLastReceived;
    public bool testPrediction = false;
    public string serverAddress;
    public int ip4;
    public int ip3;
    public int socketTimeoutTime = 5;
    public float connectionTimer = 0f;
    bool localScanAttempted = false;

    private void Start()
    {
        //check if server address is empty, if it is, we must autoscan
        if (serverAddress == "")
        {
            autoScan = true;
        }

        //so if statement catches check on first frame
        timerOfLastReceived = DateTime.Now.AddSeconds(-udpTimeout);
    }

    public void Update()
    {
        //wait before scanning
        if (menuHandler.stopwatch.ElapsedMilliseconds < 5)
            return;

        //set connected flag if timeout
        double seconds = (DateTime.Now - timerOfLastReceived).TotalSeconds;

        if (seconds > udpTimeout)
            connected = false;
        else
            connected = true;

        //search for the server and start send/listen threads
        if (!hostFound)
            Scan();
        else
            menuHandler.scanDebug.GetComponent<Text>().text = "Connected: " + serverAddress.ToString() + " : " + portNumber;
    }

    void Scan()
    {
        if (autoScan)
        {
            //look for local connection before going to wifi
            if (!localScanAttempted)
            {
                serverAddress = "127.0.0.1";

                //first frame we attempt local scan, then on to wifi
                localScanAttempted = true;
            }
            else
            {
                serverAddress = "192.168." + ip3.ToString() + "." + ip4.ToString();
            }

            //start a scanner thread
            Thread thread = new Thread(() => UDPScanner(serverAddress));
            thread.IsBackground = true;
            thread.Start();

            //update UI
            menuHandler.scanDebug.GetComponent<Text>().text = "Scanning IP: " + serverAddress.ToString(); ;

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
                    //restart - if people have strange ips they probably know about it and can use direct connection option
                    ip3 = 0;
                    ip4 = 0;

                }
            }
        }
        else
        {
            //go straight to ip address in text field from interface
            menuHandler.scanDebug.GetComponent<Text>().text = "Attempting Connection: " + serverAddress.ToString() + " : " + portNumber;
            //use value entered by user in ip address text box
            Thread thread = new Thread(() => UDPScanner(serverAddress));
            thread.IsBackground = true;
            thread.Start();
        }
    }

    void UDPScanner(string _serverAddress)
    {
        //create package to send - content arbitary
        byte[] data = System.Text.Encoding.ASCII.GetBytes("Il-2 Client request");

        //use standard constructor, if we use params it will bind the port under the ood. Only the server should bind the port
        var client = new UdpClient();
        //set quick timeouts on scan thread
        client.Client.ReceiveTimeout = 100;

        //endpoint where server is listening
        IPEndPoint ep = new IPEndPoint(IPAddress.Parse(_serverAddress), portNumber);

        try
        {
            //create a connection - this will hold until closed
            client.Connect(ep);

            // Sends a message to the host to which you have connected.
            byte[] sendBytes = System.Text.Encoding.ASCII.GetBytes("IL-2 Client");
            client.Send(sendBytes, sendBytes.Length);

            //////blocking call
            byte[] receivedData = client.Receive(ref ep);

            hostFound = true;

            //we have found the server!
            //start another thread for the data streamer (with no timeout settings)
            Thread thread = new Thread(() => UDPSender(_serverAddress));
            thread.IsBackground = true;
            thread.Start();
        }

        catch (Exception e)
        {
            client.Close();
        }

    }


    void UDPSender(string _serverAddress)
    {
        //use standard constructor, if we use params it will bind the port under the hood. Only the server should bind the port
        var client = new UdpClient();

        //endpoint where server is listening
        IPEndPoint ep = new IPEndPoint(IPAddress.Parse(_serverAddress), portNumber);
        //create a connection - this will hold until closed
        client.Connect(ep);

        //now we have an end point create a listener on a seperate thread
        Thread thread = new Thread(() => UDPListener(client, ep));
        thread.IsBackground = true;
        thread.Start();

        try
        {
            while (true)
            {
                // Sends a message to the host to which we have connected.
                byte[] sendBytes = System.Text.Encoding.ASCII.GetBytes("IL-2 Client");
                client.Send(sendBytes, sendBytes.Length);
                Thread.Sleep(sendRate);
            }
        }

        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
            client.Close();
        }

        client.Close();
    }


    void UDPListener(UdpClient udpClient, IPEndPoint ep)
    {
        try
        {
            while (true)
            {
                udpReceived = false;

                //////blocking call
                byte[] receivedData = udpClient.Receive(ref ep);

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

    void ProcessPackage(byte[] bytes)
    {
        int p = 0;

        //set length sent from server	
        int floatArrayLength = 38;
        int floatArrayLengthBytes = 4 * floatArrayLength; //4 bytes for float * array length
                                                          //float array
        float[] floats = GetFloats(bytes, p, floatArrayLength);

        //check for Nan, infinity etc and zero them - we can receive garbled data from reading raw memory
        DataCheck(floats);

        if (!testPrediction)
        {
            //set Il2 game data for client
            airplaneData.altitude = floats[0];
            airplaneData.mmhg = floats[1];
            airplaneData.airspeed = floats[2];
            //save previous heading before asigning new heading - needed for turn co-ordinator needle
            airplaneData.headingPreviousPrevious = airplaneData.headingPrevious;
            airplaneData.headingPrevious = airplaneData.heading;
            airplaneData.heading = floats[3];
            airplaneData.pitch = floats[4];
            airplaneData.rollPrev = airplaneData.roll;
            airplaneData.roll = floats[5];
            airplaneData.verticalSpeed = floats[6];
            airplaneData.turnCoordinatorBall = floats[7];
            airplaneData.turnCoordinatorNeedle = floats[8];

            if (airplaneData.planeType == "Ju-52/3m g4e") //TODO, re-order on server side
            {
                //switch order of engines to flow left to right

                //rpm
                float temp = floats[11];
                floats[11] = floats[10];
                floats[10] = temp;

                //oil out
                temp = floats[23];
                floats[23] = floats[24];
                floats[24] = temp;

                //oil inbound
                temp = floats[27];
                floats[27] = floats[28];
                floats[28] = temp;
            }

            airplaneData.rpms[0] = floats[9];
            airplaneData.rpms[1] = floats[10];
            airplaneData.rpms[2] = floats[11];
            airplaneData.rpms[3] = floats[12]; //support for 4 engines (you never know!)

            airplaneData.manifolds[0] = floats[13];
            airplaneData.manifolds[1] = floats[14];
            airplaneData.manifolds[2] = floats[15];
            airplaneData.manifolds[3] = floats[16];
            airplaneData.engineModification = (int)floats[17];
            airplaneData.waterTemps[0] = floats[18];
            airplaneData.waterTemps[1] = floats[19];
            airplaneData.waterTemps[2] = floats[20];
            airplaneData.waterTemps[3] = floats[21];
            airplaneData.oilTempsOut[0] = floats[22];
            airplaneData.oilTempsOut[1] = floats[23];
            airplaneData.oilTempsOut[2] = floats[24];
            airplaneData.oilTempsOut[3] = floats[25];
            airplaneData.oilTempsIn[0] = floats[26];
            airplaneData.oilTempsIn[1] = floats[27];
            airplaneData.oilTempsIn[2] = floats[28];
            airplaneData.oilTempsIn[3] = floats[29];
            airplaneData.cylinderHeadTemps[0] = floats[30] - 273.15f;
            airplaneData.cylinderHeadTemps[1] = floats[31] - 273.15f;
            airplaneData.cylinderHeadTemps[2] = floats[32] - 273.15f;
            airplaneData.cylinderHeadTemps[3] = floats[33] - 273.15f;

            airplaneData.carbAirTemps[0] = floats[34]; //date in celsius in game
            airplaneData.carbAirTemps[1] = floats[35];
            airplaneData.carbAirTemps[2] = floats[36];
            airplaneData.carbAirTemps[3] = floats[37];
        }

        p += floatArrayLengthBytes;

        //version number
        //receiving server version from stream (server -> client)
        airplaneData.serverVersion = BitConverter.ToSingle(bytes, p);
        p += sizeof(float);

        //plane type string size
        uint stringSize = BitConverter.ToUInt32(bytes, p);
        p += sizeof(uint);

        //plane type string
        string planeType = System.Text.Encoding.UTF8.GetString(bytes, p, (int)stringSize);

        //using setter method so we can check menu status before chaning plane name
        airplaneData.setPlaneType(planeType);
    }

    void DataCheck(float[] floats)
    {
        // check for nan infinity etc
        for (int i = 0; i < floats.Length; i++)
        {
            if (float.IsNaN(floats[i]) || float.IsInfinity(floats[i]) || float.IsNegativeInfinity(floats[i]))
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