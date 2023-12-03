using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    // ������ ������ ����
    public GameObject[] prefabs = new GameObject[1];

    // Ǯ ����� �ϴ� ����Ʈ��
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

        // ������ Ǯ�� ����ִ�(��Ȱ��ȭ��) ���� ������Ʈ ����    
        foreach(GameObject item in pools[index]) { 

            if(!item.activeSelf)
            {
                // �߰��ϸ� select ������ �Ҵ�
                select = item;
                select.SetActive(true);
                break;
            }
        }

        // �� ã������?      
        if(!select)
        {
            // ���Ӱ� �����ϰ� select ������ �Ҵ�
            // �ڱ� �ڽ�(transform) �߰� ����: hierarchyâ ������������ �� ����
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
