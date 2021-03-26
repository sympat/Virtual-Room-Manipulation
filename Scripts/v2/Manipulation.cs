﻿using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class Manipulation : MonoBehaviour
{
    private static float DT = 0.2f;
    private static float doorDT = 0.01f;

    private UnityEvent onStart = new UnityEvent();
    private UnityEvent onEnd = new UnityEvent();

    public VirtualEnvironment VE;
    public Base2D realSpace;

    public bool[] CheckWallMovable(VirtualEnvironment virtualEnvironment, Room currentRoom, User user, float[] translate) // translate은 xx,y 좌표계 기준으로 부호가 결정
    {
        bool[] isMovable = new bool[4];
        for (int i = 0; i < 4; i++)
        {
            isMovable[i] = true; // i와 i+1 사이 wall
        }

        // 벽이 사용자와 충분히 멀리 있는지를 판단
        for (int i = 0; i < 4; i++)
        {
            Vector2 wallPosition = currentRoom.GetEdge2D(i, Space.World);

            if (i % 2 == 0 && Mathf.Abs(wallPosition.y - user.Position.y) < 0.2f) // 현재 벽 i가 0, 2번 벽이고 user와 너무 가까운 경우, 0.1f는 여유 boundary , user.Size.y / 2
            {
                isMovable[i] = false;
            }
            else if (i % 2 != 0 && Mathf.Abs(wallPosition.x - user.Position.x) < 0.2f) // user.Size.x / 2
            {
                isMovable[i] = false;
            }
        }

        // 벽이 사용자 시야에 있는지를 판단
        for (int i = 0; i < 4; i++)
        {
            Vector2 vertexPosition = currentRoom.GetVertex2D(i, Space.World);
            Vector2 wallPosition = currentRoom.GetEdge2D(i, Space.World);

            if (user.IsTargetInUserFov(vertexPosition))
            {
                isMovable[Utility.mod(i, 4)] = false;
                isMovable[Utility.mod(i - 1, 4)] = false;
            }

            if(user.IsTargetInUserFov(wallPosition))
            {
                isMovable[Utility.mod(i, 4)] = false;
            }
        }

        return isMovable;
    }

    public Tuple<Vector2, Vector2> GetScaleTranlslate(Room currentRoom, Base2D realSpace) // v is currentRoom
    {
        Room v = currentRoom;
        Vector2 Scale = new Vector2(v.OriginSize.x / v.Size.x, v.OriginSize.y / v.Size.y);
        Vector2 Translate = realSpace.Position - v.Position;

        return new Tuple<Vector2, Vector2>(Scale, Translate);
    }

    public void Restore(VirtualEnvironment virtualEnvironment, Room currentRoom, User user, Vector2 scale, Vector2 translate)
    {
        Room v = currentRoom;

        float[] wallMovement = new float[4]; // Sign을 결정하는 역할만 함
        wallMovement[0] = (scale.y - 1) * v.Size.y / 2 + translate.y;
        wallMovement[1] = (1 - scale.x) * v.Size.x / 2 + translate.x;
        wallMovement[2] = (1 - scale.y) * v.Size.y / 2 + translate.y;
        wallMovement[3] = (scale.x - 1) * v.Size.x / 2 + translate.x;

        bool[] isWallMovable = CheckWallMovable(virtualEnvironment, currentRoom, user, wallMovement);

        //wall을 이동 시키는 로직 start
        for (int i = 0; i < 4; i++)
        {
            if (isWallMovable[i])
            {
                float translation = Mathf.Sign(wallMovement[i]) * DT * Time.deltaTime;
                virtualEnvironment.MoveWall(v, i, translation);
            }
        }
        // end
    }

    public void Reduce(VirtualEnvironment virtualEnvironment, Room targetRoom, Base2D realSpace)
    {
        float xMinDist = realSpace.Min.x - targetRoom.Min.x;
        float xMaxDist = realSpace.Max.x - targetRoom.Max.x;
        float yMinDist = realSpace.Min.y - targetRoom.Min.y;
        float yMaxDist = realSpace.Max.y - targetRoom.Max.y;

        if (xMinDist > 0) // 1벽
        {
            virtualEnvironment.MoveWall(targetRoom, 1, xMinDist);
        }
        if (xMaxDist < 0) // 3벽
        {
            virtualEnvironment.MoveWall(targetRoom, 3, xMaxDist);
        }

        if (yMinDist > 0) // 2벽
        {
            virtualEnvironment.MoveWall(targetRoom, 2, yMinDist);
        }
        if (yMaxDist < 0) // 0벽
        {
            virtualEnvironment.MoveWall(targetRoom, 0, yMaxDist);
        }

    }

    static bool NeedAdjust(VirtualEnvironment virtualEnvironment, Room currentRoom)
    {
        List<Door> connectedDoors = virtualEnvironment.GetConnectedDoors(currentRoom);

        foreach(var door in connectedDoors) {
            if(Mathf.Abs(door.vWeight - door.OriginVWeight) > 0.01f)
                return true;
        }

        return false;
    }

    void Adjust(VirtualEnvironment virtualEnvironment, Room currentRoom, User user)
    {
        List<Door> connectedDoors = virtualEnvironment.GetConnectedDoors(currentRoom);

        foreach(var door in connectedDoors) {
            if(!user.IsTargetInUserFov(door.Position))
                virtualEnvironment.MoveDoor(currentRoom, door, door.GetOriginWeightInThisRoom(currentRoom));
        }
    }

    public void SwitchEnable() {
        this.enabled = !this.enabled;
    }

    private void Awake() {
        onStart.AddListener(GameObject.FindWithTag("Task").GetComponent<CoinTask>().SetTaskStart);
        onEnd.AddListener(GameObject.FindWithTag("Task").GetComponent<CoinTask>().SetTaskEnd);
    }

    private void OnEnable() {
        onStart.Invoke();
    }

    private void OnDisable() {
        onEnd.Invoke();
    }

    private void Update() 
    {
        Debug.Log("Update!");
        VirtualEnvironment virtualEnvironment = VE;
        Room currentRoom = VE.GetCurrentRoom();
        User user = VE.GetUser();

        // 알고리즘 시작
        Tuple<Vector2, Vector2> st = GetScaleTranlslate(currentRoom, realSpace); 
        Vector2 scale = st.Item1, translate = st.Item2;

        if ((scale - Vector2.one).magnitude > 0.01f || (translate - Vector2.zero).magnitude > 0.01f) // 복원 연산
        {
            Restore(virtualEnvironment, currentRoom, user, scale, translate);
        }
        else if(NeedAdjust(virtualEnvironment, currentRoom)) // 조정 연산
        {
            Adjust(virtualEnvironment, currentRoom, user);
        }
        else // 축소 연산
        {
            List<Room> neighborRooms = virtualEnvironment.GetConnectedRooms(currentRoom);
            foreach (var room in neighborRooms)
            {
                if (!room.IsInside(realSpace))
                    Reduce(virtualEnvironment, room, realSpace);
            }

            SwitchEnable();
        }
        // 알고리즘 끝
    }
}
