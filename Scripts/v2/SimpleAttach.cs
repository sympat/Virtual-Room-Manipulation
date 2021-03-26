using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class SimpleAttach : MonoBehaviour
{
    private Interactable interactable; 
    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<Interactable>();
    }

    private void OnHandHoverBegin(Hand hand) {
        hand.ShowGrabHint();
    }

    private void OnHandHoverEnd(Hand hand) {
        hand.HideGrabHint();
    }

    private Vector3 oldPosition;
    private Quaternion oldRotation;
    private Hand.AttachmentFlags attachmentFlags = Hand.defaultAttachmentFlags & ( ~Hand.AttachmentFlags.SnapOnAttach ) & (~Hand.AttachmentFlags.DetachOthers) & (~Hand.AttachmentFlags.VelocityMovement);

    private void HandHoverUpdate(Hand hand) {
        // GrabTypes grabType = hand.GetGrabStarting();
        // bool isGrabEnding = hand.IsGrabEnding(gameObject);

        // Debug.Log(hand);

        // if(interactable.attachedToHand == null && grabType != GrabTypes.None) {
        //     hand.AttachObject(gameObject, grabType);
        //     // hand.HoverLock(interactable);
        //     hand.HideGrabHint();
        // } 
        // else {
        //     hand.DetachObject(gameObject);
        //     // hand.HoverUnlock(interactable);
        // }

        GrabTypes startingGrabType = hand.GetGrabStarting();
        bool isGrabEnding = hand.IsGrabEnding(this.gameObject);

        if (interactable.attachedToHand == null && startingGrabType != GrabTypes.None)
        {
            // Save our position/rotation so that we can restore it when we detach
            // oldPosition = transform.position;
            // oldRotation = transform.rotation;

            // Call this to continue receiving HandHoverUpdate messages,
            // and prevent the hand from hovering over anything else
            hand.HoverLock(interactable);

            // Attach this object to the hand
            hand.AttachObject(gameObject, startingGrabType, attachmentFlags);
        }
        else if (isGrabEnding)
        {
            // Detach this object from the hand
            hand.DetachObject(gameObject);

            // Call this to undo HoverLock
            hand.HoverUnlock(interactable);

            // Restore position/rotation
            // transform.position = oldPosition;
            // transform.rotation = oldRotation;
        }
    }
}
