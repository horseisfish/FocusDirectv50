using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using extOSC;
using TMPro;

public class OculusEventController : MonoBehaviour
{
    [Header("Script append")]
    [SerializeField]
    GameObject targetGenerateObject;
    [SerializeField]
    GameObject eyeTrackingLeftObject;
    [SerializeField]
    GameObject eyeTrackingRightObject;

    TargetGeneraterWithOSC targetGenerater;
    EyeTrackingRay LefteyeTracking;
    EyeTrackingRay RighteyeTracking;
    OVRPassthroughLayer passthroughLayer;

    [Space(5)]
    [Header("OSC setting")]
    public string RemoteHost;
    public int RemotePort = 0;
    public int LocalPort = 0;
    // Creating a transmitter.
    public OSCTransmitter Transmitter;
    // Creating a receiver.
    public OSCReceiver Receiver;

    public GameObject description;
    public TMP_InputField IPInputField;
    public TMP_InputField CPortInputField;
    public TMP_InputField QPortInputField;
    public float gazetimeis;
    public bool startStudy = false;
    public bool TargetShow = false;
    public bool TargetDestroy = false;


    // Start is called before the first frame update
    void Start()
    {
        targetGenerater = targetGenerateObject.GetComponent<TargetGeneraterWithOSC>();
        LefteyeTracking = eyeTrackingLeftObject.GetComponent<EyeTrackingRay>();
        RighteyeTracking = eyeTrackingRightObject.GetComponent<EyeTrackingRay>();
        IPInputField.GetComponent<TMP_InputField>();
        CPortInputField.GetComponent<TMP_InputField>();
        QPortInputField.GetComponent<TMP_InputField>();

        
        IPInputField.text = RemoteHost;
        CPortInputField.text = RemotePort.ToString();
        QPortInputField.text = LocalPort.ToString();

        //passthrough seeting
        GameObject ovrCameraRig = GameObject.Find("OVRCameraRig");
        if (ovrCameraRig == null)
        {
            Debug.LogError("Scene does not contain an OVRCameraRig");
            return;
        }

        passthroughLayer = ovrCameraRig.GetComponent<OVRPassthroughLayer>();
        if (passthroughLayer == null)
        {
            Debug.LogError("OVRCameraRig does not contain an OVRPassthroughLayer component");
        }

        passthroughLayer.hidden = true;
        Receiver.Bind("/passthrough", ReceivedPassthrough);

        // Set remote host address.
        Transmitter.RemoteHost = RemoteHost;

        // Set remote port;
        Transmitter.RemotePort = RemotePort;

        // Set local port.
        Receiver.LocalPort = LocalPort;

        Receiver.Bind("/gazetime", ReceiveGazeTime);
        Receiver.Bind("/radius", ReceiveRadiusandHeight);
    }

    // Update is called once per frame
    void Update()
    {
        Receiver.Bind("/startStudy", ReceiveStartStudy);
        if (startStudy)
        {
            Receiver.Bind("/passthrough", ReceivedPassthrough);
            Receiver.Bind("/generateTargetBool", ReceiveGenerateTargetBool);
            Receiver.Bind("/generateTarget", ReceivedGenerateTarget);
            Receiver.Bind("/destroy", ReceiveDistroyObj);
            SendLeftEyeGazed(LefteyeTracking.gazed);
            SendLeftEyeGazeTimer(LefteyeTracking.gazeTimer);
            SendRightEyeGazed(RighteyeTracking.gazed);
            SendRightEyeGazeTimer(RighteyeTracking.gazeTimer);
        }
    }

    public void SendLeftEyeGazeTimer(float leftGazeTimer)
    {
        var leftEyeGazeTimerMessage = new OSCMessage("/leftGazeTimer");
        leftEyeGazeTimerMessage.AddValue(OSCValue.Float(leftGazeTimer));
        Transmitter.Send(leftEyeGazeTimerMessage);
    }
    public void SendRightEyeGazeTimer(float rightGazeTimer)
    {
        var rightEyeGazeTimerMessage = new OSCMessage("/rightGazeTimer");
        rightEyeGazeTimerMessage.AddValue(OSCValue.Float(rightGazeTimer));
        Transmitter.Send(rightEyeGazeTimerMessage);
    }

    public void SendLeftEyeGazed(bool leftEyeGazed)
    {
        var leftEyeGazedMessage = new OSCMessage("/leftGazed");
        leftEyeGazedMessage.AddValue(OSCValue.Bool(leftEyeGazed));
        Transmitter.Send(leftEyeGazedMessage);
    }

    public void SendRightEyeGazed(bool rightEyeGazed)
    {
        var rightEyeGazedMessage = new OSCMessage("/rightGazed");
        rightEyeGazedMessage.AddValue(OSCValue.Bool(rightEyeGazed));
        Transmitter.Send(rightEyeGazedMessage);
    }

    protected void ReceiveStartStudy(OSCMessage message)
    {
        startStudy = message.Values[0].BoolValue;
    }

    protected void ReceiveRadiusandHeight(OSCMessage message)
    {
        targetGenerater.minRadius = message.Values[0].FloatValue;
        targetGenerater.maxRadius = message.Values[1].FloatValue;
        targetGenerater.minxAngle = message.Values[2].FloatValue;
        targetGenerater.maxxAngle = message.Values[3].FloatValue;
    }

    protected void ReceiveGazeTime(OSCMessage message)
    {
        gazetimeis = message.Values[0].FloatValue;
        LefteyeTracking.gazeTime = message.Values[0].FloatValue;
        RighteyeTracking.gazeTime = message.Values[0].FloatValue;
    }

    protected void ReceivedPassthrough(OSCMessage message)
    {
        passthroughLayer.hidden = message.Values[0].BoolValue;
        if (passthroughLayer.hidden)
        {
            description.SetActive(true);
        }
        else
        {
            description.SetActive(false);
        }
    }

    protected void ReceivedGenerateTarget(OSCMessage message)
    {

        if (TargetShow)
        {
            Debug.Log("generate message" + message.Values[0].IntValue + "," + message.Values[1].IntValue + "," + message.Values[2].IntValue);
            targetGenerater.GenerateTarget(message.Values[0].IntValue, message.Values[1].IntValue, message.Values[2].IntValue);
            TargetShow = false;
        }
    }
    protected void ReceiveDistroyObj(OSCMessage message)
    {
        TargetDestroy = message.Values[0].BoolValue;
        if (TargetDestroy)
        {
            Destroy(targetGenerater.obj);
        }
    }
    protected void ReceiveGenerateTargetBool(OSCMessage message)
    {
        TargetShow = message.Values[0].BoolValue;
        //Debug.Log("targetshow is" + TargetShow);
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
