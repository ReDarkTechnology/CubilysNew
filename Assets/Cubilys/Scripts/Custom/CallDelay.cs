using UnityEngine;
using System;
using System.Collections.Generic;

public class CallDelay : MonoBehaviour
{
    public static List<CallInstruction> instructions = new List<CallInstruction>();

    private void Update()
    {
        var destroyList = new List<CallInstruction>();
        foreach(var inst in instructions)
        {
            var t = inst.delay - Time.deltaTime;
            if(t <= 0)
            {
                inst.delay = 0;
                if(inst.afterDelay != null) inst.afterDelay.Invoke();
                destroyList.Add(inst);
            }
            else
            {
                inst.delay = t;
            }
        }
        foreach(var d in destroyList)
        {
            instructions.Remove(d);
        }
    }

    public static CallInstruction Call(float delay, Action action)
    {
        var instruction = new CallInstruction(delay, action);
        instructions.Add(instruction);
        return instruction;
    }
}

[Serializable]
public class CallInstruction
{
    public float delay;
    public Action afterDelay;

    public CallInstruction(float delay, Action action)
    {
        this.delay = delay;
        afterDelay = action;
    }
}
