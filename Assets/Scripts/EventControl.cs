using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class EventControl : MonoBehaviour
{
    [Header("Script append")]
    [SerializeField]
    GameObject AuroraGameObject;
    [SerializeField]
    GameObject nBackReaderObject;
    [SerializeField]
    GameObject targetGenerateObject;
    [SerializeField]
    GameObject eyeTrackingLeftObject;
    [SerializeField]
    GameObject eyeTrackingRightObject;
    [SerializeField]
    GameObject questionDisplayObject;

    //LSL4Unity.Samples.Complex.AuroraTrigger auroraTrigger;
    NBackReader nBackReaderFile;
    TargetGenerater targetGenerater;
    EyeTrackingRay LefteyeTracking;
    EyeTrackingRay RighteyeTracking;
    QuestionDisplayer questionDisplayer;

    public string filePath = @"C:\temp\output.csv";
    public string UserID;
    public enum signType
    {
        square = 0,
        circle = 1,
        star =2
    }
    public enum conditionType
    {
        sourcePlan = 0,
        targetPlan = 1,
        HUDPlan = 2,
        Sound = 3 
    }

    public signType whichSigh;
    public conditionType whichCondition = conditionType.targetPlan;

    public int trialRepeatTime;
    public int currentTrial;
    public float trialTime;
    public float trialRestTime;
    public int n_Back;
    public float alphabetShowTime;
    public float focusTime;
    public int TargetNumbers;
    public List<targetStruct> targetList;
    public int targetIndex;
    public float targetFindingTime;
    public float targetRestTime;
    public List<float> targetFindingTimeList;
    public List<bool> targetFindingResultList;
    public List<bool> nBackResult;
    public float timer;
    public bool startStudy;
    public bool targeAlive;
    public bool trailRunning;
    public float gazeTimeIs;
    public float leftGazeTimer;
    public float RightGazeTimer;
    public bool TargetFined;
    public bool isCoroutineRunning = false;
    private bool isFinished = false;

    public TextAsset oneBackData;
    public TextAsset twoBackData;
    public TextAsset treeBackData;

    private Coroutine generateCoroutine;

    public struct targetStruct
    {
        public int area;
        public int cond;
        public int sign;

    }


    // Start is called before the first frame update
    void Start()
    {
        //auroraTrigger = AuroraGameObject.GetComponent<LSL4Unity.Samples.Complex.AuroraTrigger>();
        nBackReaderFile = nBackReaderObject.GetComponent<NBackReader>();
        targetGenerater = targetGenerateObject.GetComponent<TargetGenerater>();
        LefteyeTracking = eyeTrackingLeftObject.GetComponent<EyeTrackingRay>();
        RighteyeTracking = eyeTrackingRightObject.GetComponent<EyeTrackingRay>();
        questionDisplayer = questionDisplayObject.GetComponent<QuestionDisplayer>();
        targetList = new List<targetStruct>();
        targetFindingTimeList = new List<float>();
        targetFindingResultList = new List<bool>();
        nBackResult = new List<bool>();
        //auroraTrigger.FixationInterval = focusTime;
        //auroraTrigger.TrialInterval = alphabetShowTime;
        switch (n_Back)
        {
            case 1:
                nBackReaderFile.textAssetData = oneBackData;
                break;

            case 2:
                nBackReaderFile.textAssetData = twoBackData;
                break;

            case 3:
                nBackReaderFile.textAssetData = treeBackData;
                break;
        }
        timer = 0f;
        currentTrial = 0;
        targetFindingTime = 0f;
        for (int i = 0; i < nBackReaderFile.myQuestionList.questions.Length; i++)
        {
            nBackResult.Add(false);
        }

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
        LefteyeTracking.gazeTime = gazeTimeIs;
        RighteyeTracking.gazeTime = gazeTimeIs;
        TargetFined = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (startStudy)
        {
            leftGazeTimer = LefteyeTracking.gazeTimer;
            RightGazeTimer = RighteyeTracking.gazeTimer;

            if (currentTrial <= trialRepeatTime - 1)
            {
                trailRunning = true;
                timer += Time.deltaTime;
                
                if (timer < trialTime)
                {
                    //auroraTrigger.startTesk = true;
                    //Debug.Log(questionDisplayer.currentQuestion);
                    if (OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger)!=0)
                    {
                        if (questionDisplayer.nbackCorr)
                        {
                            nBackResult[questionDisplayer.currentQuestion] = true;
                        }
                        else
                        {
                            nBackResult[questionDisplayer.currentQuestion] = false;
                        }
                    }
                    else
                    {
                       nBackResult[questionDisplayer.currentQuestion] = true;
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
                    if (LefteyeTracking.gazed == false || RighteyeTracking.gazed == false)
                        {
                        if (targeAlive)
                        {
                            targetFindingTime += Time.deltaTime;
                        }
                            
                        }
                        if (LefteyeTracking.gazed || RighteyeTracking.gazed)
                        {
                        if (OVRInput.Get(OVRInput.RawButton.A))
                        {
                            Debug.Log("targetsign is" + targetList[targetIndex].sign);
                            //Debug.Log(leftGazeTimer + "," + RightGazeTimer);
                            //Debug.Log(LefteyeTracking.gazed + "," + RighteyeTracking.gazed);
                            if (targetList[targetIndex].sign == 0)
                            {
                                Debug.Log("correct");
                                targetFindingResultList[targetIndex] = true;
                            }
                            else
                            {
                                Debug.LogWarning("false");
                                targetFindingResultList[targetIndex] = false;
                            }
                            targetFindingTimeList[targetIndex] = targetFindingTime;
                            targetFindingTime = 0f;
                            targetIndex++;
                            Destroy(targetGenerater.obj);
                            targeAlive = false;
                            StopCoroutine(generateCoroutine);
                            generateCoroutine = null;
                        }
                        if (OVRInput.Get(OVRInput.RawButton.B))
                        {
                            
                            Debug.Log("targetsign is" + targetList[targetIndex].sign);
                            if (targetList[targetIndex].sign == 0)
                            {
                                Debug.LogWarning("false");
                                targetFindingResultList[targetIndex] = false;
                            }
                            else
                            {
                                Debug.Log("correct");
                                targetFindingResultList[targetIndex] = true;
                            }
                            targetFindingTimeList[targetIndex]=targetFindingTime;
                            targetFindingTime = 0f;
                            targetIndex++;
                            Destroy(targetGenerater.obj);
                            targeAlive = false;
                            StopCoroutine(generateCoroutine);
                            generateCoroutine = null;
                        }
                            

                        }

                    
                }
                else if (timer < trialTime + trialRestTime)
                {
                    //auroraTrigger.startTesk = false;
                    SaveToCSV();
                    questionDisplayer.displayText.text = "Take a Rest";
                    Destroy(targetGenerater.obj);
                    targeAlive = false;
                    if (generateCoroutine != null)
                    {
                        StopCoroutine(generateCoroutine);
                        generateCoroutine = null;
                    }


                }
                else
                {
                    
                    currentTrial++;
                    shuffleList(targetList);
                    targetIndex = 0;
                    timer = 0f;
                    targetFindingTime = 0f;
                    trailRunning = false;
                    Destroy(targetGenerater.obj);
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
                questionDisplayer.displayText.text = "The End of The Study";
                Destroy(targetGenerater.obj);
                StopAllCoroutines();
            }
        }
    }

    public void startTask()
    {
        startStudy = true;
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
            targetGenerater.GenerateTarget(targetList[targetIndex].area,targetList[targetIndex].cond, targetList[targetIndex].sign);
            targeAlive = true;
            //Debug.Log(targeAlive);
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
        using (StreamWriter writer = new StreamWriter(filePath+ "User" + UserID+"nBack" + whichCondition + "trial" + currentTrial + ".csv"))
        {
            writer.WriteLine("Nback result");

            for (int i = 0; i < nBackReaderFile.myQuestionList.questions.Length; i++)
            {
                string nbackResult = nBackResult[i].ToString();              
                writer.WriteLine(nbackResult);
            }

        }
    }
}
