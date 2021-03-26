using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInitializer : BaseIntializer
{
    public RoomInitializer(GlobalRoomSetting globalRoomSetting, RoomSetting roomSetting) : base(roomSetting.size, globalRoomSetting.height, globalRoomSetting.material, true) {
    }
}
