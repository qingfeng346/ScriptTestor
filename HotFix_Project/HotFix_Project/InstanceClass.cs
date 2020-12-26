using System;
using System.Collections.Generic;
using UnityEngine;
namespace HotFix_Project
{
    public class InstanceClass
    {
        // static method
        public static void func1()
        {
            double eee = 0;
            for (var i = 0; i <= 1000000; ++i) {
                var a = i + 1;
                var b = 2.3;
                if (a < b) {
                    a = a + 1;
                } else {
                    b = b + 1;
                }
                if (a == b) {
                    b = b + 1;
                }
                eee = eee + a * b + a / b;
            }
        }
        public static void func2() {
            for (var i = 0; i <= 50000; ++i) {
                UnityUtil.TestFunc2(100, 200);
            }
        }
        public static void func3() {
            for (var i = 0; i <= 50000; ++i) {
                UnityUtil.TestFunc3 = i;
            }
        }
        public static void func4() {
            for (var i = 0; i <= 50000; ++i) {
                UnityUtil.TestFunc4 = i;
            }
        }
        public static void func5() {
            for (var i = 0; i <= 50000; ++i) {
                UnityUtil.TestFunc5("1", i);
            }
        }
    }
}
