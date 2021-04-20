using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackTransform2D : MonoBehaviour
{
    public GameObject trackingObject;
    private Vector3 relativePosition;
    private Quaternion relativeRotation;

    private void Awake() {
        relativePosition = Utility.ProjectVector3Dto2D(this.transform.position - trackingObject.transform.position);
        relativeRotation = Utility.ProjectRotation3Dto2D(Quaternion.Inverse(this.transform.rotation) * trackingObject.transform.rotation);
    }

    private void Update()
    {
        this.transform.position = Utility.ProjectVector3Dto2D(trackingObject.transform.position + relativePosition);
        this.transform.rotation = Utility.ProjectRotation3Dto2D(trackingObject.transform.rotation * relativeRotation);
    }
}
