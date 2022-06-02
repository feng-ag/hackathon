    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using ArcadeGalaxyKit;
    using KartGame.KartSystems;
    using MapEditor;

    public class LevelManager : MonoBehaviour
    {

        public const int LAUNCH_SCENE = 1;
        public const int CUBS_EDITOR_SCENE = 2;
        public const int MAP_EDITOR_SCENE = 3;
        public const int Game_Scene = 4;


        [SerializeField] private UIScreen _lobbyScreen;
        [SerializeField] private UIScreen _cubsEditorScreen;
        [SerializeField] private UIScreen _levelEditorScreen;
        [SerializeField] private CanvasFader fader;

         //List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();

         public static LevelManager Instance { get; private set; }




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


    private void Start()
    {
        StartCoroutine(LoadMultiScenes());

    }

    private IEnumerator LoadMultiScenes()
    {
        fader.FadeIn();


        AsyncOperation asyncUnload = SceneManager.LoadSceneAsync(LevelManager.CUBS_EDITOR_SCENE);

        while (!asyncUnload.isDone)
        {
            yield return null;
        }

        AsyncOperation asyncUnload2 = SceneManager.LoadSceneAsync(LevelManager.MAP_EDITOR_SCENE, LoadSceneMode.Additive);

        while (!asyncUnload2.isDone)
        {
            yield return null;
        }
        //scenesToLoad.Add(SceneManager.LoadSceneAsync(LevelManager.Game_Scene, LoadSceneMode.Additive));


        fader.FadeOut();

    }


    public void ShowMapEditor()
    {
          MapEditorController.Instance.ShowMapEditor();
    }


    public  void HideMapEditor()
    {
          MapEditorController.Instance.HideMapEditor();
    }

    public  void ShowCubsEditor()
    {
        OutFitChangingSysten.instance.ShowCubsEditor();

    }

    public  void HideCubsEditor()
    {
        OutFitChangingSysten.instance.HideCubsEditor();
    }




 

    public void QuitGameplay()
    {
        _cubsEditorScreen.gameObject.SetActive(true);

        StartCoroutine(QuitGamePlayCoroutine());

    }

    public IEnumerator QuitGamePlayCoroutine()
    {

        if (SceneManager.GetSceneByBuildIndex(LevelManager.Game_Scene).IsValid())
        {
            AsyncOperation asyncUnload =  SceneManager.UnloadSceneAsync(LevelManager.Game_Scene);

            while (!asyncUnload.isDone)
            {
                yield return null;
            }

        }

        ShowCubsEditor();
    }


    public void EnterGamePlay()
    {

        //if (SceneManager.GetSceneByBuildIndex(LevelManager.Game_Scene).IsValid())
        //{
        //    Debug.Log("unload scene");
        //    SceneManager.UnloadSceneAsync(LevelManager.Game_Scene);
        //}

        StartCoroutine(EnterGamePlayCoroutine());
    }


    public IEnumerator EnterGamePlayCoroutine()
    {


        fader.FadeIn();


        if (SceneManager.GetSceneByBuildIndex(LevelManager.Game_Scene).IsValid())
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(LevelManager.Game_Scene);

            while (!asyncUnload.isDone)
            {
                yield return null;
            }

        }


        AsyncOperation asyncLoad =  SceneManager.LoadSceneAsync(LevelManager.Game_Scene, LoadSceneMode.Additive);


        while (!asyncLoad.isDone)
        {
            yield return null;
        }


        Transform mapRoot = new GameObject("MapRoot").transform;
        mapRoot.SetParent(GameSceneManager.instance.gameRoot.transform);
        mapRoot.transform.position = Vector3.zero;


        yield return MapEditorController.Instance.CreateMapObject(mapRoot);
        var spawnPos = MapEditorController.Instance.GetStartingPoint();

        HideMapEditor();


        mapRoot.transform.localScale = new Vector3(5, 5, 5);

        var cub = CubLoader.instance.LoadCub();

        cub.transform.SetParent(GameSceneManager.instance.gameRoot.transform);
        cub.transform.position = new Vector3(spawnPos.position.x, 0.3f, (spawnPos.position.z - 0.5f));

        var arcadeKartComp = cub.GetComponentInChildren<ArcadeKart>();
        arcadeKartComp.KartAudio.gameObject.SetActive(true);


        GameSceneManager.instance.gameRoot.SetActive(true);



        fader.FadeOut();

    }




    //protected override IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished)
    //{
    //    Debug.Log($"Loading scene {newScene}");

    //    PreLoadScene(newScene);

    //    List<NetworkObject> sceneObjects = new List<NetworkObject>();

    //    if (newScene >= LOBBY_SCENE)
    //    {
    //        yield return SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Single);
    //        Scene loadedScene = SceneManager.GetSceneByBuildIndex(newScene);
    //        Debug.Log($"Loaded scene {newScene}: {loadedScene}");
    //        sceneObjects = FindNetworkObjects(loadedScene, disable: false);
    //    }

    //    finished(sceneObjects);

    //    // Delay one frame, so we're sure level objects has spawned locally
    //    yield return null;

    //    // Now we can safely spawn karts
    //    if (GameManager.CurrentTrack != null && newScene > LOBBY_SCENE)
    //    {
    //        if (Runner.GameMode == GameMode.Host)
    //        {
    //            foreach (var player in RoomPlayer.Players)
    //            {
    //                player.GameState = RoomPlayer.EGameState.GameCutscene;
    //                GameManager.CurrentTrack.SpawnPlayer(Runner, player);
    //            }
    //        }
    //    }

    //    PostLoadScene();
    //}

    //private void PreLoadScene(int scene)
    //{
    //    if (scene > LOBBY_SCENE)
    //    {
    //        // Show an empty dummy UI screen - this will stay on during the game so that the game has a place in the navigation stack. Without this, Back() will break
    //        Debug.Log("Showing Dummy");
    //        UIScreen.Focus(_dummyScreen);
    //    }
    //    else if (scene == LOBBY_SCENE)
    //    {
    //        foreach (RoomPlayer player in RoomPlayer.Players)
    //        {
    //            player.IsReady = false;
    //        }
    //        UIScreen.activeScreen.BackTo(_lobbyScreen);
    //    }
    //    else
    //    {
    //        UIScreen.BackToInitial();
    //    }
    //    fader.gameObject.SetActive(true);
    //    fader.FadeIn();
    //}

    //private void PostLoadScene()
    //{
    //    fader.FadeOut();
    //}

}

