using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseIntializer
{
    public Vector2 size;
    public float height;
    public Material material;
    public bool useMesh;

    public BaseIntializer() {
        this.size = Vector2.one;
        this.height = 1;
        this.material = null;
        this.useMesh = false;
    }

    public BaseIntializer(Vector2 size, float height, Material material, bool useMesh) {
        this.size = size;
        this.height = height;
        this.material = material;
        this.useMesh = useMesh;
    }
}
