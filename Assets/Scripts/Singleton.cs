using System;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where  T : MonoBehaviour
{
    public static T Instance {get; private set;}
    public static bool IsReady => Instance != null;
    public static event Action OnReady;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"An instance of {typeof(T)} already exists!");
            Destroy(this.gameObject);
            return;
        }

        HandleAwake();
        Instance = this.GetComponent<T>();
        OnReady?.Invoke();
    }

    private void OnDestroy()
    {
        if (Instance != this)
            return;
        
        HandleDestroy();
        Instance = null;
    }

    protected virtual void HandleAwake() {}
    protected virtual void HandleDestroy() {}
}