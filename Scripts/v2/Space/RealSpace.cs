using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealSpace : Bound2D
{
    public override void Initializing()
    {
        base.Initializing();
        // Debug.Log($"Initialzing {this.gameObject.name}");

        this.gameObject.layer = LayerMask.NameToLayer("Real Space");
        this.gameObject.tag = "Real Space";

    }

}
