using System;
using System.Collections.Generic;
using System.Reflection;

namespace ASeKi.evt
{
    public class EventHandler
    {
        private readonly Dictionary<int, Delegate> callbackDict = new Dictionary<int, Delegate>(521);

        public void Clear()
        {
            callbackDict.Clear();
        }

        #region Register

        public void Register(int id, Action callback)
        {
            callbackDict.TryGetValue(id, out Delegate del);
            if(del == null)
            {
                callbackDict[id] = callback;
                return;
            }

            if(del is Action callbacks)
            {
                checkDupAction(callbacks, callback);
                callbackDict[id] = callbacks + callback;
            }
            else
            {
                logErrorRegisterError(del.Method, callback.Method);
            }
        }

        public void Register<T>(int id, Action<T> callback)
        {
            callbackDict.TryGetValue(id, out Delegate del);
            if(del == null)
            {
                callbackDict[id] = callback;
                return;
            }

            if(del is Action<T> callbacks)
            {
                checkDupAction(callbacks, callback);
                callbackDict[id] = callbacks + callback;
            }
            else
            {
                logErrorRegisterError(del.Method, callback.Method);
            }
        }

        public void Register<T1, T2>(int id, Action<T1, T2> callback)
        {
            callbackDict.TryGetValue(id, out Delegate del);
            if(del == null)
            {
                callbackDict[id] = callback;
                return;
            }

            if(del is Action<T1, T2> callbacks)
            {
                checkDupAction(callbacks, callback);
                callbackDict[id] = callbacks + callback;
            }
            else
            {
                logErrorRegisterError(del.Method, callback.Method);
            }
        }

        public void Register<T1, T2, T3>(int id, Action<T1, T2, T3> callback)
        {
            callbackDict.TryGetValue(id, out Delegate del);
            if(del == null)
            {
                callbackDict[id] = callback;
                return;
            }

            if(del is Action<T1, T2, T3> callbacks)
            {
                checkDupAction(callbacks, callback);
                callbackDict[id] = callbacks + callback;
            }
            else
            {
                logErrorRegisterError(del.Method, callback.Method);
            }
        }

        public void Register<T1, T2, T3, T4>(int id, Action<T1, T2, T3, T4> callback)
        {
            callbackDict.TryGetValue(id, out Delegate del);
            if(del == null)
            {
                callbackDict[id] = callback;
                return;
            }

            if(del is Action<T1, T2, T3, T4> callbacks)
            {
                checkDupAction(callbacks, callback);
                callbackDict[id] = callbacks + callback;
            }
            else
            {
                logErrorRegisterError(del.Method, callback.Method);
            }
        }

        public void Register<T1, T2, T3, T4, T5>(int id, Action<T1, T2, T3, T4, T5> callback)
        {
            callbackDict.TryGetValue(id, out Delegate del);
            if(del == null)
            {
                callbackDict[id] = callback;
                return;
            }

            if(del is Action<T1, T2, T3, T4, T5> callbacks)
            {
                checkDupAction(callbacks, callback);
                callbackDict[id] = callbacks + callback;
            }
            else
            {
                logErrorRegisterError(del.Method, callback.Method);
            }
        }

        #endregion

        #region Unregister
        public void Unregister(int id, Action callback)
        {
            callbackDict.TryGetValue(id, out Delegate del);
            if(del is Action callbacks)
            {
                callbackDict[id] = callbacks - callback;
            }
        }

        public void Unregister<T>(int id, Action<T> callback)
        {
            callbackDict.TryGetValue(id, out Delegate del);
            if(del is Action<T> callbacks)
            {
                callbackDict[id] = callbacks - callback;
            }
        }

        public void Unregister<T1, T2>(int id, Action<T1, T2> callback)
        {
            callbackDict.TryGetValue(id, out Delegate del);
            if(del is Action<T1, T2> callbacks)
            {
                callbackDict[id] = callbacks - callback;
            }
        }

        public void Unregister<T1, T2, T3>(int id, Action<T1, T2, T3> callback)
        {
            callbackDict.TryGetValue(id, out Delegate del);
            if(del is Action<T1, T2, T3> callbacks)
            {
                callbackDict[id] = callbacks - callback;
            }
        }

        public void Unregister<T1, T2, T3, T4>(int id, Action<T1, T2, T3, T4> callback)
        {
            callbackDict.TryGetValue(id, out Delegate del);
            if(del is Action<T1, T2, T3, T4> callbacks)
            {
                callbackDict[id] = callbacks - callback;
            }
        }

        public void Unregister<T1, T2, T3, T4, T5>(int id, Action<T1, T2, T3, T4, T5> callback)
        {
            callbackDict.TryGetValue(id, out Delegate del);
            if(del is Action<T1, T2, T3, T4, T5> callbacks)
            {
                callbackDict[id] = callbacks - callback;
            }
        }

        #endregion

        #region Send
        public void Send(int id)
        {
            callbackDict.TryGetValue(id, out Delegate del);
            if(del == null)
            {
                return;
            }

            Action action = del as Action;
            debug.PrintSystem.Assert(action != null, $"Action cast error, need: {del.Method}, pass: ()");
            action?.Invoke();
        }

        public void Send<T>(int id, T arg)
        {
            callbackDict.TryGetValue(id, out Delegate del);
            if(del == null)
            {
                return;
            }

            Action<T> action = del as Action<T>;
            debug.PrintSystem.Assert(action != null, $"Action cast error, need: {del.Method}, pass: (${arg?.GetType()})");
            action?.Invoke(arg);
        }

        public void Send<T1, T2>(int id, T1 arg1, T2 arg2)
        {
            callbackDict.TryGetValue(id, out Delegate del);
            if(del == null)
            {
                return;
            }

            Action<T1, T2> action = del as Action<T1, T2>;
            debug.PrintSystem.Assert(action != null, $"Action cast error, need: {del.Method}, pass: (${arg1?.GetType()}, ${arg2?.GetType()})");
            action?.Invoke(arg1, arg2);
        }

        public void Send<T1, T2, T3>(int id, T1 arg1, T2 arg2, T3 arg3)
        {
            callbackDict.TryGetValue(id, out Delegate del);
            if(del == null)
            {
                return;
            }

            Action<T1, T2, T3> action = del as Action<T1, T2, T3>;
            debug.PrintSystem.Assert(action != null, $"Action cast error, need: {del.Method}, pass: (${arg1?.GetType()}, ${arg2?.GetType()}, ${arg3?.GetType()})");
            action?.Invoke(arg1, arg2, arg3);
        }

        public void Send<T1, T2, T3, T4>(int id, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            callbackDict.TryGetValue(id, out Delegate del);
            if(del == null)
            {
                return;
            }

            Action<T1, T2, T3, T4> action = del as Action<T1, T2, T3, T4>;
            debug.PrintSystem.Assert(action != null, $"Action cast error, need: {del.Method}, pass: (${arg1?.GetType()}, ${arg2?.GetType()}, ${arg3?.GetType()}, ${arg4?.GetType()})");
            action?.Invoke(arg1, arg2, arg3, arg4);
        }

        public void Send<T1, T2, T3, T4, T5>(int id, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            callbackDict.TryGetValue(id, out Delegate del);
            if(del == null)
            {
                return;
            }

            Action<T1, T2, T3, T4, T5> action = del as Action<T1, T2, T3, T4, T5>;
            debug.PrintSystem.Assert(action != null, $"Action cast error, need: {del.Method}, pass: (${arg1?.GetType()}, ${arg2?.GetType()}, ${arg3?.GetType()}, ${arg4?.GetType()}, ${arg5?.GetType()})");
            action?.Invoke(arg1, arg2, arg3, arg4, arg5);
        }
        #endregion

        private static void logErrorRegisterError(MethodInfo delMethodInfo, MethodInfo actionMethodInfo)
        {
            debug.PrintSystem.LogError($"[EventHandler] Cannot register different types of callback functions in the same Event ID, need: {delMethodInfo}, pass: {actionMethodInfo}");
        }

        #region 检查是否注册了两次

        [System.Diagnostics.Conditional("SeKiDebug")]
        private static void checkDupAction(Action callbacks, Action action)
        {
            Delegate[] list = callbacks.GetInvocationList();
            debug.PrintSystem.Assert(Array.IndexOf(list, action) == -1, $"Can't register same event, {action.Method}");
        }

        [System.Diagnostics.Conditional("SeKiDebug")]
        private static void checkDupAction<T>(Action<T> callbacks, Action<T> action)
        {
            Delegate[] list = callbacks.GetInvocationList();
            debug.PrintSystem.Assert(Array.IndexOf(list, action) == -1, $"Can't register same event, {action.Method}");
        }

        [System.Diagnostics.Conditional("SeKiDebug")]
        private static void checkDupAction<T1, T2>(Action<T1, T2> callbacks, Action<T1, T2> action)
        {
            Delegate[] list = callbacks.GetInvocationList();
            debug.PrintSystem.Assert(Array.IndexOf(list, action) == -1, $"Can't register same event, {action.Method}");
        }

        [System.Diagnostics.Conditional("SeKiDebug")]
        private static void checkDupAction<T1, T2, T3>(Action<T1, T2, T3> callbacks, Action<T1, T2, T3> action)
        {
            Delegate[] list = callbacks.GetInvocationList();
            debug.PrintSystem.Assert(Array.IndexOf(list, action) == -1, $"Can't register same event, {action.Method}");
        }

        [System.Diagnostics.Conditional("SeKiDebug")]
        private static void checkDupAction<T1, T2, T3, T4>(Action<T1, T2, T3, T4> callbacks, Action<T1, T2, T3, T4> action)
        {
            Delegate[] list = callbacks.GetInvocationList();
            debug.PrintSystem.Assert(Array.IndexOf(list, action) == -1, $"Can't register same event, {action.Method}");
        }

        [System.Diagnostics.Conditional("SeKiDebug")]
        private static void checkDupAction<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> callbacks, Action<T1, T2, T3, T4, T5> action)
        {
            Delegate[] list = callbacks.GetInvocationList();
            debug.PrintSystem.Assert(Array.IndexOf(list, action) == -1, $"Can't register same event, {action.Method}");
        }

        #endregion
    }
}