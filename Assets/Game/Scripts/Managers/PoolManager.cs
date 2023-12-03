using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    // 프리팹 보관할 변수
    public GameObject[] prefabs = new GameObject[1];

    // 풀 담당을 하는 리스트들
    List<GameObject>[] pools;

    private void Awake()
    {
        pools = new List<GameObject>[prefabs.Length];
        for(int index = 0; index < pools.Length; index++)
        {
            pools[index] = new List<GameObject>();
        }
    }

    public GameObject Get(int index)
    {
        GameObject select = null;

        // 선택한 풀의 놀고있는(비활성화된) 게임 오브젝트 접근    
        foreach(GameObject item in pools[index]) { 

            if(!item.activeSelf)
            {
                // 발견하면 select 변수에 할당
                select = item;
                select.SetActive(true);
                break;
            }
        }

        // 못 찾았으면?      
        if(!select)
        {
            // 새롭게 생성하고 select 변수에 할당
            // 자기 자신(transform) 추가 이유: hierarchy창 지저분해지는 거 방지
            select = Instantiate(prefabs[index]);
            pools[index].Add(select);
        }

        return select;
    }

    /*
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(Instance.transform);
        pools.AddRange(obj);
    }*/ 
}
