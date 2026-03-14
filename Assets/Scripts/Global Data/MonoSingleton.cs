using UnityEngine;

namespace Global_Data
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        private static bool _applicationIsQuitting = false;
        public static T Instance
        {
            get
            {
                if (_applicationIsQuitting)
                {
                    return null;
                }

                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<T>();

                    if (_instance == null)
                    {
                        Debug.LogWarning($"No instance of {typeof(T)} found in the scene!");
                    }
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                //DontDestroyOnLoad(gameObject); // Ensures singleton persists across scenes
            }
            else if (_instance != this)
            {
                Debug.LogError($"More than one instance of {typeof(T)} detected! Destroying duplicate.");
                Destroy(gameObject); // Destroy the entire GameObject, not just the component
            }
        }

        private void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
        }
    }
}
