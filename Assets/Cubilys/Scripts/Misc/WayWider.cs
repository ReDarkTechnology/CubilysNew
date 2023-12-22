using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WayWider : MonoBehaviour
{
    public List<Transform> trs = new List<Transform>();
    public List<TransformCache> tc = new List<TransformCache>();

    public Vector3 wideRate = new Vector3(1, 1, 1);
    public bool widen;
    public bool undo;

    void Update()
    {
        if(widen)
        {
            trs.Clear();
            tc.Clear();
            GetAllTransforms();
            WideAllTransforms();
            widen = false;
        }

        if(undo)
        {
            UndoAll();
            tc.Clear();
            undo = false;
        }
    }

    public void GetAllTransforms()
    {
        trs = new List<Transform>(GetComponentsInChildren<Transform>());
        if (trs.Contains(transform)) trs.Remove(transform);
    }

    public void WideAllTransforms()
    {
        foreach(var t in trs)
        {
            tc.Add(new TransformCache(t));
            var s = t.localScale;
            var r = wideRate;
            t.transform.localScale = new Vector3(s.x + r.x, s.y + r.y, s.z + r.z);
        }
    }

    public void UndoAll()
    {
        foreach(var t in tc)
        {
            t.tr.localScale = t.scale;
        }
    }
}

[System.Serializable]
public class TransformCache
{
    public Vector3 scale;
    public Transform tr;

    public TransformCache(Transform t)
    {
        scale = t.localScale;
        tr = t;
    }
}