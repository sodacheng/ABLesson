using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ABTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 第一步 加载AB包  
        AssetBundle ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + "model");
        //AB包不能 [重复] 加载, 否则报错
        //AssetBundle ab2 = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + "model");




        // 第二步 加载AB包中的资源
        // 只是用名字加载 会出现 同名不同类型资源 分不清
        // 建议用 泛型加载 或是 Type指定
        //GameObject obj = ab.LoadAsset<GameObject>("Cube");
        GameObject obj = ab.LoadAsset("Cube",typeof(GameObject)) as GameObject;

        Instantiate(obj);


        //AssetBundle.UnloadAllAssetBundles(false); // 卸载所有加载的AB包 传入true, 会把场景中通过AB包加载的资源一起卸载 false只卸载AB包
        ab.Unload(false); // 卸载单个AB包

        // 异步加载 - 协程
        StartCoroutine(LoadABRes("model","Sphere"));
    }

    IEnumerator LoadABRes(string ABName, string resName)
    {
        // 第一步 加载AB包
        AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/" + ABName);
        yield return abcr;
        // 第二步 加载资源
        AssetBundleRequest abr = abcr.assetBundle.LoadAssetAsync(resName, typeof(GameObject));
        yield return abr;
        Instantiate(abr.asset as GameObject);
    }

    void Update()
    {

    }
}
