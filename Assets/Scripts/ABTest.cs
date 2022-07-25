using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABTest : MonoBehaviour
{

    void Start()
    {
        //GameObject obj =  ABMgr.GetInstance().LoadRes<GameObject>("model", "Cube");
        //obj.transform.localScale = Vector3.one * 1;
        //obj = ABMgr.GetInstance().LoadRes<GameObject>("model", "Cube");
        //obj.transform.position = Vector3.right * 2;

        //ABMgr.GetInstance().LoadResAsyne("model", "Cube", (obj) => 
        //{
        //    (obj as GameObject).transform.localScale = Vector3.one * 2;
        //});

        ABMgr.GetInstance().LoadResAsyne<GameObject>("model", "Cube", (obj) =>
        {
            obj.transform.localScale = Vector3.one * 2;
        });
    }
}
