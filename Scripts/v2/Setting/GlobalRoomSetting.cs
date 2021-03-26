using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GlobalRoomSetting 
{
    private static GlobalRoomSetting instance = null;
    public static GlobalRoomSetting Instance
    {
        get
        {
            if (instance == null)
                instance = new GlobalRoomSetting();
            return instance;
        }
    }

    public int startRoom;
    public float height;
    public Material material;
    public GameObject roomPrefab;
}
