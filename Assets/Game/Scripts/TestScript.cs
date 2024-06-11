using System.Collections;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    float a = 0;
    float b = 0;

    int num1 = 0;
    int num2 = 0;

    bool isOK = false;
    float r1 = 0;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        Invoke("EE", 1f);
        //StartCoroutine(AA());
    }

    // Update is called once per frame
    void Update()
    {
        a += Time.deltaTime;
        //Debug.Log("Update: " + "   " + Time.timeScale + "      " + Time.deltaTime + "        " + a);

        num1++;
        //Debug.Log("Update, num1: " + "   " + num1);

        if (isOK)
        {
            r1 += Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        b += Time.fixedDeltaTime;
        //Debug.Log("FixedUpdate: " + "   " + Time.timeScale + "      " + Time.fixedDeltaTime + "        " + b);

        num2++;
        //Debug.Log("FixedUpdate, num2: " + "   " + num2);
    }

    void EE()
    {
        StartCoroutine(AA());
    }

    IEnumerator AA()
    {
        Debug.Log("1초 기록 시작");

        int beforNum1 = num1;
        int beforNum2 = num2;
        //Debug.Log("Update: " + "   " + Time.timeScale + "      " + Time.deltaTime + "        " + a);
        //Debug.Log("Update, num1: " + "   " + num1);
        //Debug.Log("FixedUpdate: " + "   " + Time.timeScale + "      " + Time.fixedDeltaTime + "        " + b);
        //Debug.Log("FixedUpdate, num2: " + "   " + num2);
        isOK = true;
        yield return new WaitForSecondsRealtime(1f);
        isOK = false;
        Debug.Log("1초 기록 끝");

        int afterNum1 = num1;
        int afterNum2 = num2;
        //Debug.Log("Update: " + "   " + Time.timeScale + "      " + Time.deltaTime + "        " + a);
        //Debug.Log("Update, num1: " + "   " + num1);
        //Debug.Log("FixedUpdate: " + "   " + Time.timeScale + "      " + Time.fixedDeltaTime + "        " + b);
        //Debug.Log("FixedUpdate, num2: " + "   " + num2);

        int dif1 = afterNum1 - beforNum1 + 1;
        int dif2 = afterNum2 - beforNum2 + 1;

        Debug.Log("Update문 호출 횟수 = " + beforNum1 + "   " + afterNum1);
        Debug.Log("FixedUpdate문 호출 횟수 = " + beforNum2 + "   " + afterNum2);

        Debug.Log("평균 deltaTime = " + r1 / dif1);
        float avDeltaTime = r1 / dif1;
        Debug.Log("1 / deltaTime = " + 1 / avDeltaTime);

        Debug.Log("Update문 호출 횟수, 1 / deltaTime * timeScale = " + (afterNum1 - beforNum1)  +", "+ 1 / avDeltaTime * Time.timeScale);
        Debug.Log("FixedUpdate문 호출 횟수, 1 / fixedDeltaTime * timeScale = " + (afterNum2 - beforNum2) + ", " + 1 / Time.fixedDeltaTime * Time.timeScale);

    }
}
