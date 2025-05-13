using System;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class ArduinoReader : MonoBehaviour
{
    //SerialPort serialPort = new SerialPort("COM3", 9600); 
    public string comName = "Com3";
    public int port = 9600;

    public int throttleValue;                             // Variable for received throttle value
    public int secondValue;                               // Variable for received braking value
    private Thread serialThread;                          // Thread for reading serial data
    private bool isRunning = true;                         // Boolean for checking if the thread is running
    private SerialPort serialPort;

    public EScooterNew eScooterWheel;
    public EScooterNew eScooterWheelOld;
    void Start()
    {
        serialPort = new SerialPort(comName, port);
        serialPort.Open();                                // Open serial connection
        serialPort.ReadTimeout = 100;                    // Timeout can be adjusted for faster reads
        serialThread = new Thread(ReadSerialData);
        serialThread.Start();
    }

    void ReadSerialData()
    {
        while (isRunning)
        {
            try
            {
                //read last input
                string line = serialPort.ReadLine();
                serialPort.ReadExisting();
                lock (this)
                {
                    string data = line;
                    //our data input uses "," as divider
                    string[] values = data.Split(',');
                    {
                        //Parse to integer to pass on
                        throttleValue = int.Parse(values[0]);
                        secondValue = int.Parse(values[1]);

                        //send data to e-scooter
                        eScooterWheel.arduinoAccelerate = throttleValue;
                        eScooterWheel.arduinoDeceleration = secondValue;
                        //sends to a second e-scooter
                        if (eScooterWheelOld != null)
                        {
                            eScooterWheelOld.arduinoAccelerate = throttleValue;
                            eScooterWheelOld.arduinoDeceleration = secondValue;
                        }

                    }
                }
            }
            catch (TimeoutException) { }
            catch (FormatException) { }
            catch
            {
                Debug.Log("Temporary microcontroller reading error!");
            }
        }
    }


    void OnApplicationQuit()
    {
        //closing connection and ending thread
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }
        isRunning = false;
        serialThread.Join();
    }
}
