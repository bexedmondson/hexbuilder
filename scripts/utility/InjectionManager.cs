using System;
using System.Collections.Generic;
using Godot;

[Tool]
public class InjectionManager
{
    private static InjectionManager s_instance;
    public static InjectionManager Instance()
    {
        if (s_instance == null)
            s_instance = new InjectionManager();
        return s_instance;
    }

    private static Dictionary<Type, IInjectable> s_injectableMap = new Dictionary<Type, IInjectable>();

    public static void Register<T>(T injectable) where T : IInjectable
    {
        if (s_injectableMap.ContainsKey(typeof(T)))
        {
            GD.PushError("[InjectionManager] Injectable of type " + typeof(T).AssemblyQualifiedName + " already registered! Discarding new.");
            return;
        }

        s_injectableMap.Add(typeof(T), injectable);
    }
    
    public static void Deregister<T>(T injectable) where T : IInjectable
    {
        if (!s_injectableMap.TryGetValue(typeof(T), out var registered) || !registered.Equals(injectable))
        {
            GD.PushError($"[InjectionManager] Injectable {injectable} of type {typeof(T).AssemblyQualifiedName} not registered! Discarding attempt to deregister.");
            return;
        }

        s_injectableMap.Remove(typeof(T));
    }

    public static T Get<T>() where T : IInjectable
    {
        if (s_injectableMap.TryGetValue(typeof(T), out IInjectable injectableT))
        {
            return (T)injectableT;
        }
        return default(T);
    }

    public static bool Has<T>() where T : IInjectable
    {
        return s_injectableMap.ContainsKey(typeof(T));
    }
}