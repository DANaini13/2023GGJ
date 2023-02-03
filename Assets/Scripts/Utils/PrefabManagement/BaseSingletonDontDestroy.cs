using UnityEngine;

public class BaseSingletonDontDestroy<T> : MonoBehaviour where T : BaseSingletonDontDestroy<T>
{

    protected static T _instance = null;

    public static T Instance
    {
        get
        {

            if ( _instance==null)
            {
                GameObject go = GameObject.Find("SingletonsDontDestroy");

                if (go == null)
                {
                    go = new GameObject("SingletonsDontDestroy");
                    DontDestroyOnLoad(go);
                }

                _instance = go.GetComponent<T>();


                if (_instance == null)
                {
                    _instance = go.AddComponent<T>();
                }

            }
            return _instance;
        }
    }
}

public class BaseSingleton<T> : MonoBehaviour where T : BaseSingleton<T>
{
    protected static T _instance = null;
    public static T Instance
    {
        get
        {

            if ( _instance==null)
            {
                GameObject go = GameObject.Find("Singletons");

                if (go == null)
                {
                    go = new GameObject("Singletons");
                }

                _instance = go.GetComponent<T>();


                if (_instance == null)
                {
                    _instance = go.AddComponent<T>();
                }

            }
            return _instance;
        }
    }
}