using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInitializer : BaseIntializer
{
    public Room sourceRoom, targetRoom;
    public WallDirection wallDirection;
    public float soruceWeight, targetWeight;

    public DoorInitializer(GlobalDoorSetting globalDoorSetting, DoorSetting doorSetting, Room v, Room u) : base(globalDoorSetting.size, globalDoorSetting.height, globalDoorSetting.material, true) {
        this.sourceRoom = v;
        this.targetRoom = u;
        this.wallDirection = doorSetting.wallDirection;
        this.soruceWeight = doorSetting.soruceWeight;
        this.targetWeight = doorSetting.targetWeight;
    }
}
