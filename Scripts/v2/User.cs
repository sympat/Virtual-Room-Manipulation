using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class User : MonoBehaviour {

    private static int totalID = 0;
    private int id;
    private UnityEvent onCollect = new UnityEvent();

    public Vector2 Position 
    {
        get
        {
            return Utility.CastVector3Dto2D(this.gameObject.transform.position);
        }

        set
        {
            this.gameObject.transform.position = Utility.CastVector2Dto3D(value);
        }
    }
    public float fov
    {
        get { return GetComponentInChildren<Camera>().fieldOfView; }
    }

    public Vector2 Forward {
        get {
            return Utility.CastVector3Dto2D(this.transform.forward);
        }
    }

    private void Awake() {
        onCollect.AddListener(GameObject.FindWithTag("Task").GetComponent<CoinTask>().Destroy);
        onCollect.AddListener(GameObject.FindWithTag("Task").GetComponent<CoinTask>().Generate);
    }

    private void OnTriggerStay(Collider other) {
        Room room = other.GetComponent<Room>();
        if(room != null && !room.IsContain(this.Position)) {
            Debug.Log("User is out of room");
        }
    }

    private void OnTriggerEnter(Collider other) {
        Coin coin = other.GetComponent<Coin>();
        if(coin != null) {
            Debug.Log("User collected a coin");
            onCollect.Invoke();
        }
    }

    public bool IsTargetInUserFov(Vector2 target) // global 좌표계 기준으로 비교
    {
        Vector2 userToTarget = target - this.Position;
        Vector2 userForward = this.Forward;

        float unsignedAngle = Vector2.Angle(userToTarget, userForward);

        if (unsignedAngle - ((this.fov + 10) / 2) < 0.01f)
            return true;
        else
            return false;
    }
}

// public class User : Base2D{

//     private static int totalID = 0;
//     private int id;

//     private UnityEvent onCollect = new UnityEvent();

//     public float fov
//     {
//         get { return GetComponentInChildren<Camera>().fieldOfView; }
//     }

//     public Vector2 Forward {
//         get {
//             return Utility.CastVector3Dto2D(this.transform.forward);
//         }
//     }

//     public override void Initializing(BaseIntializer init)
//     {
//         base.Initializing(init);

//         this.id = totalID++;
//         this.gameObject.name = "User_" + this.id;

//         Rigidbody rigidbody = GetComponent<Rigidbody>();
//         if(rigidbody == null) rigidbody = this.gameObject.AddComponent<Rigidbody>();

//         rigidbody.isKinematic = true;
//         box.isTrigger = true;

//         onCollect.AddListener(GameObject.FindWithTag("Task").GetComponent<CoinTask>().Generate);
//     }

//     public bool IsTargetInUserFov(Vector2 target) // global 좌표계 기준으로 비교
//     {
//         Vector2 userToTarget = target - this.Position;
//         Vector2 userForward = this.Forward;

//         float unsignedAngle = Vector2.Angle(userToTarget, userForward);

//         if (unsignedAngle - ((this.fov) / 2) < 0.01f)
//             return true;
//         else
//             return false;
//     }

//     private void OnTriggerStay(Collider other) {
//         Room room = other.GetComponent<Room>();
//         if(room != null && !this.IsInside(room)) {
//             Debug.Log("User is out of room");
//         }
//     }

//     private void OnTriggerEnter(Collider other) {
//         Coin coin = other.GetComponent<Coin>();
//         if(coin != null) {
//             Debug.Log("User collected a coin");
//             onCollect.Invoke();
//             // Generate();
//         }
//     }
// }