using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(LevelManager))]
public class GameLauncher : MonoBehaviour
{
 
    //[SerializeField] private GameManagerSingle _gameManagerPrefab;
    //[SerializeField] private RoomPlayer _roomPlayerPrefab;


    //private LevelManagerSingle _levelManager;

    //List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();


    private void Start()
    {
        Application.runInBackground = true;
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
        QualitySettings.vSyncCount = 1;

        //_levelManager = GetComponent<LevelManagerSingle>();

        DontDestroyOnLoad(gameObject);

        //scenesToLoad.Add(SceneManager.LoadSceneAsync(LevelManager.LOBBY_SCENE));
        //scenesToLoad.Add(SceneManager.LoadSceneAsync(LevelManagerSingle.CUBS_EDITOR_SCENE));
        //scenesToLoad.Add(SceneManager.LoadSceneAsync(LevelManagerSingle.MAP_EDITOR_SCENE, LoadSceneMode.Additive));
    }

  
  
   
    //public void OnPlayerJoined(Transform playerContainer)
    //{  
    //        runner.Spawn(_gameManagerPrefab, Vector3.zero, Quaternion.identity);
    //        var roomPlayer = runner.Spawn(_roomPlayerPrefab, Vector3.zero, Quaternion.identity, playerContainer);   
    //}

    //public void OnPlayerLeft(Transform playerContainer)
    //{

    //    RoomPlayer.RemovePlayer(runner, playerContainer);

    //}

 
  

    
}

