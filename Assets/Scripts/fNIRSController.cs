using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;
using TMPro;

public class fNIRSController : MonoBehaviour
{
    [Header("Script append")]
    [SerializeField]
    GameObject AuroraGameObject;

    LSL4Unity.Samples.Complex.AuroraTriggerWithoutnBack auroraTrigger;

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

    public int WhichConditionInt;
    public bool startStudy = false;
    public bool startTask = false;
    public bool sendTrigger = false;
    

    // Start is called before the first frame update
    void Start()
    {

        auroraTrigger = AuroraGameObject.GetComponent<LSL4Unity.Samples.Complex.AuroraTriggerWithoutnBack>();
        IPInputField.GetComponent<TMP_InputField>();
        CPortInputField.GetComponent<TMP_InputField>();
        QPortInputField.GetComponent<TMP_InputField>();

        Transmitter.RemoteHost = RemoteHost;

        // Set remote port;
        Transmitter.RemotePort = RemotePort;

        // Set local port.
        Receiver.LocalPort = LocalPort;

        IPInputField.text = RemoteHost;
        CPortInputField.text = RemotePort.ToString();
        QPortInputField.text = LocalPort.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        Receiver.Bind("/startStudy", ReceiveStartStudy);
        Receiver.Bind("/fNIRSTrigger", ReceiveSendTrigger);


        if (startStudy)
        {
            auroraTrigger.startTesk = startTask;
            
            if (sendTrigger)
            {
                auroraTrigger.SendTrigger();
            }
        }
    }

    protected void ReceiveStartStudy(OSCMessage message)
    {
        startStudy = message.Values[0].BoolValue;
    }

    protected void ReceiveSendTrigger(OSCMessage message)
    {
        WhichConditionInt = message.Values[0].IntValue;
        auroraTrigger.whichCondition = WhichConditionInt.ToString();
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
