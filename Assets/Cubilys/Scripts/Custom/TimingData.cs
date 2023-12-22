using System;
using System.Collections.Generic;

namespace OsuToCubilys
{
    [Serializable]
    public class TimingData
    {
        public string name = "Unnamed";
        public List<HitPoint> hitPoints = new List<HitPoint>();
        public List<BPMPoint> bpms = new List<BPMPoint>();
    }

    [Serializable]
    public class HitPoint
    {
        public float x;
        public float time;

        public HitPoint() { }
        public HitPoint(float x, float time)
        {
            this.x = x;
            this.time = time;
        }
    }

    [Serializable]
    public class BPMPoint
    {
        public float time;
        public float bpm = 120;

        public BPMPoint() { }
        public BPMPoint(float time, float bpm)
        {
            this.time = time;
            this.bpm = bpm;
        }
    }
}
