using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UserControl : MonoBehaviour
{
    // Reference: https://github.com/kinifi/Unity-FPS-Controller
    public float speed = 0.25f;
    public float mouseSensitivity = 1.5f;
    private float m_MovX;
    private float m_MovY;
    private Vector3 m_moveHorizontal;
    private Vector3 m_movVertical;
    private Vector3 m_velocity;
    private float m_yRot;
    private float m_xRot;
    private Vector3 m_rotation;
    private Vector3 m_cameraRotation;
    // private float m_lookSensitivity = 1.5f;
    private bool m_cursorIsLocked = true;
    private Camera m_Camera;
    private User user;
    private VirtualEnvironment virtualEnvironment;

    public class DoorInteractionEvent : UnityEvent<Door> {}
    private DoorInteractionEvent onInteraction = new DoorInteractionEvent();
    private UnityEvent onEnter = new UnityEvent();

    private void Awake() {
        user = GetComponent<User>();
        virtualEnvironment = GameObject.FindWithTag("VE").GetComponent<VirtualEnvironment>();
        // onInteraction.AddListener(virtualEnvironment.ChangeCurrentRoom);
        onEnter.AddListener(GameObject.FindWithTag("Manipulation").GetComponent<Manipulation>().SwitchEnable);
    }

    public void CheckDoorInteraction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction, Color.red);
            int layerMask = 1 << LayerMask.NameToLayer("Door");

            if (Physics.Raycast(ray, out hit, 0.3f, layerMask))
            {
                Door door = hit.transform.GetComponent<Door>();

                // Debug.Log(user.Size);

                user.Position = door.Position + user.Forward * 0.2f; // user.Size.x

                onInteraction.Invoke(door);
                onEnter.Invoke();
                // this.transform.position = d.Position + Utility.CastVector3Dto2D(this.instance.transform.forward) * size.x;
                // Room u = d.GetConnectedRoom(currentRoom);
                
                //instance.transform.position = Utility.CastVector2Dto3D(d.center) + this.instance.transform.forward * size.x;

                // this.center = d.center + Utility.CastVector3Dto2D(this.instance.transform.forward) * size.x;

                // return u;
            }
        }
    }

    private void Update()
    {
        CheckDoorInteraction();

        if(m_Camera == null)
        {
            m_Camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }

        m_MovX = Input.GetAxis("Horizontal");
        m_MovY = Input.GetAxis("Vertical");

        m_moveHorizontal = this.transform.right * m_MovX;
        m_movVertical = this.transform.forward * m_MovY;

        m_velocity = (m_moveHorizontal + m_movVertical).normalized * speed;

        //mouse movement 
        m_yRot = Input.GetAxisRaw("Mouse X");
        m_rotation = new Vector3(0, m_yRot, 0) * mouseSensitivity;

        m_xRot = Input.GetAxisRaw("Mouse Y");
        m_cameraRotation = new Vector3(m_xRot, 0, 0) * mouseSensitivity;

        //apply camera rotation

        //move the actual player here
        if (m_velocity != Vector3.zero)
        {
            this.transform.position += m_velocity * Time.fixedDeltaTime;

            // Vector2 temp = this.center;
            // this.center += Utility.CastVector3Dto2D(m_velocity) * Time.fixedDeltaTime;

            // if (!currentRoom.IsInside(this)) // currentRoom 밖으로 벗어나지 않도록 조정
            //     this.center = temp;
        }

        if (m_rotation != Vector3.zero)
        {
            //rotate the camera of the player
            this.transform.rotation *= Quaternion.Euler(m_rotation);
        }

        if (m_Camera != null)
        {
            //negate this value so it rotates like a FPS not like a plane

            m_Camera.transform.Rotate(-m_cameraRotation);
        }

        InternalLockUpdate();
    }

    //controls the locking and unlocking of the mouse
    private void InternalLockUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            m_cursorIsLocked = false;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            m_cursorIsLocked = true;
        }

        if (m_cursorIsLocked)
        {
            UnlockCursor();
        }
        else if (!m_cursorIsLocked)
        {
            LockCursor();
        }
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
