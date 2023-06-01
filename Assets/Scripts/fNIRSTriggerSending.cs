using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;

public class fNIRSTriggerSending : MonoBehaviour
{

    public string fNIRSHost;
    public int fNIRSRemotePort;
    public int fNIRSLocalPort;

    // Creating a transmitter.
    public OSCTransmitter fNIRSTransmitter;
    // Creating a receiver.
    public OSCReceiver fNIRSReceiver;

    public int triggerNum;
    public bool starTask;
    public bool sendtrigger;

    

    // Start is called before the first frame update
    void Start()
    {
        // Set remote host address.
        fNIRSTransmitter.RemoteHost = fNIRSHost;

        // Set remote port;
        fNIRSTransmitter.RemotePort = fNIRSRemotePort;

        // Set local port.
        fNIRSReceiver.LocalPort = fNIRSLocalPort;
    }

    void Update()
    {
        if(starTask && sendtrigger)
        {
            SendfNIRSTrigger(triggerNum, starTask, sendtrigger);
            sendtrigger = false;
        }
    }


    public void SendfNIRSTrigger(int triggerNum, bool startTask, bool sendTrigger)
    {
        var fNIRSTriggerMessage = new OSCMessage("/fNIRSTrigger");
        fNIRSTriggerMessage.AddValue(OSCValue.Int(triggerNum));
        fNIRSTriggerMessage.AddValue(OSCValue.Bool(startTask));
        fNIRSTriggerMessage.AddValue(OSCValue.Bool(sendTrigger));
        fNIRSTransmitter.Send(fNIRSTriggerMessage);
    }
}
