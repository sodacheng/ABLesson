using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ABMgr : SingletonAutoMono<ABMgr> // 继承Mono的单例
{
    // AB包管理器 目的是
    // 让外部更方便的进行资源加载 -> AB包不能够重复加载 重复加载会报错
    // 主包
    private AssetBundle mainAB = null;
    // 依赖包获取用的配置文件
    private AssetBundleManifest manifest = null;
    // 用字典来存储 加载过的AB包
    private Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();

    /// <summary>
    /// AB包存放路径, 方便修改
    /// </summary>
    private string PathUrl
    {
        get
        {
            return Application.streamingAssetsPath + "/";
        }
    }

    /// <summary>
    /// 主包名 方便修改
    /// </summary>
    private string MainABName
    {
        get
        {
#if UNITY_IOS
            return "IOS";
#elif UNITY_ANDROID
            return "Android";
#else
            return "PC";
#endif
        }
    }

    /// <summary>
    /// 加载AB包
    /// </summary>
    /// <param name="abName"></param>
    public void LoadAB(string abName)
    {
        // 加载AB包
        if (mainAB == null)
        {
            mainAB = AssetBundle.LoadFromFile(PathUrl + MainABName); // 主包
            manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest"); // 依赖包获取用的配置文件
        }
        // 获取依赖包相关信息
        AssetBundle ab = null; // 加载AB包用的零时变量
        string[] strs = manifest.GetAllDependencies(abName);
        for (int i = 0; i < strs.Length; i++)
        {
            // 判断包是否加载
            if (abDic.ContainsKey(strs[i]))
            {
                ab = AssetBundle.LoadFromFile(PathUrl + strs[i]);
                abDic.Add(strs[i], ab);
            }
        }
        // 加载资源来源包   
        if (!abDic.ContainsKey(abName)) // 如果没有加载过 再加载
        {
            ab = AssetBundle.LoadFromFile(PathUrl + abName);
            abDic.Add(abName, ab);
        }
    }

    /// <summary>
    /// 同步加载 不指定类型 GameObject会自动实例化再返回
    /// </summary>
    /// <param name="abName">目标包</param>
    /// <param name="resName">资源</param>
    /// <returns></returns>
    public Object LoadRes(string abName, string resName)
    {
        // 加载AB包
        LoadAB(abName);
        // 加载资源
        // 为了外面方便, 在加载资源时 判断一下 资源是不是GameObject
        // 如果是 直接实例化 再返回给外部
        Object obj = abDic[abName].LoadAsset(resName);
        if (obj is GameObject)
            return Instantiate(obj);
        else
            return obj;
    }

    /// <summary>
    /// 同步加载 根据type指定类型 LUA中不支持泛型加载, 常使用这种方法.
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public Object LoadRes(string abName, string resName, System.Type type)
    {
        // 加载AB包
        LoadAB(abName);
        // 加载资源
        Object obj = abDic[abName].LoadAsset(resName, type);
        if (obj is GameObject)
            return Instantiate(obj);
        else
            return obj;
    }

    /// <summary>
    /// 同步加载 根据泛型指定类型 C#常使用这种方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <returns></returns>
    public T LoadRes<T>(string abName, string resName) where T : Object
    {
        // 加载AB包
        LoadAB(abName);
        // 加载资源
        T obj = abDic[abName].LoadAsset<T>(resName);
        if (obj is GameObject)
            return Instantiate(obj);
        else
            return obj;
    }
    // 异步加载
    // 这里的异步加载 AB包并没有使用异步加载 只是从AB包中 加载资源时 使用异步

    /// <summary>
    /// 根据名字 异步加载资源
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="callBack"></param>
    public void LoadResAsyne(string abName, string resName, UnityAction<Object> callBack)
    {
        StartCoroutine(ReallyLoadResAsync(abName, resName, callBack));
    }
    private IEnumerator ReallyLoadResAsync(string abName, string resName, UnityAction<Object> callBack)
    {
        // 加载AB包
        LoadAB(abName);
        // 加载资源
        // 为了外面方便, 在加载资源时 判断一下 资源是不是GameObject
        // 如果是 直接实例化 再返回给外部
        AssetBundleRequest abr = abDic[abName].LoadAssetAsync(resName);
        yield return abr;
        // 异步加载结束后, 通过委托传递给外部 外部来使用
        if (abr.asset is GameObject)
            callBack(Instantiate(abr.asset));
        else
            callBack(abr.asset);
    }

    /// <summary>
    /// 通过Type异步加载资源
    /// </summary>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="type"></param>
    /// <param name="callBack"></param>
    public void LoadResAsyne(string abName, string resName, System.Type type, UnityAction<Object> callBack)
    {
        StartCoroutine(ReallyLoadResAsync(abName, resName, type, callBack));
    }
    private IEnumerator ReallyLoadResAsync(string abName, string resName, System.Type type, UnityAction<Object> callBack)
    {
        // 加载AB包
        LoadAB(abName);
        // 加载资源
        // 为了外面方便, 在加载资源时 判断一下 资源是不是GameObject
        // 如果是 直接实例化 再返回给外部
        AssetBundleRequest abr = abDic[abName].LoadAssetAsync(resName, type);
        yield return abr;
        // 异步加载结束后, 通过委托传递给外部 外部来使用
        if (abr.asset is GameObject)
            callBack(Instantiate(abr.asset));
        else
            callBack(abr.asset);
    }

    /// <summary>
    /// 根据泛型 异步加载资源
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="abName"></param>
    /// <param name="resName"></param>
    /// <param name="callBack"></param>
    public void LoadResAsyne<T>(string abName, string resName, UnityAction<T> callBack) where T : Object
    {
        StartCoroutine(ReallyLoadResAsync<T>(abName, resName, callBack));
    }
    private IEnumerator ReallyLoadResAsync<T>(string abName, string resName, UnityAction<T> callBack) where T : Object
    {
        // 加载AB包
        LoadAB(abName);
        // 加载资源
        // 为了外面方便, 在加载资源时 判断一下 资源是不是GameObject
        // 如果是 直接实例化 再返回给外部
        AssetBundleRequest abr = abDic[abName].LoadAssetAsync<T>(resName);
        yield return abr;
        // 异步加载结束后, 通过委托传递给外部 外部来使用
        if (abr.asset is GameObject)
            callBack(Instantiate(abr.asset) as T);
        else
            callBack(abr.asset as T);
    }


    // 单个包卸载
    public void UnLoad(string abName)
    {
        if (abDic.ContainsKey(abName))
        {
            abDic[abName].Unload(false);
            abDic.Remove(abName);
        }
    }
    // 所有包的卸载
    public void ClearAB()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        abDic.Clear();
        mainAB = null;
        manifest = null;
    }
}
