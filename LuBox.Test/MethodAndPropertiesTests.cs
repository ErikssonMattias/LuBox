﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LuBox.Compiler;

namespace LuBox.Test
{
    [TestClass]
    public class MethodAndPropertiesTests
    {
        private LuScriptEngine _luScriptEngine;

        [TestInitialize]
        public void Initialize()
        {
            _luScriptEngine = new LuScriptEngine();
        }

        [TestMethod]
        public void CallMember()
        {
            var getter = new CallReceiver();
            _luScriptEngine.Globals.callReceiver = getter;
            _luScriptEngine.Execute("callReceiver:Call()");
            Assert.AreEqual("Call", getter.Called);
        }

        [TestMethod]
        public void CallMemberVoid()
        {
            var getter = new CallReceiver();
            _luScriptEngine.SetGlobal("callReceiver", getter);
            _luScriptEngine.Execute("callReceiver:CallVoid()");
            Assert.AreEqual("CallVoid", getter.Called);
        }

        [TestMethod]
        public void CallMemberVoidString()
        {
            var getter = new CallReceiver();
            _luScriptEngine.SetGlobal("callReceiver", getter);
            _luScriptEngine.Execute("callReceiver:CallVoid(\"Hello\")");
            Assert.AreEqual("CallVoid(messages)", getter.Called);
            Assert.AreEqual(1, getter.Counter);
        }

        [TestMethod]
        public void CallMemberParams()
        {
            var getter = new CallReceiver();
            _luScriptEngine.SetGlobal("callReceiver", getter);
            _luScriptEngine.Execute("callReceiver:CallVoidParams(\"Hello\", 123, 456, 0.3)");
            Assert.AreEqual("CallVoidParams", getter.Called);
        }

        [TestMethod]
        public void GetProperty()
        {
            var getter = new CallReceiver();
            _luScriptEngine.SetGlobal("callReceiver", getter);
            var propValue = _luScriptEngine.Evaluate<string>("callReceiver.Property");
            Assert.AreEqual("value", propValue);
        }

        [TestMethod]
        public void GetPropertyThenCall()
        {
            var getter = new CallReceiver();
            _luScriptEngine.SetGlobal("callReceiver", getter);
            var propValue = _luScriptEngine.Evaluate<string>("callReceiver.Property:ToString()");
            Assert.AreEqual("value", propValue);
        }

        [TestMethod]
        public void UseGlobalVariable()
        {
            _luScriptEngine.SetGlobal("factor", 3);
            var result = _luScriptEngine.Evaluate<int>("factor * 3");
            Assert.AreEqual(9, result);
        }

        [TestMethod]
        public void ChainProperties()
        {
            var getter = new CallReceiver();
            _luScriptEngine.SetGlobal("callReceiver", getter);
            var propValue = _luScriptEngine.Evaluate<CallReceiver>("callReceiver.Self.Self.Self");
            Assert.AreSame(getter, propValue);
        }

        [TestMethod]
        public void ChainMethods()
        {
            var getter = new CallReceiver();
            _luScriptEngine.SetGlobal("callReceiver", getter);
            var propValue = _luScriptEngine.Evaluate<CallReceiver>("callReceiver:GetSelf():GetSelf():GetSelf()");
            Assert.AreSame(getter, propValue);
            Assert.AreEqual(3, getter.Counter);
        }

        [TestMethod]
        public void ChainMethodAndProperties()
        {
            var getter = new CallReceiver();
            _luScriptEngine.SetGlobal("callReceiver", getter);
            var propValue = _luScriptEngine.Evaluate<String>("callReceiver:GetSelf().Property:ToUpper()");
            Assert.AreEqual(getter.Property.ToUpper(), propValue);
        }

        public class CallReceiver
        {
            private string _called;
            private int _counter;

            public string Called
            {
                get { return _called; }
            }

            public int Counter
            {
                get { return _counter; }
            }

            public string Property
            {
                get { return "value"; }
            }

            public CallReceiver Self
            {
                get { return this; }
            }

            public CallReceiver GetSelf()
            {
                _counter++;
                return this; 
            }

            public object Call()
            {
                _counter++;
                _called = "Call";
                return _called;
            }

            public void CallVoid()
            {
                _counter++;
                _called = "CallVoid";
            }

            public void CallVoid(string messages)
            {
                _counter++;
                _called = "CallVoid(messages)";
            }

            public void CallVoidParams(string messages, params object[] args)
            {
                _counter++;
                _called = "CallVoidParams";
            }
        }
    }
}
