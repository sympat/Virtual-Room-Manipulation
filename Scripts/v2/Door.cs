using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Base2D
{
    private static int totalID = 0;
    private int id;
    public Room v, u;
    public float vWeight, uWeight;  // [-1, 1] 로 고정
    private float originVWeight, originUWeight;

    public WallDirection wallDirection;

    public float OriginVWeight {
        get {
            return originVWeight;
        }
    }

    public float OriginUWeight {
        get {
            return originUWeight;
        }
    }
    
    public int vWall {
        get {
            switch(wallDirection) {
                case WallDirection.North:
                    return 0;
                case WallDirection.West:
                    return 1;
                case WallDirection.South:
                    return 2;
                case WallDirection.East:
                    return 3;
                default:
                    throw new System.Exception("Invalid wall direction");
            }
        }
    }

    public int uWall {
        get {
            return (vWall + 2) % 4;
        }
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        Door objAsRoom = obj as Door;
        if (objAsRoom == null) return false;
        else return Equals(objAsRoom);
    }

    public bool Equals(Door v)
    {
        if (v == null) return false;
        else return (this.id == v.id);
    }

        public override int GetHashCode()
    {
        return this.id; // TODO: 이런식으로 Hash 값을 지정해도 되나?
    }

    public override string ToString()
    {
        string result = "";
        result += string.Format("ID: {0}", id);
        result += string.Format(", ObjName: {0}", this.gameObject.name);
        result += "\n vWeight: " + vWeight;
        result += "\n uWeight: " + uWeight;
        result += "\n\t" + v;
        result += "\n\t" + u;

        return result;
    }

    public override void Initializing(BaseIntializer init)
    {
        DoorInitializer doorInit = init as DoorInitializer;
        base.Initializing(init);

        this.id = totalID++;
        this.gameObject.name = "Door_" + this.id;
        this.wallDirection = doorInit.wallDirection;
        this.v = doorInit.sourceRoom;
        this.u = doorInit.targetRoom;

        this.vWeight = doorInit.soruceWeight;
        this.uWeight = doorInit.targetWeight;
        this.originVWeight = this.vWeight;
        this.originUWeight = this.uWeight;
    }

    // public override void InitializePosition()
    // {
    //     float doorXPos = 0, doorYPos = 0;

    //     if (vWall == 0 || vWall == 2)
    //     {
    //         doorXPos = (vWeight + 1) / 2 * (v.Max.x - v.Min.x) + v.Min.x; // 방 v 기준 문의 x축 위치
    //         doorYPos = (vWall == 0) ? v.Max.y : v.Min.y;
    //     }
    //     else if (vWall == 1 || vWall == 3)
    //     {
    //         doorXPos = (vWall == 3) ? v.Max.x : v.Min.x;
    //         doorYPos = (vWeight + 1) / 2 * (v.Max.y - v.Min.y) + v.Min.y;
    //     }

    //     this.Position = new Vector2(doorXPos, doorYPos);
    // }

    public override void HideObject() {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public override void ShowObject()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void PlaceDoorAndConnectedRoom(Room currentRoom) {
        float doorXPos = 0, doorYPos = 0, roomXPos = 0, roomYPos = 0;

        Room v = GetThisRoom(currentRoom);
        Room u = GetConnectedRoom(currentRoom);
        int vWall = GetWallinThisRoom(currentRoom);
        float vWeight = GetWeightInThisRoom(currentRoom);
        float uWeight = GetWeightInOtherRoom(currentRoom);

        if (vWall == 0 || vWall == 2)
        {
            doorXPos = (vWeight + 1) / 2 * (v.Max.x - v.Min.x) + v.Min.x; // 방 v 기준 문의 x축 위치
            doorYPos = (vWall == 0) ? v.Max.y : v.Min.y;

            roomXPos = doorXPos - ((uWeight * u.Size.x) / 2);
            roomYPos = (vWall == 0) ? v.Max.y + u.Extents.y : v.Min.y - u.Extents.y;
        }
        else if (vWall == 1 || vWall == 3)
        {
            doorXPos = (vWall == 3) ? v.Max.x : v.Min.x;
            doorYPos = (vWeight + 1) / 2 * (v.Max.y - v.Min.y) + v.Min.y;

            roomXPos = (vWall == 3) ? v.Max.x + u.Extents.x : v.Min.x - u.Extents.x;
            roomYPos = doorYPos - ((uWeight * u.Size.y) / 2);

            this.transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        }

        u.Position = new Vector2(roomXPos, roomYPos);
        this.Position = new Vector2(doorXPos, doorYPos);
    }

    public void UpdateDoorWeight(Vector2 currentDoorPos, Room currentRoom) 
    {
        Room v = GetThisRoom(currentRoom);
        int vWall = GetWallinThisRoom(currentRoom);
        float newWeight = 0;

        if (vWall == 0 || vWall == 2)
        {
            newWeight = 2 * (currentDoorPos.x - v.Min.x) / (v.Max.x - v.Min.x) - 1;

        }
        else if (vWall == 1 || vWall == 3)
        {
            newWeight = 2 * (currentDoorPos.y - v.Min.y) / (v.Max.y - v.Min.y) - 1;
        }

        SetWeightInThisRoom(currentRoom, newWeight);
    }

    public Room GetConnectedRoom(Room currentRoom)
    {
        if (currentRoom.Equals(v)) return u;
        else if (currentRoom.Equals(u)) return v;
        else return null;
    }

    public Room GetThisRoom(Room targetRoom)
    {
        if (targetRoom.Equals(v)) return v;
        else if (targetRoom.Equals(u)) return u;
        else return null;
    }

    public int GetWallinThisRoom(Room currentRoom)
    {
        if (currentRoom.Equals(v)) return vWall;
        else if (currentRoom.Equals(u)) return uWall;
        else throw new System.Exception("No Valid Room");
    }

    public int GetWallInOtherRoom(Room currentRoom)
    {
        if (currentRoom.Equals(v)) return uWall;
        else if (currentRoom.Equals(u)) return vWall;
        else throw new System.Exception("No Valid Room");
    }

    public float GetOriginWeightInThisRoom(Room currentRoom) {
        if (currentRoom.Equals(v)) return OriginVWeight;
        else if (currentRoom.Equals(u)) return OriginUWeight;
        else throw new System.Exception("No Valid Room");
    }
    
    public float GetOriginWeightInOtherRoom(Room currentRoom) {
        if (currentRoom.Equals(v)) return OriginUWeight;
        else if (currentRoom.Equals(u)) return OriginVWeight;
        else throw new System.Exception("No Valid Room");
    }

    public float GetWeightInThisRoom(Room currentRoom)
    {
        if (currentRoom.Equals(v)) return vWeight;
        else if (currentRoom.Equals(u)) return uWeight;
        else throw new System.Exception("No Valid Room");
    }

    public float GetWeightInOtherRoom(Room currentRoom)
    {
        if (currentRoom.Equals(v)) return uWeight;
        else if (currentRoom.Equals(u)) return vWeight;
        else throw new System.Exception("No Valid Room");
    }

    public void SetWeightInThisRoom(Room currentRoom, float weight)
    {
        if (currentRoom.Equals(v)) vWeight = weight;
        else if (currentRoom.Equals(u)) uWeight = weight;
        else throw new System.Exception("No Valid Room");
    }

    public bool CheckWallDirection()
    {
        if (vWall % 2 == 0 && uWall % 2 == 0) // true - y
            return true;
        else if (vWall % 2 != 0 && uWall % 2 != 0) // false - x
            return false;
        else
            throw new System.Exception("Invalid wall");
    }

    public int GetWall(Room targetRoom) // targetRoom에 있는 Door 가 어느 wall에 있는지를 반환
    {
        if (GetThisRoom(targetRoom) == v) // targetRoom이 v인 경우
        {
            return vWall;
        }
        else // u인 경우
        {
            return uWall;
        }
    }

    public bool IsInThisWall(Room v, int wall) // 방 v에 있는 door가 주어진 wall에 있는지를 반환
    {
        int currentWall = GetWall(v);

        if (currentWall == wall)
            return true;
        else
            return false;
    }

    public bool IsInNeighborWall(Room v, int wall) // 방 v에 있는 door가 neighbor wall (wall+1 or wall-1)에 있는지를 반환
    {
        int currentWall = GetWall(v);

        if (currentWall == Utility.mod(wall + 1, 4) || currentWall == Utility.mod(wall - 1, 4))
            return true;
        else
            return false;
    }

    public void MoveConnectedRoom(Room v, float translate, bool moveAlongWall = true)
    {
        Room u = GetConnectedRoom(v);

        if (CheckWallDirection()) // y축으로 서로 연결된 경우
        {
            if (moveAlongWall)
            {
                u.transform.Translate(Vector2.right * translate); // 연결된 방을 x축으로 이동시킨다
                this.transform.Translate(Vector2.right * translate); // 문을 x축으로 이동시킨다
            }
            else
            {
                u.transform.Translate(Vector2.up * translate); // 연결된 방을 y축으로 이동시킨다
                this.transform.Translate(Vector2.up * translate); // 문을 y축으로 이동시킨다
            }
        }
        else // x축으로 서로 연결된 경우
        {
            if (moveAlongWall)
            {
                u.transform.Translate(Vector2.up * translate); // 연결된 방을 y축으로 이동시킨다
                this.transform.Translate(Vector2.up * translate); // 문을 y축으로 이동시킨다
            }
            else
            {
                u.transform.Translate(Vector2.right * translate); // 연결된 방을 x축으로 이동시킨다
                this.transform.Translate(Vector2.right * translate); // 문을 x축으로 이동시킨다
            }
        }
    }
}
