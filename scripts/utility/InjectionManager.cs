using System;
using System.Collections.Generic;
using Godot;

public static class InjectionManager
{
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
    
    //only use this if the injectable deregisters itself!
    public static T GetOrCreate<T>() where T : IInjectable, new()
    {
        if (s_injectableMap.TryGetValue(typeof(T), out IInjectable injectableT))
        {
            return (T)injectableT;
        }
        
        var newT = new T();
        s_injectableMap.Add(typeof(T), newT);
        
        return newT;
    }

    public static bool Has<T>() where T : IInjectable
    {
        return s_injectableMap.ContainsKey(typeof(T));
    }
}