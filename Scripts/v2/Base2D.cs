using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base2D : MonoBehaviour
{
    protected BoxCollider box;

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

    public float Height
    {
        get
        {
            return box.size.y;
        }

        set
        {
            box.size = new Vector3(box.size.x, value, box.size.z);
            box.center = new Vector3(box.center.x, value / 2, box.center.z);
        }
    }

    public Vector2 Size
    {
        get
        {
            return Utility.CastVector3Dto2D(box.size);
        }

        set
        {
            box.size = Utility.CastVector2Dto3D(value, box.size.y);
        }
    }

    public Vector2 Max
    {
        get
        {

            return Utility.CastVector3Dto2D(box.size / 2) + this.Position;
        }
    }

    public Vector2 Min
    {
        get
        {
            return Utility.CastVector3Dto2D(-box.size / 2) + this.Position;

        }
    }

    public Vector2 localMax {
        get {
            return Utility.CastVector3Dto2D(box.center + box.size / 2);
        }
    }

    public Vector2 localMin {
        get
        {
            return Utility.CastVector3Dto2D(box.center - box.size / 2);

        }
    }

    public Vector2 Extents
    {
        get
        {
            return Utility.CastVector3Dto2D(box.size / 2);
        }
    }

    public virtual void Initializing(BaseIntializer init)
    {
        box = this.gameObject.GetComponent<BoxCollider>();
        if (box == null) box = this.gameObject.AddComponent<BoxCollider>();

        this.Size = init.size;
        this.Height = init.height;

        if(init.useMesh) {
            MeshFilter meshFilter = this.gameObject.GetComponentInChildren<MeshFilter>();
            if (meshFilter == null)
            {
                meshFilter = this.gameObject.AddComponent<MeshFilter>();
                meshFilter.mesh = GenerateMesh();
                this.gameObject.AddComponent<MeshRenderer>(); // MeshRenderer 컴포넌트 부착
                this.gameObject.GetComponent<MeshRenderer>().material = init.material; // 공간 object에 대한 material 초기화
            }   
            else {
                box.size = meshFilter.mesh.bounds.size;
                box.center = meshFilter.mesh.bounds.center + meshFilter.gameObject.transform.position;
            }
        }

    }

    public virtual void HideObject() {

    }

    public virtual void ShowObject() {
        
    }

    public Vector2 GetEdge2D(int index, Space relativeTo = Space.Self)
    {
        int realIndex = Utility.mod(index, 4);
        Vector2 result;

        switch (realIndex)
        {
            case 0:
                result = (relativeTo == Space.World) ? new Vector2(0, this.Extents.y) + this.Position : new Vector2(0, this.Extents.y);
                break;
            case 1:
                result = (relativeTo == Space.World) ? new Vector2(-this.Extents.x, 0) + this.Position : new Vector2(-this.Extents.x, 0);
                break;
            case 2:
                result = (relativeTo == Space.World) ? new Vector2(0, -this.Extents.y) + this.Position : new Vector2(0, -this.Extents.y);
                break;
            case 3:
                result = (relativeTo == Space.World) ? new Vector2(this.Extents.x, 0) + this.Position : new Vector2(this.Extents.x, 0);
                break;
            default:
                throw new System.NotImplementedException();
        }

        return result;
    }

    public Vector2 GetVertex2D(int index, Space relativeTo = Space.Self) {
        int realIndex = Utility.mod(index, 4);
        Vector2 result;

        switch (realIndex)
        {
            case 0:
                result = (relativeTo == Space.World) ? this.Max : this.localMax;
                break;
            case 1:
                result = (relativeTo == Space.World) ? new Vector2(-this.Extents.x, this.Extents.y) + this.Position : new Vector2(-this.Extents.x, this.Extents.y);
                break;
            case 2:
                result = (relativeTo == Space.World) ? this.Min : this.localMin;
                break;
            case 3:
                result = (relativeTo == Space.World) ? new Vector2(this.Extents.x, -this.Extents.y) + this.Position : new Vector2(this.Extents.x, -this.Extents.y);
                break;
            default:
                throw new System.NotImplementedException();
        }

        return result;
    }

    public Vector3 GetVertex3D(int index, float height = 0, Space relativeTo = Space.Self)
    {
        Vector2 result = GetVertex2D(index, relativeTo);
        return Utility.CastVector2Dto3D(result, height);
    }

    public Vector2 SamplingInnerPosition(float bound = 0)
    {
        float xSampling = Random.Range(this.Min.x + bound, this.Max.x - bound);
        float ySampling = Random.Range(this.Min.y + bound, this.Max.y - bound);

        return new Vector2(xSampling, ySampling);
    }

    public void MoveEdge(int index, float translate) // box 형태를 유지하기 위해 wall의 1차원 움직임만 허용 (translate 부호 기준은 2차원 좌표계)
    {
        int realIndex = Utility.mod(index, 4);
        float newCenterX = this.Position.x,
            newCenterY = this.Position.y,
            newSizeX = this.Size.x,
            newSizeY = this.Size.y;

        if (realIndex == 0) // N (+y)
        {
            newCenterY = this.Position.y + translate / 2;
            newSizeY = this.Size.y + translate;
        }
        else if (realIndex == 1) // W (-x)
        {
            newCenterX = this.Position.x + translate / 2;
            newSizeX = this.Size.x - translate;
        }
        else if (realIndex == 2) // S (-y)
        {
            newCenterY = this.Position.y + translate / 2;
            newSizeY = this.Size.y - translate;
        }
        else if (realIndex == 3) // E (+x)
        {
            newCenterX = this.Position.x + translate / 2;
            newSizeX = this.Size.x + translate;
        }
        else
        {
            throw new System.NotImplementedException();
        }

        this.Position = new Vector2(newCenterX, newCenterY);
        this.Size = new Vector2(newSizeX, newSizeY);

        GetComponent<MeshFilter>().mesh = GenerateMesh();
    }

    public bool IsInsideInXAxis(Base2D other) // x축 기준으로 this가 other 안에 들어오는지 판단하는 함수
    {
        // if ((this.Min.x <= other.Min.x) && (this.Max.x >= other.Max.x))
        // {
        //     return true;
        // }
        // else
        // {
        //     return false;
        // }

        if ((other.Min.x <= this.Min.x) && (other.Max.x >= this.Max.x))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsInsideInYAxis(Base2D other) // y축 기준
    {
        if ((other.Min.y <= this.Min.y) && (other.Max.y >= this.Max.y))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsInside(Base2D other) // this가 other 안에 들어오는지 판단하는 함수
    {
        if (IsInsideInXAxis(other) && IsInsideInYAxis(other))
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public bool IsContain(Vector2 point) // this가 point를 포함하는지 판단하는 함수
    {
        if (this.box.bounds.Contains(Utility.CastVector2Dto3D(point, this.Height / 2)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsIntersectInXAxis(Base2D other) // x축 기준으로 this 와 otherbox가 교차하는지 판단하는 함수
    {
        if((this.Min.x <= other.Max.x) && (this.Max.x >= other.Min.x))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool IsIntersectInYAxis(Base2D other) // y축 기준
    {
        if ((this.Min.y <= other.Max.y) && (this.Max.y >= other.Min.y))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsIntersect(Base2D other) // this와 point가 교차하는지 판단하는 함수
    {
        if (IsIntersectInXAxis(other) && IsIntersectInYAxis(other))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Mesh GenerateMesh(bool useOutNormal = false)
    {
        // 아래의 코드는 육면체(cube) 형태에서만 동작
        int n = 4;

        Vector3[] vertices = null;
        int[] triangles = null;
        Vector3[] normals = null;
        Vector2[] uv = null;

        vertices = new Vector3[6 * n]; // vertex 지정
        vertices[0] = vertices[7] = vertices[19] = GetVertex3D(0); // 음... 하드 코딩
        vertices[1] = vertices[6] = vertices[11] = GetVertex3D(1);
        vertices[2] = vertices[10] = vertices[14] = GetVertex3D(2);
        vertices[3] = vertices[15] = vertices[18] = GetVertex3D(3);
        vertices[4] = vertices[16] = vertices[20] = GetVertex3D(0, this.Height);
        vertices[5] = vertices[8] = vertices[21] = GetVertex3D(1, this.Height);
        vertices[9] = vertices[13] = vertices[22] = GetVertex3D(2, this.Height);
        vertices[12] = vertices[17] = vertices[23] = GetVertex3D(3, this.Height);

        triangles = new int[] // index 지정
        {
            2,1,0,0,3,2,  // bottom 
            6,5,4,4,7,6, // north
            10,9,8,8,11,10, // west
            12,13,14,14,15,12, // south
            16,17,18,18,19,16, // east
            20,21,22,22,23,20 // top
        };

        normals = new Vector3[6 * n]; // normal 지정
        if(useOutNormal)
        {
            normals[0] = normals[1] = normals[2] = normals[3] = new Vector3(0, -1, 0); // 음... 하드 코딩
            normals[4] = normals[5] = normals[6] = normals[7] = new Vector3(0, 0, 1);
            normals[8] = normals[9] = normals[10] = normals[11] = new Vector3(-1, 0, 0);
            normals[12] = normals[13] = normals[14] = normals[15] = new Vector3(0, 0, -1);
            normals[16] = normals[17] = normals[18] = normals[19] = new Vector3(1, 0, 0);
            normals[20] = normals[21] = normals[22] = normals[23] = new Vector3(0, 1, 0);
        }
        else
        {
            normals[0] = normals[1] = normals[2] = normals[3] = new Vector3(0, 1, 0); // 음... 하드 코딩
            normals[4] = normals[5] = normals[6] = normals[7] = new Vector3(0, 0, -1);
            normals[8] = normals[9] = normals[10] = normals[11] = new Vector3(1, 0, 0);
            normals[12] = normals[13] = normals[14] = normals[15] = new Vector3(0, 0, 1);
            normals[16] = normals[17] = normals[18] = normals[19] = new Vector3(-1, 0, 0);
            normals[20] = normals[21] = normals[22] = normals[23] = new Vector3(0, -1, 0);
        }

        uv = new Vector2[6 * n]; // UV 좌표 지정

        // bottom (floor)
        uv[0] = new Vector2(0.5f, 0.5f);
        uv[1] = new Vector2(0.0f, 0.5f);
        uv[2] = new Vector2(0.0f, 0.0f);
        uv[3] = new Vector2(0.5f, 0.0f);
        // north (wall)
        uv[4] = new Vector2(1.0f, 1.0f);
        uv[5] = new Vector2(0.5f, 1.0f);
        uv[6] = new Vector2(0.5f, 0.5f);
        uv[7] = new Vector2(1.0f, 1.0f);
        // west (wall)
        uv[8] = new Vector2(1.0f, 1.0f);
        uv[9] = new Vector2(0.5f, 1.0f);
        uv[10] = new Vector2(0.5f, 0.5f);
        uv[11] = new Vector2(1.0f, 1.0f);
        // south (wall)
        uv[12] = new Vector2(1.0f, 1.0f);
        uv[13] = new Vector2(0.5f, 1.0f);
        uv[14] = new Vector2(0.5f, 0.5f);
        uv[15] = new Vector2(1.0f, 1.0f);
        // east (wall)
        uv[16] = new Vector2(1.0f, 1.0f);
        uv[17] = new Vector2(0.5f, 1.0f);
        uv[18] = new Vector2(0.5f, 0.5f);
        uv[19] = new Vector2(1.0f, 1.0f);
        // top (floor)        
        uv[20] = new Vector2(0.5f, 1.0f);
        uv[21] = new Vector2(0.0f, 1.0f);
        uv[22] = new Vector2(0.0f, 0.5f);
        uv[23] = new Vector2(0.5f, 0.5f);

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;

        return mesh;
    }
}
