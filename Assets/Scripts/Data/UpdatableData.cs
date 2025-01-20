﻿using UnityEngine;
using System.Collections;

public class UpdatableData : ScriptableObject
{

    public event System.Action OnValuesUpdated;
    public bool autoUpdate;

    protected virtual void OnValidate()
    {
#if UNITY_EDITOR
        if (autoUpdate)
        {
            UnityEditor.EditorApplication.update += NotifyOfUpdatedValues;
        }
#endif
    }

    public void NotifyOfUpdatedValues()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.update -= NotifyOfUpdatedValues;
#endif
        if (OnValuesUpdated != null)
        {
            OnValuesUpdated();
        }
    }

}
