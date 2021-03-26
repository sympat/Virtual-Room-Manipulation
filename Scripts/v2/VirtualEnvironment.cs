using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualEnvironment : MonoBehaviour
{
    public GlobalRoomSetting globalRoomSetting;
    public GlobalDoorSetting globalDoorSetting;
    public UserSetting userSetting;
    public RoomSetting[] roomSettings;
    public DoorSetting[] doorSettings;

    private Dictionary<Room, List<Door>> adjList;
    private User user;
    private Room currentRoom;
    private static int totalID = 1;
    private int id;

    private bool isFirst = true;

    private void Awake()
    {
        id = totalID++;
        adjList = new Dictionary<Room, List<Door>>();

        // add rooms
        foreach (var roomSetting in roomSettings)
        {
            RoomInitializer init = new RoomInitializer(globalRoomSetting, roomSetting);
            AddRoom(init);
        }

        // set startRoom as currentRoom
        currentRoom = GetRoom(globalRoomSetting.startRoom);

        // connect rooms (generate door)
        foreach (var doorSetting in doorSettings) {
            Room sourceRoom = GetRoom(doorSetting.sourceRoom);
            Room targetRoom = GetRoom(doorSetting.targetRoom);
            DoorInitializer init = new DoorInitializer(globalDoorSetting, doorSetting, sourceRoom, targetRoom);
            Door currentDoor = ConnectRoom(init);

            HideRoom(sourceRoom);
            HideRoom(targetRoom);
        }

        ShowRoom(currentRoom);
        foreach(var connectedRoom in GetConnectedRooms(currentRoom)) {
            ShowRoom(connectedRoom);
        }

        // create user
        UserInitializer userInit = new UserInitializer();
        CreateUser(userInit);

        // place all door and room
        PlaceAllDoorAndRoom();
    }

    public Room GetCurrentRoom() {
        return currentRoom;
    }

    public User GetUser() {
        return user;
    }

    public Room GetRoom(int id) {
        foreach (var kv in adjList)
        {
            if (kv.Key.ID == id)
                return kv.Key;
        }

        return null;
    }

    public Room GetRoom(Room v)
    {
        int roomID = v.ID;

        return GetRoom(roomID);
    }

    public List<Room> GetConnectedRooms(Room v, bool containSelf = false)
    {
        if (GetRoom(v) == null)
            return null;

        List<Room> result = new List<Room>();
        foreach(var door in adjList[v])
        {
            result.Add(door.GetConnectedRoom(v));
        }

        if (containSelf)
            result.Add(v);

        return result;
    }

    public List<Door> GetConnectedDoors(Room v)
    {
        return adjList[v];
    }

    public void CreateUser(UserInitializer init) {
        user = (GameObject.Instantiate(userSetting.userPrefab, this.transform)).GetComponentInChildren<User>();
        // user.transform.SetParent(this.transform);
        // user.Initializing(init);
    }
    
    public void AddRoom(RoomInitializer init) {
        Room room = (new GameObject()).AddComponent<Room>();
        room.transform.SetParent(this.transform);
        room.Initializing(init);

        GameObject pointLightObj = new GameObject("Light");
        pointLightObj.AddComponent<Light>();
        pointLightObj.transform.position = new Vector3(0, room.Height - 0.3f, 0);
        pointLightObj.transform.SetParent(room.transform);
        pointLightObj.SetActive(false);

        adjList.Add(room, new List<Door>());
    }

    public Door ConnectRoom(DoorInitializer init) {
        Door door = (GameObject.Instantiate(globalDoorSetting.doorPrefab)).GetComponent<Door>();
        door.transform.SetParent(this.transform);
        door.Initializing(init);

        adjList[init.sourceRoom].Add(door);
        adjList[init.targetRoom].Add(door);

        return door;
    }

    public void PlaceAllDoorAndRoom() {
        Dictionary<Room, bool> visited = new Dictionary<Room, bool>();
        foreach (var kv in adjList)
        {
            visited.Add(kv.Key, false);
        }

        Queue<Room> q = new Queue<Room>();

        Room v = GetRoom(currentRoom);
        visited[v] = true;
        q.Enqueue(v);

        Room u;
        while (q.Count != 0)
        {
            u = q.Dequeue(); // currentRoom
            foreach (var door in adjList[u])
            {
                Room w = GetRoom(door.GetConnectedRoom(u)); // nextRoom

                if (!visited[w])
                {
                    visited[w] = true;
                    
                    if(!isFirst && u == currentRoom && user.IsTargetInUserFov(door.Position)) {
                        door.UpdateDoorWeight(door.Position, u);
                    }
                    else {
                        door.PlaceDoorAndConnectedRoom(u);
                    }

                    q.Enqueue(w);
                }
            }
        }

        isFirst = false;
    }

    public void MoveDoor(Room v, Door d, float translate)
    {
        d.SetWeightInThisRoom(v, translate);
        // UpdateAllRoom(v);
        PlaceAllDoorAndRoom();
    }

    public void MoveWall(Room v, int wall, float translate)
    {
        v.MoveEdge(wall, translate); // 현재 방의 wall을 이동시킨다

        PlaceAllDoorAndRoom();
    }


    public void HideRoom(Room room) {
        room.transform.GetChild(0).gameObject.SetActive(false);

        foreach(var connectedDoor in GetConnectedDoors(room)) {
            connectedDoor.HideObject();
        }
        room.HideObject();
    }

    public void ShowRoom(Room room) {
        room.transform.GetChild(0).gameObject.SetActive(true);

        room.ShowObject();
        foreach(var connectedDoor in GetConnectedDoors(room)) {
            connectedDoor.ShowObject();
        }
    }

    // public void ChangeCurrentRoom(Door door) {
    //     Room targetRoom = door.GetConnectedRoom(currentRoom); // 다음 currentRoom

    //     foreach(var connectedRoom in GetConnectedRooms(currentRoom)) {
    //         HideRoom(connectedRoom);
    //     }
    //     HideRoom(currentRoom);

    //     ShowRoom(targetRoom);
    //     foreach(var connectedRoom in GetConnectedRooms(targetRoom)) {
    //         ShowRoom(connectedRoom);
    //     }

    //     currentRoom = targetRoom;
    // }

    private void Update() {

        Room nextRoom = CheckCurrentRoom();

        if(!nextRoom.Equals(currentRoom)) {
            // Debug.Log(nextRoom);
            GameObject.FindWithTag("Manipulation").GetComponent<Manipulation>().SwitchEnable();

            foreach(var connectedRoom in GetConnectedRooms(currentRoom)) {
                HideRoom(connectedRoom);
            }
            HideRoom(currentRoom);

            ShowRoom(nextRoom);
            foreach(var connectedRoom in GetConnectedRooms(nextRoom)) {
                ShowRoom(connectedRoom);
            }

            currentRoom = nextRoom;
        }
    }

    public Room CheckCurrentRoom()
    {
        Room result = currentRoom;
        foreach(var kv in adjList)
        {
            Room room = kv.Key;

            if (room.IsContain(user.Position))
            {
                result = room;
                break;
            }
        }

        return result;
    }
}
