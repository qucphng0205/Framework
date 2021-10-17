using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T m_Instance = null;
    public static bool IsInitialized { get; private set; }
    public static bool isAppClosing = false;

    [SerializeField] bool dontDestroyOnLoad = false;

    public static T Instance
    {
        get
        {
            if (isAppClosing || !Application.isPlaying)
                return null;

            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType(typeof(T)) as T;

                if (m_Instance == null)
                {
                    Debug.LogWarning("No instance of " + typeof(T).ToString() + ", a temporary one is created.");

                    m_Instance = new GameObject("Temp Instance of " + typeof(T).ToString(), typeof(T)).GetComponent<T>();
                }
                if (!IsInitialized)
                {
                    IsInitialized = true;
                    m_Instance.Init();
                }
            }
            return m_Instance;
        }
    }

    protected virtual void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this as T;
            IsInitialized = true;
            m_Instance.Init();
        }
        else if (m_Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    protected void DontDestroy()
    {
        if (m_Instance == null)
        {
            m_Instance = this as T;
        }
        else if (m_Instance != this)
        {
            Debug.LogError("Another instance of " + GetType() + " is already exist! Destroying self...");
            DestroyImmediate(this);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    protected virtual void Init() {
        if (dontDestroyOnLoad)
            DontDestroy();
    }

    protected virtual void OnDestroy()
    {
        if (this == m_Instance)
        {
            m_Instance = null;
            IsInitialized = false;
        }
    }

    private void OnApplicationQuit()
    {
        m_Instance = null;
        isAppClosing = true;
    }
}