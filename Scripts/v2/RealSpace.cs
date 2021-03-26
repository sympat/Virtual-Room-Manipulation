using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealSpace : MonoBehaviour
{
    public Vector2 size;

    private void Awake() {
        BaseIntializer init = new BaseIntializer(size, 3, null, false);
        GetComponent<Base2D>().Initializing(init);
    }
}
