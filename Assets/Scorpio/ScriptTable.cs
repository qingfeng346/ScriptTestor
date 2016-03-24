﻿using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text;
namespace Scorpio
{
    //脚本table类型
    public class ScriptTable : ScriptObject
    {
        public override ObjectType Type { get { return ObjectType.Table; } }
        private Dictionary<object, ScriptObject> m_listObject = new Dictionary<object, ScriptObject>();  //所有的数据(函数和数据都在一个数组)
        public ScriptTable(Script script) : base(script) { }
        public override void SetValue(object key, ScriptObject value)
        {
			if (key == null) return;
            Util.SetObject(m_listObject, key, value);
        }
        public override ScriptObject GetValue(object key)
        {
			if (key == null) return Script.Null;
            return m_listObject.ContainsKey(key) ? m_listObject[key] : Script.Null;
        }
        public bool HasValue(object key)
        {
			if (key == null) return false;
            return m_listObject.ContainsKey(key);
        }
        public int Count()
        {
            return m_listObject.Count;
        }
        public void Clear()
        {
            m_listObject.Clear();
        }
        public void Remove(object key) {
            m_listObject.Remove(key);
        }
		public ScriptArray GetKeys() {
			ScriptArray ret = Script.CreateArray ();
			foreach (KeyValuePair<object, ScriptObject> pair in m_listObject) {
				ret.Add(Script.CreateObject(pair.Key));
			}
			return ret;
		}
		public ScriptArray GetValues() {
			ScriptArray ret = Script.CreateArray ();
			foreach (KeyValuePair<object, ScriptObject> pair in m_listObject) {
				ret.Add(pair.Value.Assign());
			}
			return ret;
		}
        public Dictionary<object, ScriptObject>.Enumerator GetIterator() {
            return m_listObject.GetEnumerator();
        }
        public override ScriptObject Clone() {
            ScriptTable ret = Script.CreateTable();
            ScriptObject obj = null;
            ScriptFunction func = null;
            foreach (KeyValuePair<object, ScriptObject> pair in m_listObject) {
                if (pair.Value == this) {
                    ret.m_listObject[pair.Key] = ret;
                } else {
                    obj = pair.Value.Clone();
                    if (obj is ScriptFunction) {
                        func = (ScriptFunction)obj;
                        if (!func.IsStatic) func.SetTable(ret);
                    }
                    ret.m_listObject[pair.Key] = obj;
                }
            }
            return ret;
        }
        public override string ToString() { return "Table"; }
        public override string ToJson()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{");
            bool first = true;
            foreach (KeyValuePair<object, ScriptObject> pair in m_listObject) {
                if (first)
                    first = false;
                else
                    builder.Append(",");
                builder.Append("\"");
                builder.Append(pair.Key);
                builder.Append("\":");
                builder.Append(pair.Value.ToJson());
            }
            builder.Append("}");
            return builder.ToString();
        }
    }
}
