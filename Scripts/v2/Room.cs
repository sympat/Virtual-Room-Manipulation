using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : Base2D
{
    private static int totalID = 0;
    private int id;
    private Vector2 originSize;

    public Vector2 OriginSize {
        get {
            return originSize;
        }
    }

    public int ID {
        get {
            return id;
        }
    }

    public override void Initializing(BaseIntializer init)
    {
       base.Initializing(init);

       this.id = totalID++;
       this.gameObject.name = "Room_" + this.id;
       this.originSize = this.Size;
    }

    
    public override void HideObject() {
        GetComponent<MeshRenderer>().enabled = false;
    }

    public override void ShowObject()
    {
        GetComponent<MeshRenderer>().enabled = true;
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        Room objAsRoom = obj as Room;
        if (objAsRoom == null) return false;
        else return Equals(objAsRoom);
    }

    public override int GetHashCode()
    {
        return this.id; // TODO: 이런식으로 Hash 값을 지정해도 되나?
    }

    public bool Equals(Room v)
    {
        if (v == null) return false;
        else return (this.id == v.id);
    }

    public override string ToString()
    {
        string result = "";
        result += string.Format("ID: {0}, Size: {1}, Origin: {2}", id, this.Size, this.Position);
        result += string.Format(", ObjName: {0}", this.gameObject.name);

        return result;
    }
}
