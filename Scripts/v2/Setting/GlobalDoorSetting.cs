using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GlobalDoorSetting
{
    private static GlobalDoorSetting instance = null;
    public static GlobalDoorSetting Instance
    {
        get
        {
            if (instance == null)
                instance = new GlobalDoorSetting();
            return instance;
        }
    }

    public Vector2 size;
    public float height;
    public Material material;

    public GameObject doorPrefab;
}
 