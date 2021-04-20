using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public enum GainType { Translation = 0, Rotation = 1, Curvature = 2, Undefined = -1 };

[Serializable]
public class GainRedirector : MonoBehaviour
{
    [HideInInspector]
    public const float MIN_ROTATION_GAIN = -0.33f;
    [HideInInspector]
    public const float MAX_ROTATION_GAIN = 0.24f;
    [HideInInspector]
    public const float MIN_CURVATURE_GAIN = -0.045f; // turn radius : 22m
    [HideInInspector]
    public const float MAX_CURVATURE_GAIN = 0.045f;
    [HideInInspector]
    public const float HODGSON_MIN_CURVATURE_GAIN = -0.133f; // turn radius : 7.5m
    [HideInInspector]
    public const float HODGSON_MAX_CURVATURE_GAIN = 0.133f;
    [HideInInspector]
    public const float MIN_TRANSLATION_GAIN = -0.14f;
    [HideInInspector]
    public const float MAX_TRANSLATION_GAIN = 0.26f;

    protected float translationGain;
    protected float rotationGain;
    protected float curvatureGain;

    protected UserBody user;
    protected VirtualEnvironment virtualEnvironment;

    public virtual (GainType, float) ApplyRedirection() {
        float degree = 0;
        GainType type = GainType.Undefined;

        if (user.DeltaPosition.magnitude > 0.01f && user.DeltaPosition.magnitude >= Mathf.Abs(user.DeltaRotation))
        {
            degree = user.DeltaPosition.magnitude * (MAX_TRANSLATION_GAIN);
            type = GainType.Translation;
        }
        else if (Mathf.Abs(user.DeltaRotation) > 0.1f && user.DeltaPosition.magnitude < Mathf.Abs(user.DeltaRotation))
        {
            degree = user.DeltaRotation * (MIN_ROTATION_GAIN);
            type = GainType.Rotation;
        }
        else
        {
            type = GainType.Undefined;
        }

        return (type, degree);
    }

    private void Start() {
        virtualEnvironment = GetComponent<VirtualEnvironment>();
    }

    private void FixedUpdate() {
        user = virtualEnvironment.userBody;
        var result = ApplyRedirection();

        GainType gainType = result.Item1;
        float degree = result.Item2;

        // Debug.Log("gainType: " + gainType);
        // Debug.Log("degree " + degree);
        // Debug.Log("user.DeltaPosition " + user.DeltaPosition);


        switch (gainType)
        {
            case GainType.Translation:
                virtualEnvironment.Translate(-user.Forward * degree * Time.fixedDeltaTime, Space.World);
                break;
            case GainType.Rotation:
                virtualEnvironment.RotateAround(user.Position, degree * Time.fixedDeltaTime);
                break;
            case GainType.Curvature:
                // this.Translate(-user.Forward * user.DeltaPosition.magnitude * Time.fixedDeltaTime, Space.World);
                // this.Rotate(degree * Time.fixedDeltaTime);
                break;
            default:
                break;
        }
    }
}
