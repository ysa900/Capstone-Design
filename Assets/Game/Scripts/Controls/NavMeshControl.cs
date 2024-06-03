using NavMeshPlus.Components;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.AI.Navigation;
using Unity.MLAgents;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NavMeshControl : MonoBehaviour
{

    
    public NavMeshPlus.Components.NavMeshSurface surface2d; // bake 해주기 위한 nav mesh 표면
    public GameObject surface; // 게임 오브젝트 표면(해당 오브젝트에 Nav Mesh 영역을 설치)
    public float DirectionX = 0;
    public float DirectionY = 0;

    
    private string Stage;

    private void Start()
    {
        Stage = SceneManager.GetActiveScene().name;

    }

    private void FixedUpdate()
    {

       
        ChangeSurface();

    }

    //GameObject 표면의 위치를 이동시키고 NavMesh를 삭제후 굽는 함수
    public void ChangeSurface()
    {

        switch (Stage)
        {
            case "Stage1":
                if (DirectionX == 0 && DirectionY == 0) { return; }
                else if (DirectionX != 0 && DirectionY != 0)
                {
                    surface.transform.Translate(Vector3.right * DirectionX * 40);
                    surface.transform.Translate(Vector3.up * DirectionY * 40);
                    DirectionX = 0;
                    DirectionY = 0;
                 
                }
                else if (DirectionX != 0 && DirectionY == 0)
                {
                    surface.transform.Translate(Vector3.right * DirectionX * 40);
                    DirectionX = 0;
                    DirectionY = 0;
                
                }
                else if (DirectionX == 0 && DirectionY != 0)
                {
                    surface.transform.Translate(Vector3.up * DirectionY * 40);
                    DirectionX = 0;
                    DirectionY = 0;
                  

                }
                break;

            case "Stage2":
                if (DirectionX != 0)
                {
                    surface.transform.Translate(Vector3.right * 85); // 오른쪽 방향 * 거리
                 
                    DirectionX = 0;
                }
                break;
            default:
                break;
        }

       
            
    }

    // NavMesh 영역 굽는 함수
    public void BakeNavMeshArea()
    {
        surface2d.BuildNavMesh();
       
    }

    // NavMesh 영역 삭제 함수
    public void RemoveNavMeshArea()
    {
        surface2d.RemoveData();  ;
      
    }
}
