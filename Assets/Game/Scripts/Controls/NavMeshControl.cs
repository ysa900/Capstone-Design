using NavMeshPlus.Components;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NavMeshControl : MonoBehaviour
{

    
    public NavMeshPlus.Components.NavMeshSurface surface2d; // bake ���ֱ� ���� nav mesh ǥ��
    public GameObject surface; // ���� ������Ʈ ǥ��(�ش� ������Ʈ�� Nav Mesh ������ ��ġ)
    public float DirectionX = 0;
    public float DirectionY = 0;

    private void FixedUpdate()
    {
        ChangeSurface();

    }

    //GameObject ǥ���� ��ġ�� �̵���Ű�� NavMesh�� ������ ���� �Լ�
    public void ChangeSurface()
    {
        if (DirectionX == 0 && DirectionY == 0) { return; }
        else if (DirectionX != 0 && DirectionY != 0)
        {
            surface.transform.Translate(Vector3.right * DirectionX * 40);
            surface.transform.Translate(Vector3.up * DirectionY * 40);
            DirectionX = 0;
            DirectionY = 0;  
            RemoveNavMeshArea();
            BakeNavMeshArea();
        }
        else if (DirectionX != 0 && DirectionY == 0)
        {
            surface.transform.Translate(Vector3.right * DirectionX * 40);
            DirectionX = 0;
            DirectionY = 0;      
            RemoveNavMeshArea();
            BakeNavMeshArea();
        }
        else if (DirectionX == 0 && DirectionY != 0)
        {
            surface.transform.Translate(Vector3.up * DirectionY * 40);
            DirectionX = 0;
            DirectionY = 0;
            RemoveNavMeshArea();
            BakeNavMeshArea();

        }
            
    }

    // NavMesh ���� ���� �Լ�
    public void BakeNavMeshArea()
    {
        surface2d.BuildNavMesh();
    }

    // NavMesh ���� ���� �Լ�
    public void RemoveNavMeshArea()
    {
        surface2d.RemoveData();

    }
}
