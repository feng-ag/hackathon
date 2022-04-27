using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerSingle : MonoBehaviour
{

  
    public new Camera camera;

    public static GameManagerSingle Instance { get; private set; }

  
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }   
  
}
