using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.LSL4Unity.Scripts;

public class LSL4UnityTest : MonoBehaviour
{

    public LSLMarkerStream maker;
    public bool sendTrigger;
    // Start is called before the first frame update
    void Start()
    {
        maker = GetComponent<LSLMarkerStream>();
    }

    // Update is called once per frame
    void Update()
    {
        sendtrigger(sendTrigger);
    }

    public void sendtrigger(bool trigger)
    {
        if (trigger)
        {
            maker.Write(1.ToString());
            trigger = false;
        }
        
    }
}
