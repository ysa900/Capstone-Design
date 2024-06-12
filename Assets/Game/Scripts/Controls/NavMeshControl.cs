using UnityEngine;
using UnityEngine.SceneManagement;

public class NavMeshControl : MonoBehaviour
{

    
    public NavMeshPlus.Components.NavMeshSurface surface2d; // bake ���ֱ� ���� nav mesh ǥ��
    public GameObject surface; // ���� ������Ʈ ǥ��(�ش� ������Ʈ�� Nav Mesh ������ ��ġ)
    public float DirectionX = 0;
    public float DirectionY = 0;

    
    private string StageName;

    private void Start()
    {
        StageName = SceneManager.GetActiveScene().name;

    }

    private void FixedUpdate()
    {

       
        ChangeSurface();

    }

    //GameObject ǥ���� ��ġ�� �̵���Ű�� NavMesh�� ������ ���� �Լ�
    public void ChangeSurface()
    {

        switch (StageName)
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
                    surface.transform.Translate(Vector3.right * 85); // ������ ���� * �Ÿ�
                 
                    DirectionX = 0;
                }
                break;
            case "Stage3":

                break;



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
        surface2d.RemoveData();  ;
      
    }
}
