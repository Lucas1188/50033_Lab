using UnityEngine;
/// <summary>
/// Unity friendly singleton generic class by destroying on load if more than one exists, 
/// but we shouldnt need to get there if the class was actually important enough to be a singleton -.-
/// I dont think this is thread safe yet...
/// </summary>
/// <typeparam name="T">Implementing class type</typeparam>
#nullable enable
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T? Instance { get; private set; }
    protected virtual void Awake()
    {
         if (Instance == null)
        {
            Instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}