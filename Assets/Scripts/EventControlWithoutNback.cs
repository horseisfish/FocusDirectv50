using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using extOSC;
using TMPro;

public class EventControlWithoutNback : MonoBehaviour
{
    //[Header("Script append")]
    //[SerializeField]
    //GameObject AuroraGameObject;

    //LSL4Unity.Samples.Complex.AuroraTriggerWithoutnBack auroraTrigger;

    [Space(5)]
    [Header("UserID and File path")]
    public string UserID;
    public string filePath = @"C:\temp\output.csv";

    [Space(5)]
    [Header("OSC setting")]
    public string QuestHost;
    public string PCHost;
    public string fNIRSHost;
    public int QuestRemotePort;
    public int QuestLocalPort;
    public int PCRemotePort;
    public int PCLocalPort;
    public int fNIRSRemotePort;
    public int fNIRSLocalPort;
    // Creating a transmitter.
    public OSCTransmitter QuestTransmitter;
    // Creating a receiver.
    public OSCReceiver QuestReceiver;
    // Creating a transmitter.
    public OSCTransmitter PCTransmitter;
    // Creating a receiver.
    public OSCReceiver PCReceiver;
    // Creating a transmitter.
    public OSCTransmitter fNIRSTransmitter;
    // Creating a receiver.
    public OSCReceiver fNIRSReceiver;
    public TMP_InputField QIPInputField;    
    public TMP_InputField QRPortInputField;
    public TMP_InputField QLPortInputField;
    public TMP_InputField PCIPInputField;
    public TMP_InputField PCRPortInputField;
    public TMP_InputField PCLPortInputField;
    public TMP_InputField fNIRSIPInputField;
    public TMP_InputField fNIRSRPortInputField;
    public TMP_InputField fNIRSLPortInputField;



    [Space(5)]
    [Header("Condition setting")]

    public conditionType whichCondition = conditionType.targetPlan;
    public int trialRepeatTime;
    public float trialTime;
    public float trialRestTime;
    public int TargetNumbers;
    public float targetRestTime;
    public float gazeTimeIs;
    public float minRadius;
    public float maxRadius;
    public float minHeight;
    public float maxHeight;

    [Space(5)]
    [Header("In Game Data")]

    public int currentTrial;
    public float timer;
    public float leftGazeTimer;
    public float RightGazeTimer;
    public int targetIndex;
    public float targetFindingTime;
    public List<targetStruct> targetList;
    public List<float> targetFindingTimeList;
    public List<bool> targetFindingResultList;
    public bool startStudy;
    public bool targeAlive;
    public bool trailRunning;    
    public bool TargetFined;
    public bool isCoroutineRunning = false;
    public bool Lefteyegazed;
    public bool Righteyegazed;
    public bool passthroughHidden = false;
    public bool correctButton = false;
    public bool wrongButton = false;
    private bool isFinished = false;
    private bool isFirstTime = true;
    private bool destroyObj = true;
    public bool sendtriggers = false;
    public bool fNIRSStartTask = false;
    signType whichSigh;
    private Coroutine generateCoroutine;
    private bool prevfNIRSStartTask= false;
    private bool prevsendtriggers = false;
    private bool prevPassthroughHidden = false;

    public struct targetStruct
    {
        public int area;
        public int cond;
        public int sign;

    }

    public enum signType
    {
        square = 0,
        circle = 1,
        star = 2
    }
    public enum conditionType
    {
        sourcePlan = 1,
        targetPlan = 2,
        HUDPlan = 3,
        Sound = 4
    }
       

    // Start is called before the first frame update
    void Start()
    {
        //auroraTrigger = AuroraGameObject.GetComponent<LSL4Unity.Samples.Complex.AuroraTriggerWithoutnBack>();     
        targetList = new List<targetStruct>();
        targetFindingTimeList = new List<float>();
        targetFindingResultList = new List<bool>();
        timer = 0f;
        currentTrial = 0;
        targetFindingTime = 0f;

        //Generate target
        for (int i = 0; i < TargetNumbers; i++)
        {
            targetStruct target1 = new targetStruct();
            target1.area = i % 2;
            target1.cond = (int)whichCondition;
            target1.sign = Mathf.RoundToInt( Random.Range(0, 2));
            targetList.Add(target1);
            targetFindingResultList.Add(false);
            targetFindingTimeList.Add(0);

        }
        shuffleList(targetList);
        targeAlive = false;
        trailRunning = false;
        targetIndex = 0;
        

        //EyeTracking seeting
        
        TargetFined = false;

        
        

        

        // Set remote host address.
        QuestTransmitter.RemoteHost = QuestHost;

        // Set remote port;
        QuestTransmitter.RemotePort = QuestRemotePort;

        // Set local port.
        QuestReceiver.LocalPort = QuestLocalPort;

        // Set remote host address.
        PCTransmitter.RemoteHost = PCHost;

        // Set remote port;
        PCTransmitter.RemotePort = PCRemotePort;

        // Set local port.
        PCReceiver.LocalPort = PCLocalPort;
        // Set remote host address.
        fNIRSTransmitter.RemoteHost = fNIRSHost;

        // Set remote port;
        fNIRSTransmitter.RemotePort = fNIRSRemotePort;

        // Set local port.
        fNIRSReceiver.LocalPort = fNIRSLocalPort;


        QIPInputField.GetComponent<TMP_InputField>();
        QRPortInputField.GetComponent<TMP_InputField>();
        QLPortInputField.GetComponent<TMP_InputField>();

        PCIPInputField.GetComponent<TMP_InputField>();
        PCRPortInputField.GetComponent<TMP_InputField>();
        PCLPortInputField.GetComponent<TMP_InputField>();

        fNIRSIPInputField.GetComponent<TMP_InputField>();
        fNIRSRPortInputField.GetComponent<TMP_InputField>();
        fNIRSLPortInputField.GetComponent<TMP_InputField>();


        QIPInputField.text = QuestHost;
        QRPortInputField.text = QuestRemotePort.ToString();
        QLPortInputField.text = QuestLocalPort.ToString();
        PCIPInputField.text = PCHost;
        PCRPortInputField.text = PCRemotePort.ToString();
        PCLPortInputField.text = PCLocalPort.ToString();

        fNIRSIPInputField.text = fNIRSHost;
        fNIRSRPortInputField.text = fNIRSRemotePort.ToString();
        fNIRSLPortInputField.text = fNIRSLocalPort.ToString();

        SendRadiusandHeight(minRadius, maxRadius, minHeight, maxHeight);
        SendGazeTime(gazeTimeIs);
        passthroughHidden = true;
        SendPassthrough(passthroughHidden);

    }

    // Update is called once per frame
    void Update()
    {
        if (startStudy)
        {
            
            QuestReceiver.Bind("/leftGazeTimer", leftEyegazeTimerMessageReceived);
            QuestReceiver.Bind("/rightGazeTimer", rightEyegazeTimerMessageReceived);
                                 
            if (currentTrial <= trialRepeatTime - 1)
            {
                trailRunning = true;
                timer += Time.deltaTime;               

                if (timer < trialTime)
                {
                    QuestReceiver.Bind("/leftGazed", leftEyegazed);
                    QuestReceiver.Bind("/rightGazed", rightEyegazed);
                    PCReceiver.Bind("/steeringwheel", buttonPressed);
                    //auroraTrigger.startTesk = true;
                    fNIRSStartTask = true;
                    if (isFirstTime)
                    {
                        //auroraTrigger.SendTrigger();
                        sendtriggers = true;
                        //sendtriggers = false;                        
                        //Debug.Log("sendtriggerfunction");
                        passthroughHidden = false;
                        SendPassthrough(passthroughHidden);
                        //Debug.Log("send passthrough");
                        isFirstTime = false;
                    }

                    if (generateCoroutine == null)
                    {

                        if (targetIndex < TargetNumbers)
                        {
                            //Debug.Log("targetarea is" + targetList[targetIndex].area);
                            generateCoroutine = StartCoroutine(GenerateWithDelay());
                        }
                        else
                        {
                            if (!isFinished)
                            {
                                Debug.Log("targetFinished");
                                isFinished = true;
                            }
                            
                        }
                        
                    }
                    if (Lefteyegazed == false || Righteyegazed == false)
                        {
                        if (targeAlive)
                        {
                            targetFindingTime += Time.deltaTime;
                        }
                            
                        }
                        if (Lefteyegazed || Righteyegazed)
                        {
                            if (Input.GetKeyDown( KeyCode.A) || correctButton )
                            {
                                //Debug.Log("targetsign is" + targetList[targetIndex].sign);
                                if (targetList[targetIndex].sign == 0)
                                {
                                    //Debug.Log("correct");
                                    targetFindingResultList[targetIndex] = true;
                                }
                                else
                                {
                                    //Debug.LogWarning("false");
                                    targetFindingResultList[targetIndex] = false;
                                }
                                targetFindingTimeList[targetIndex] = targetFindingTime;
                                targetFindingTime = 0f;
                                targetIndex++;
                                SendDestroyObj(destroyObj);
                                targeAlive = false;
                                StopCoroutine(generateCoroutine);
                                generateCoroutine = null;
                            correctButton = false;
                                return;
                            }
                            if (Input.GetKeyDown(KeyCode.B) || wrongButton )
                            {
                            
                                //Debug.Log("targetsign is" + targetList[targetIndex].sign);
                                if (targetList[targetIndex].sign == 0)
                                {
                                    //Debug.LogWarning("false");
                                    targetFindingResultList[targetIndex] = false;
                                }
                                else
                                {
                                    //Debug.Log("correct");
                                    targetFindingResultList[targetIndex] = true;
                                }
                                targetFindingTimeList[targetIndex]=targetFindingTime;
                                targetFindingTime = 0f;
                                targetIndex++;
                                SendDestroyObj(destroyObj);
                                targeAlive = false;
                                StopCoroutine(generateCoroutine);
                                generateCoroutine = null;
                            wrongButton = false;
                                return;
                            }
                            

                        }

                    
                }
                else if (timer < trialTime + trialRestTime)
                {
                    //auroraTrigger.startTesk = false;
                    fNIRSStartTask = false;
                    SaveToCSV();
                    //SendDestroyObj(destroyObj);
                    targeAlive = false;
                    passthroughHidden = true;
                    //SendPassthrough(passthroughHidden);
                    if (generateCoroutine != null)
                    {
                        StopCoroutine(generateCoroutine);
                        generateCoroutine = null;
                    }


                }
                else
                {
                    
                    currentTrial++;
                    //auroraTrigger.SendTrigger();
                    fNIRSStartTask = true;
                    sendtriggers = true;                    
                    //sendtriggers = false;                    
                    passthroughHidden = false;
                    //SendPassthrough(passthroughHidden);
                    shuffleList(targetList);
                    targetIndex = 0;
                    timer = 0f;
                    targetFindingTime = 0f;
                    trailRunning = false;
                    //SendDestroyObj(destroyObj);
                    targeAlive = false;
                    if (generateCoroutine != null)
                    {
                        StopCoroutine(generateCoroutine);
                        generateCoroutine = null;
                    }                    

                }
            }
            else
            {
                passthroughHidden = true;
                //SendPassthrough(passthroughHidden);
                //SendDestroyObj(destroyObj);
                StopAllCoroutines();
            }

            if(passthroughHidden != prevPassthroughHidden)
            {
                SendPassthrough(passthroughHidden);
                SendDestroyObj(destroyObj);
                prevPassthroughHidden = passthroughHidden;
            }

            if (fNIRSStartTask != prevfNIRSStartTask || sendtriggers != prevsendtriggers)
            {
                Debug.Log("fNIRSStartTask: " + fNIRSStartTask + " sendtriggers: " + sendtriggers);

                // Call the function to send the steering wheel button
                SendfNIRSTrigger(fNIRSStartTask, sendtriggers);
                sendtriggers = false;               
                // Update the previous state variables
                prevfNIRSStartTask = fNIRSStartTask;
                prevsendtriggers = sendtriggers;
            }
        }
    }

    public void startTask()
    {
        startStudy = true;
        SendStartStudy(startStudy);
    }

    public void shuffleList(List<targetStruct> origenalList)
    {
        for (int i = 0; i < origenalList.Count; i++)
        {
            targetStruct temp = origenalList[i];
            int randomIndex = Random.Range(i, origenalList.Count);
            origenalList[i] = origenalList[randomIndex];
            origenalList[randomIndex] = temp;
        }
    }

    IEnumerator GenerateWithDelay()
    {
        if (targeAlive == false)
        {
            isCoroutineRunning = true;
            yield return new WaitForSeconds(targetRestTime);
            SendGenerateTargetBool(!targeAlive);
            SendGenerateTarget(targetList[targetIndex].area, targetList[targetIndex].cond, targetList[targetIndex].sign);
            targeAlive = true;
            SendGenerateTargetBool(!targeAlive);
        }
            
       
    }

    void SaveToCSV()
    {
        using(StreamWriter writer = new StreamWriter(filePath+"User"+UserID + whichCondition + "trial"+currentTrial+".csv"))
        {
            writer.WriteLine("Target Area, Target sign,TargetFinding Time, TargetFinding result");

            for (int i = 0; i < TargetNumbers; i++)
            {
                string targetArea = targetList[i].area.ToString();
                string targetSign = targetList[i].sign.ToString();
                string tragetFindingTime = targetFindingTimeList[i].ToString();
                string targetFindResult = targetFindingResultList[i].ToString();
                writer.WriteLine(targetArea + "," + targetSign + ","+ tragetFindingTime+ "," + targetFindResult);
            }
                
        }        
    }
    protected void leftEyegazeTimerMessageReceived(OSCMessage message)
    {
        leftGazeTimer = message.Values[0].FloatValue;
        //Debug.Log(message);
    }
    protected void rightEyegazeTimerMessageReceived(OSCMessage message)
    {
        RightGazeTimer = message.Values[0].FloatValue;
        //Debug.Log(message);
    }
    protected void leftEyegazed(OSCMessage message)
    {
        Lefteyegazed = message.Values[0].BoolValue;
        //Debug.Log(message);
    }
    protected void rightEyegazed(OSCMessage message)
    {
        Righteyegazed = message.Values[0].BoolValue;
        //Debug.Log(message);
    }
    protected void buttonPressed(OSCMessage message)
    {
        correctButton = message.Values[0].BoolValue;
        wrongButton = message.Values[1].BoolValue;
    }

    public void SendStartStudy(bool start)
    {
        var startStudyMessage = new OSCMessage("/startStudy");
        startStudyMessage.AddValue(OSCValue.Bool(start));
        QuestTransmitter.Send(startStudyMessage);
        PCTransmitter.Send(startStudyMessage);
        fNIRSTransmitter.Send(startStudyMessage);
    }

    public void SendPassthrough(bool hide)
    {
        // Create message
               
        var passthroughMessage = new OSCMessage("/passthrough");
        passthroughMessage.AddValue(OSCValue.Bool(hide));
        QuestTransmitter.Send(passthroughMessage);
    }

    public void SendRadiusandHeight(float minR, float maxR, float minH, float maxH)
    {
        var RadiusMessage = new OSCMessage("/radius");
        RadiusMessage.AddValue(OSCValue.Float(minR));
        RadiusMessage.AddValue(OSCValue.Float(maxR));
        RadiusMessage.AddValue(OSCValue.Float(minH));
        RadiusMessage.AddValue(OSCValue.Float(maxH));
        QuestTransmitter.Send(RadiusMessage);
    }
    public void SendGazeTime(float gazetime)
    {
        // Create message

        var gazeTimeMessage = new OSCMessage("/gazetime"); 
        gazeTimeMessage.AddValue(OSCValue.Float(gazetime));
        QuestTransmitter.Send(gazeTimeMessage);        
    }
    
    public void SendGenerateTargetBool(bool trigger)
    {
        var generateTargetBoolMessage = new OSCMessage("/generateTargetBool");
        generateTargetBoolMessage.AddValue(OSCValue.Bool(trigger));
        QuestTransmitter.Send(generateTargetBoolMessage);
    }
    public void SendGenerateTarget(int area, int cond, int sign)
    {
        // Create message       
        var generateTargetMessage = new OSCMessage("/generateTarget");        
        generateTargetMessage.AddValue(OSCValue.Int(area));
        generateTargetMessage.AddValue(OSCValue.Int(cond));
        generateTargetMessage.AddValue(OSCValue.Int(sign));
        QuestTransmitter.Send(generateTargetMessage);
    }

    public void SendDestroyObj(bool destroy)
    {
        var destroyMessage = new OSCMessage("/destroy");
        destroyMessage.AddValue(OSCValue.Bool(destroy));
        QuestTransmitter.Send(destroyMessage);
    }

    public void SendfNIRSTrigger(bool startTask, bool sendTrigger)
    {
        var fNIRSTriggerMessage = new OSCMessage("/fNIRSTrigger");
        fNIRSTriggerMessage.AddValue(OSCValue.Int((int)whichCondition));        
        fNIRSTriggerMessage.AddValue(OSCValue.Bool(startTask));
        fNIRSTriggerMessage.AddValue(OSCValue.Bool(sendTrigger));
        fNIRSTransmitter.Send(fNIRSTriggerMessage);
    }

    public void changeIP()
    {
        
        QuestHost = QIPInputField.text;
        QuestRemotePort = int.Parse(QRPortInputField.text);
        QuestLocalPort = int.Parse(QLPortInputField.text);

        PCHost = PCIPInputField.text;
        PCRemotePort = int.Parse(PCRPortInputField.text);
        PCLocalPort = int.Parse(PCLPortInputField.text);

        fNIRSHost = fNIRSIPInputField.text;
        fNIRSRemotePort = int.Parse(fNIRSRPortInputField.text);
        fNIRSLocalPort = int.Parse(fNIRSLPortInputField.text);

        // Set remote host address.
        PCTransmitter.RemoteHost = PCHost;

        // Set remote host address.
        QuestTransmitter.RemoteHost = QuestHost;

        // Set remote host address.
        fNIRSTransmitter.RemoteHost = fNIRSHost;

        // Set remote port;
        QuestTransmitter.RemotePort = QuestRemotePort;
        PCTransmitter.RemotePort = PCRemotePort;
        fNIRSTransmitter.RemotePort = fNIRSRemotePort;

        // Set local port.
        QuestReceiver.LocalPort = QuestLocalPort;
        PCReceiver.LocalPort = PCLocalPort;
        fNIRSReceiver.LocalPort = fNIRSLocalPort;
    }

    private void OnApplicationQuit()
    {
        startStudy = false;
        SendStartStudy(startStudy);
        SendDestroyObj(destroyObj);
        SendPassthrough(true);
    }
}
