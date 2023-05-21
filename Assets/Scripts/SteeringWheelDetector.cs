using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;
using TMPro;
using UnityEngine.UI;

public class SteeringWheelDetector : MonoBehaviour
{

    [Header("OSC setting")]
    public string RemoteHost;
    public int RemotePort;
    public int LocalPort;
    // Creating a transmitter.
    public OSCTransmitter Transmitter;
    // Creating a receiver.
    public OSCReceiver Receiver;

    public TMP_InputField IPInputField;
    public TMP_InputField CPortInputField;
    public TMP_InputField QPortInputField;

    public Toggle correctToggle;
    public Toggle wrongToggle;
    public Toggle starTaskToggle;

    public bool startStudy;
    public bool correctButton;
    public bool wrongButton;
    
    private bool prevCorrectButton;
    private bool prevWrongButton;

    //steeringWheel
    LogitechGSDK.LogiControllerPropertiesData properties;
    

    // Start is called before the first frame update
    void Start()
    {
        correctButton = false;
        wrongButton = false;
        prevCorrectButton = false;
        prevWrongButton = false;

        IPInputField.GetComponent<TMP_InputField>();
        CPortInputField.GetComponent<TMP_InputField>();
        QPortInputField.GetComponent<TMP_InputField>();
        starTaskToggle.GetComponent<Toggle>();
        correctToggle.GetComponent<Toggle>();
        wrongToggle.GetComponent<Toggle>();

        string remotportstring = RemotePort.ToString();
        IPInputField.text = RemoteHost;
        CPortInputField.text = remotportstring;
        QPortInputField.text = LocalPort.ToString();

        // Set remote host address.
        Transmitter.RemoteHost = RemoteHost;

        // Set remote port;
        Transmitter.RemotePort = RemotePort;

        // Set local port.
        Receiver.LocalPort = LocalPort;

        starTaskToggle.isOn = false;
        correctToggle.isOn = false;
        wrongToggle.isOn = false;
        
        Debug.Log("SteeringInit:" + LogitechGSDK.LogiSteeringInitialize(false));
    }

    // Update is called once per frame
    void Update()
    {
        Receiver.Bind("/startStudy", ReceiveStartStudy);
        if (startStudy)
        {
            starTaskToggle.isOn = true;
            if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
            {

                
                
                LogitechGSDK.LogiControllerPropertiesData actualProperties = new LogitechGSDK.LogiControllerPropertiesData();
                LogitechGSDK.LogiGetCurrentControllerProperties(0, ref actualProperties);
                //LogitechGSDK.DIJOYSTATE2ENGINES rec;
                //rec = LogitechGSDK.LogiGetStateUnity(0);
                Debug.Log(LogitechGSDK.LogiButtonIsPressed(0, 2));
                if (LogitechGSDK.LogiButtonIsPressed(0,2) || Input.GetKeyDown(KeyCode.A))
                {
                    correctButton = true;
                    correctToggle.isOn = true;
                    
                }
                else 
                {
                    correctButton = false;
                    correctToggle.isOn = false;
                    
                }
                if (LogitechGSDK.LogiButtonIsPressed(0, 0)|| Input.GetKeyDown(KeyCode.B))
                {
                    wrongButton = true;
                    wrongToggle.isOn = true;
                    

                }
                else 
                {
                    wrongButton = false;
                    wrongToggle.isOn = false;
                    
                    
                }


                if (correctButton != prevCorrectButton || wrongButton != prevWrongButton)
                {
                    Debug.Log("Correct Button: " + correctButton + " WrongButton: " + wrongButton);

                    // Call the function to send the steering wheel button
                    SendStreeingWheelButton(correctButton, wrongButton);

                    // Update the previous state variables
                    prevCorrectButton = correctButton;
                    prevWrongButton = wrongButton;
                }



            }
        }
        starTaskToggle.isOn = false;
    }

    protected void ReceiveStartStudy(OSCMessage message)
    {
        startStudy = message.Values[0].BoolValue;
    }

    public void SendStreeingWheelButton(bool correct, bool wrong)
    {
        var ButtonMessage = new OSCMessage("/steeringwheel");
        ButtonMessage.AddValue(OSCValue.Bool(correct));
        ButtonMessage.AddValue(OSCValue.Bool(wrong));
        Transmitter.Send(ButtonMessage);
    }

    public void changeIP()
    {
        RemoteHost = IPInputField.text;
        RemotePort = int.Parse(CPortInputField.text);
        LocalPort = int.Parse(QPortInputField.text);

        // Set remote host address.
        Transmitter.RemoteHost = RemoteHost;

        // Set remote port;
        Transmitter.RemotePort = RemotePort;

        // Set local port.
        Receiver.LocalPort = LocalPort;
    }

    void OnApplicationQuit()
    {
        Debug.Log("SteeringShutdown:" + LogitechGSDK.LogiSteeringShutdown());
    }
}
