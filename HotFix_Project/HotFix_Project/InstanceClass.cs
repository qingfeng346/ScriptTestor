using System;
using System.Collections.Generic;
using UnityEngine;
namespace HotFix_Project
{
    public class InstanceClass
    {
        // static method
        public static double func1()
        {
            double eee = 0;
            for (var i = 0; i <= 1000000; ++i) {
                var a = i + 10;
                var b = 2.3;
                if (a < b) {
                    a = a + 10;
                } else {
                    b = b + 10;
                }
                if (a == b) {
                    b = b + 10;
                }
                eee = eee + a * b + a / b;
            }
            return eee;
        }
        public static void func2() {
            for (var i = 0; i <= 100000; ++i) {
                UnityUtil.TestFunc2(100, 200);
            }
        }
        public static void func3() {
            for (var i = 0; i <= 100000; ++i) {
                var t = UnityUtil.TestFunc3;
                UnityUtil.TestFunc3 = t;
            }
        }
        public static void func4() {
            for (var i = 0; i <= 100000; ++i) {
                var t = UnityUtil.TestFunc4;
                UnityUtil.TestFunc4 = t;
            }
        }
        public static void func5() {
            for (var i = 0; i <= 100000; ++i) {
                UnityUtil.TestFunc5("1", i);
            }
        }
    }
}
