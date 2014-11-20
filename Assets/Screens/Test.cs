using UnityEngine;
using System.Collections;
using System.Diagnostics;
using Scorpio;
using CSLE;
using LuaInterface;
using System.Collections.Generic;
using System.IO;
public class ScriptLogger : ICLS_Logger
{
    public void Log(string str)
    {

    }
    public void Log_Warn(string str)
    {

    }
    public void Log_Error(string str)
    {

    }
}
public class Test : MonoBehaviour {
    const int Start = 50;
    const int Label_Left = 0;
    const int Text_Height = 150;
    const int Text_Space = 20;
    const int Script_Space = 170;

    private string m_Scorpio = "";
    private string m_Lua = "";
    private string m_CLS = "";

    private string m_ScorpioRuntime = "Scorpio";
    private string m_LuaRuntime = "Lua";
    private string m_CLSRuntime = "CLS";

    private string[] TestList;
    private int m_Select = -1;
    void Awake()
    {
        string[] Dirs = Directory.GetDirectories(Application.dataPath + "/Resources", "*", SearchOption.TopDirectoryOnly);
        List<string> Paths = new List<string>();
        foreach (var dir in Dirs)
        {
            Paths.Add(Path.GetFileName(dir));
        }
        TestList = Paths.ToArray();
    }
    void OnGUI()
    {
        int old = m_Select;
        m_Select = GUI.Toolbar(new Rect(0, 0, Screen.width, Start), m_Select, TestList);
        if (m_Select != old)
        {
            string str = TestList[m_Select];
            m_Scorpio = (Resources.Load(str + "/sco") as TextAsset).text;
            m_Lua = (Resources.Load(str + "/lua") as TextAsset).text;
            m_CLS = (Resources.Load(str + "/cls") as TextAsset).text;
        }
        GUI.Label(new Rect(Label_Left, Start, Screen.width, 50), m_ScorpioRuntime);
        m_Scorpio = GUI.TextArea(new Rect(Label_Left, Start + Text_Space, Screen.width - Label_Left, Text_Height), m_Scorpio);

        GUI.Label(new Rect(Label_Left, Start + Script_Space, Screen.width, 50), m_LuaRuntime);
        m_Lua = GUI.TextArea(new Rect(Label_Left, Start + Script_Space + Text_Space, Screen.width - Label_Left, Text_Height), m_Lua);

        GUI.Label(new Rect(Label_Left, Start + Script_Space * 2, Screen.width, 50), m_CLSRuntime);
        m_CLS = GUI.TextArea(new Rect(Label_Left, Start + Script_Space * 2 + Text_Space, Screen.width - Label_Left, Text_Height), m_CLS);

        if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height - 70, 200, 50), "Run"))
        {
            Stopwatch watch = null;
            {
                watch = Stopwatch.StartNew();
                Script env = new Script();
                env.LoadLibrary();
                env.PushAssembly(typeof(GameObject).Assembly);
                var token = env.LoadString(m_Scorpio);
                m_ScorpioRuntime = string.Format("Scorpio 耗时:{0} ms  返回值 : {1}", watch.ElapsedMilliseconds, token);
            }
            {
                watch = Stopwatch.StartNew();
                LuaState env = new LuaState();
                var token = env.DoString(m_Lua);
                m_LuaRuntime = string.Format("Lua 耗时:{0} ms  返回值 : {1}", watch.ElapsedMilliseconds, token != null && token.Length > 0 ? token[0] : "null");
            }
            {
                CLS_Environment env = new CLS_Environment(new ScriptLogger());
                env.RegType(new RegHelper_Type(typeof(GameObject)));
                var token = env.ParserToken(m_CLS);
                var expr = env.Expr_CompilerToken(token, false);
                watch = Stopwatch.StartNew();
                var value = env.Expr_Execute(expr, null);
                m_CLSRuntime = string.Format("cls 耗时:{0} ms  返回值 : {1} (因听说CLS解析很慢 故只计算Expr_Execute的时间)", watch.ElapsedMilliseconds, value);
            }
        }
    }
}
