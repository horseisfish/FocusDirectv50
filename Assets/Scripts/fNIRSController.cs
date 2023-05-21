using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;
using TMPro;
using Assets.LSL4Unity.Scripts;
using UnityEngine.UI;

public class fNIRSController : MonoBehaviour
{
    [Header("Script append")]
    [SerializeField]
    public LSLMarkerStream maker;
    

    [Space(5)]
    [Header("OSC setting")]
    public string RemoteHost;
    public int RemotePort = 0;
    public int LocalPort = 0;
    // Creating a transmitter.
    public OSCTransmitter Transmitter;
    // Creating a receiver.
    public OSCReceiver Receiver;

    public TMP_InputField IPInputField;
    public TMP_InputField CPortInputField;
    public TMP_InputField QPortInputField;
    public TMP_InputField ConditionField;
    public Toggle starTaskToggle;
    public Toggle sendTriggerToggle;
    public Image background;

    public int WhichConditionInt;
    public bool startStudy = false;
    public bool startTask = false;
    public bool sendTrigger = false;
    

    // Start is called before the first frame update
    void Start()
    {

        maker = GetComponent<LSLMarkerStream>();
        IPInputField.GetComponent<TMP_InputField>();
        CPortInputField.GetComponent<TMP_InputField>();
        QPortInputField.GetComponent<TMP_InputField>();
        ConditionField.GetComponent<TMP_InputField>();
        starTaskToggle.GetComponent<Toggle>();
        sendTriggerToggle.GetComponent<Toggle>();
        background.GetComponent<Image>();

        Transmitter.RemoteHost = RemoteHost;

        // Set remote port;
        Transmitter.RemotePort = RemotePort;

        // Set local port.
        Receiver.LocalPort = LocalPort;

        IPInputField.text = RemoteHost;
        CPortInputField.text = RemotePort.ToString();
        QPortInputField.text = LocalPort.ToString();
        ConditionField.text = WhichConditionInt.ToString();

        starTaskToggle.isOn = false;
        sendTriggerToggle.isOn = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        Receiver.Bind("/startStudy", ReceiveStartStudy);
        Receiver.Bind("/fNIRSTrigger", ReceiveSendTrigger);


        if (startStudy)
        {
            starTaskToggle.isOn = true;
            if (sendTrigger)
            {
                sendTriggerToggle.isOn = true;
                maker.Write(WhichConditionInt.ToString());
                background.color = new Color(225,193,110);                
                sendTriggerToggle.isOn = false;
                background.color = new Color(0, 0, 0);
                sendTrigger = false;
            }
            
        }
        starTaskToggle.isOn = false;
    }

    protected void ReceiveStartStudy(OSCMessage message)
    {
        startStudy = message.Values[0].BoolValue;
    }

    protected void ReceiveSendTrigger(OSCMessage message)
    {
        WhichConditionInt = message.Values[0].IntValue;
        ConditionField.text = WhichConditionInt.ToString();
        startTask = message.Values[1].BoolValue;
        sendTrigger = message.Values[2].BoolValue;
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
}
