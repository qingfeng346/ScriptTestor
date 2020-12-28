using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ILRuntime.Runtime.Enviorment;
using Scorpio;
using Scorpio.Userdata;
using UnityEngine;
using XLua;
using System.Text;
public class NewBehaviourScript : MonoBehaviour {
    // Start is called before the first frame update
    ILRuntime.Runtime.Enviorment.AppDomain appdomain;
    LuaEnv lua;
    Script sco;
    System.IO.MemoryStream fs;
    System.IO.MemoryStream p;
    void Start () {
        lua = new LuaEnv ();
        lua.DoString (Resources.Load<TextAsset> ("lua").bytes);
        sco = new Script ();
        sco.LoadLibraryV1 ();
        TypeManager.PushAssembly (typeof (GameObject).Assembly);
        sco.LoadBuffer (Resources.Load<TextAsset> ("sco").bytes);
        StartCoroutine (LoadHotFixAssembly ());
    }
    IEnumerator LoadHotFixAssembly () {
        //首先实例化ILRuntime的AppDomain，AppDomain是一个应用程序域，每个AppDomain都是一个独立的沙盒
        appdomain = new ILRuntime.Runtime.Enviorment.AppDomain ();
        //正常项目中应该是自行从其他地方下载dll，或者打包在AssetBundle中读取，平时开发以及为了演示方便直接从StreammingAssets中读取，
        //正式发布的时候需要大家自行从其他地方读取dll

        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //这个DLL文件是直接编译HotFix_Project.sln生成的，已经在项目中设置好输出目录为StreamingAssets，在VS里直接编译即可生成到对应目录，无需手动拷贝
        //工程目录在Assets\Samples\ILRuntime\1.6\Demo\HotFix_Project~
        //以下加载写法只为演示，并没有处理在编辑器切换到Android平台的读取，需要自行修改
#if UNITY_ANDROID
        WWW www = new WWW (Application.streamingAssetsPath + "/netstandard2.0/HotFix_Project.dll");
#else
        WWW www = new WWW ("file:///" + Application.streamingAssetsPath + "/netstandard2.0/HotFix_Project.dll");
#endif
        while (!www.isDone)
            yield return null;
        if (!string.IsNullOrEmpty (www.error))
            UnityEngine.Debug.LogError (www.error);
        byte[] dll = www.bytes;
        www.Dispose ();

        //PDB文件是调试数据库，如需要在日志中显示报错的行号，则必须提供PDB文件，不过由于会额外耗用内存，正式发布时请将PDB去掉，下面LoadAssembly的时候pdb传null即可
// #if UNITY_ANDROID
//         www = new WWW (Application.streamingAssetsPath + "/HotFix_Project.pdb");
// #else
//         www = new WWW ("file:///" + Application.streamingAssetsPath + "/HotFix_Project.pdb");
// #endif
//         while (!www.isDone)
//             yield return null;
//         if (!string.IsNullOrEmpty (www.error))
//             UnityEngine.Debug.LogError (www.error);
//         byte[] pdb = www.bytes;
        fs = new MemoryStream (dll);
        // p = new MemoryStream (pdb);
        try {
            appdomain.LoadAssembly (fs, null, new ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider ());
        } catch { }
#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
        //由于Unity的Profiler接口只允许在主线程使用，为了避免出异常，需要告诉ILRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
        appdomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
        UnityEngine.Debug.Log ("ILRuntime 加载完成");
        // OnHotFixLoaded();
    }
    void OnGUI () {
        if (GUI.Button (new Rect (100, 100, 100, 100), "test")) {
            StartCoroutine(Test());
        }
    }
    IEnumerator Test()
    {
        var builder = new StringBuilder();
        yield return StartCoroutine(TestFunc("脚本自身运算", "func1", builder));
        yield return StartCoroutine(TestFunc("访问c# 函数", "func2", builder));
        yield return StartCoroutine(TestFunc("访问c# Field", "func3", builder));
        yield return StartCoroutine(TestFunc("访问c# Property", "func4", builder));
        yield return StartCoroutine(TestFunc("访问c# 重载函数", "func5", builder));
        UnityEngine.Debug.LogWarning(builder.ToString());
    }
    IEnumerator TestFunc(string comment, string func, StringBuilder builder) {
        var scoTime = new List<long>();
        var luaTime = new List<long>();
        var ilTime = new List<long>();
        int time = 10;
        for (var i = 0; i < time; ++i) {
            var stop = Stopwatch.StartNew ();
            sco.call(func);
            scoTime.Add(stop.ElapsedMilliseconds);
            yield return null;
        }
        for (var i = 0; i < time; ++i) {
            var stop = Stopwatch.StartNew ();
            lua.Global.Get<LuaFunction> (func).Call ();
            luaTime.Add(stop.ElapsedMilliseconds);
            yield return null;
        }
        for (var i = 0; i < time; ++i) {
            var stop = Stopwatch.StartNew ();
            appdomain.Invoke ("HotFix_Project.InstanceClass", func, null, null);
            ilTime.Add(stop.ElapsedMilliseconds);
            yield return null;
        }
        var str = $"{comment}  Sco平均耗时:{GetTime(scoTime)}  Lua平均耗时:{GetTime(luaTime)}  ILRuntime平均耗时:{GetTime(ilTime)}";
        builder.AppendLine(str);
        UnityEngine.Debug.Log(str);
        yield return new WaitForSeconds(1);
    }
    long GetTime(List<long> time)
    {
        time.Sort();
        time.RemoveAt(0);
        time.RemoveAt(time.Count - 1);
        long count = 0;
        time.ForEach(_ => count += _);
        return count / time.Count;
    }
}