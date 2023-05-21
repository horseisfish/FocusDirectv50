using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class QuestionDisplayer : MonoBehaviour
{

    NBackReader questiondata;
    LSL4Unity.Samples.Complex.AuroraTrigger triggerData;

    [SerializeField]
    GameObject question;

    [SerializeField]
    GameObject trigger;

    [SerializeField]
    public TMP_Text displayText;

    [SerializeField]
    public int currentQuestion = 0;

    public bool nbackCorr;
    // Start is called before the first frame update
    void Start()
    {
        questiondata = question.GetComponent<NBackReader>();
        triggerData = trigger.GetComponent<LSL4Unity.Samples.Complex.AuroraTrigger>();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentQuestion < questiondata.myQuestionList.questions.Length)
        {

            if (triggerData.showFixation)
            {
                displayText.text = "+";
            }

            if (triggerData.showCurrentQuestion)
            {
                displayText.text = questiondata.myQuestionList.questions[currentQuestion].alphabet;
                nbackCorr = questiondata.myQuestionList.questions[currentQuestion].corresp;
            }

            if (triggerData.timesUp)
            {
                currentQuestion++;
            }

        }
        else
        {
            displayText.text = "End of Test";
        }

    }

    
}
