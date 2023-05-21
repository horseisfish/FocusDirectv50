using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL4Unity.Utils;
using LSL;

namespace LSL4Unity.Samples.Complex
{
    public class AuroraTriggerWithoutnBack : AStringOutlet
    {
        public string Source_ID = "Aurora";
        public string whichCondition;
        public bool startTesk = false;
        public bool sendTrigger = false;

        public override List<string> ChannelNames
        {
            get
            {
                List<string> chanNames = new List<string>{ "Event" };
                return chanNames;
            }
        }

        public void Reset()
        {
            StreamName = "Trigger";
            StreamType = "Markers";
            moment = MomentForSampling.EndOfFrame;
            IrregularRate = true;
        }

        protected override void Start()
        {
            StreamInfo streamInfo = new StreamInfo(StreamName, StreamType, 1, LSL.LSL.IRREGULAR_RATE,channel_format_t.cf_int32,Source_ID);
            base.Start();
        }

        // Update is called once per frame
        protected override bool BuildSample()
        {
            if (startTesk)
            {
                // Called by Update because our moment is EndOfFrame.
                if (sendTrigger)
                {
                    //sample[0] = 1.ToString();
                    sample[0] = whichCondition;
                    //Debug.Log("triggersent");
                    sendTrigger = false;
                    return true;
                    
                }
               
                
            }
            return false;
        }

        public void SendTrigger()
        {
            sendTrigger = true;
        }
    }
}