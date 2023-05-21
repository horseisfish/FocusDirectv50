using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class NBackReader : MonoBehaviour
{

    public TextAsset textAssetData;
    

    [System.Serializable]
    public class Question
    {
        public string alphabet;
        public int correspnum;
        public bool corresp;
    }

    [System.Serializable]
    public class QuestionList
    {
        public Question[] questions;
    }

    public QuestionList myQuestionList = new QuestionList();

    
    // Start is called before the first frame update
    void Start()
    {
        ReadCSV();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ReadCSV()
    {
        string[] data = textAssetData.text.Split(new string[] { ",", "\n" }, System.StringSplitOptions.None);
        
        //-1 means ignore first row
        int tableSize = data.Length / 2 - 1;
        myQuestionList.questions = new Question[tableSize];

        for(int i=0; i<tableSize; i++)
        {
            myQuestionList.questions[i] = new Question();
            myQuestionList.questions[i].alphabet = data[2 * (i + 1)];
            myQuestionList.questions[i].correspnum =int.Parse( data[2 * (i + 1) + 1]);
            if (myQuestionList.questions[i].correspnum == 1)
            {
                myQuestionList.questions[i].corresp = true;
            }
            else
            {
                myQuestionList.questions[i].corresp = false;
            }
        }

    }

}
