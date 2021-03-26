using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WallDirection { North, West, South, East }

[System.Serializable]
public class DoorSetting
{
    public int sourceRoom, targetRoom;
    public WallDirection wallDirection;
    [Range(-1, 1)]
    public float soruceWeight, targetWeight;
}
