using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL4Unity.Utils;
using LSL;

namespace LSL4Unity.Samples.Complex
{
    public class AuroraTrigger: AStringOutlet
    {
        public string Source_ID = "Aurora";
        public float FixationInterval = 1.0f;
        public float TrialInterval = 3.4f;
        public bool showFixation = false;
        public bool showCurrentQuestion = false;
        public bool timesUp = false;
        public float elapsed_time = 0.0f;
        public bool startTesk = false;
        public bool endOfTrial = false;

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
                elapsed_time += Time.deltaTime;
                if (elapsed_time <= FixationInterval)
                {
                    showFixation = true;
                    showCurrentQuestion = false;
                }
                else if (elapsed_time > FixationInterval)
                {
                    showFixation = false;
                    showCurrentQuestion = true;
                }
                if (elapsed_time >= TrialInterval + FixationInterval)
                {
                    
                    timesUp = true;
                    elapsed_time = 0.0f;
                    
                }
                else
                {
                    timesUp = false;
                }

                if (endOfTrial)
                {
                    sample[0] = 1.ToString();
                    return true;
                }
               
                
            }
            return false;
        }
    }
}