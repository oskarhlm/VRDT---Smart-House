using System.Collections;
using UnityEngine;

namespace Utils
{
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static object mLock = new object();
        private static T mInstance;

        public static T Instance
        {
            get
            {
                lock (mLock)
                {
                    if (mInstance == null)
                    {
                        // try to find it
                        T[] instances = FindObjectsOfType(typeof(T)) as T[];

                        // didn't find S***
                        if (instances == null)
                        {
                            var instanceObj = new GameObject("(Singleton) " + typeof(T));
                            mInstance = instanceObj.AddComponent<T>();
                            DontDestroyOnLoad(instanceObj); // for preservation of this object through scenes
                            Debug.Log("[SINGLETON]: An instance of `" + typeof(T) + "` is needed." +
                                      " So gameObject `" + instanceObj.name + "` was created" +
                                      " with `" + typeof(T) + "` component attached to it" +
                                      " and with DontDestroyOnLoad called on it.");
                        }
                        else
                        {
                            // see if there's more than one, if so, do something about it
                            if (instances.Length > 1)
                            {
                                Debug.LogWarning("[SINGLETON]: There is more than one instance of `" +
                                                 typeof(T) +
                                                 "` in your scene. Destroying all, keeping only one...");

                                for (int i = 1, len = instances.Length; i < len; i++)
                                {
                                    Destroy(instances[i]);
                                }
                            }
                            else if (instances.Length == 1) Debug.Log("[SINGLETON]: Found only one instance of `" +
                                                                  typeof(T) +
                                                                  "` in `" + instances[0].gameObject.name +
                                                                  "` So singlation successful! :)");
                            mInstance = instances[0];
                        }
                    }
                    return mInstance;
                }
            }
        }
    }

}