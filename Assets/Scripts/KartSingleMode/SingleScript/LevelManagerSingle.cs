using System.Collections;
using System.Collections.Generic;
using Fusion;
using FusionExamples.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;
using ArcadeGalaxyKit;

public class LevelManagerSingle : MonoBehaviour
{

        public const int LAUNCH_SCENE = 0;
        public const int LOBBY_SCENE = 1;
        public const int CUBS_EDITOR_SCENE = 2;
        public const int MAP_EDITOR_SCENE = 3;
        public const int Game_Scene = 4;


    //[SerializeField] private UIScreen _dummyScreen;
    [SerializeField] private UIScreen _lobbyScreen;
        [SerializeField] private CanvasFader fader;

        public static LevelManagerSingle Instance => Singleton<LevelManagerSingle>.Instance;

    List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();


    private void Start()
    {
        StartCoroutine(LoadMultiScenes());
    }

    private IEnumerator LoadMultiScenes()
    {
        fader.gameObject.SetActive(true);
        
        scenesToLoad.Add(SceneManager.LoadSceneAsync(LevelManagerSingle.CUBS_EDITOR_SCENE));
        scenesToLoad.Add(SceneManager.LoadSceneAsync(LevelManagerSingle.MAP_EDITOR_SCENE, LoadSceneMode.Additive));
        scenesToLoad.Add(SceneManager.LoadSceneAsync(LevelManagerSingle.Game_Scene, LoadSceneMode.Additive));


        yield return new WaitForSeconds(2);

        PostLoadScene();

      
    }


    public static void ShowMapEditor()
        {
            MapEditorController.Instance.ShowMapEditor();
        }

        public static void HideMapEditor()
        {
            MapEditorController.Instance.HideMapEditor();
        }

        public static void ShowCubsEditor()
        {
            OutFitChangingSysten.instance.ShowCubsEditor();
        }

        public static void HideCubsEditor()
        {
            OutFitChangingSysten.instance.HideCubsEditor();
        }

    public static void EnterGamePlay()
    {
        var map = MapEditorController.Instance.CreacteMapObject();

        var trans = MapEditorController.Instance.GetStartingPoint(map);

        GameSceneManager.instance.gameRoot.SetActive(true);

        var cub = CubLoader.instance.LoadCub();

        Instantiate(cub, trans.position, Quaternion.identity);
    }

    //public static void LoadCub()
    //{
    //    GameObject cub = CubLoader.instance.LoadCub();
    //}

    //public static void ShowGameScene()
    //{
    //    Track.Instance.ShowGameScene();
    //}

    //public static void ShowGameScene()
    //{
    //    Track.Instance.HideGameScene();
    //}



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

    private void PostLoadScene()
    {
        fader.FadeOut();
    }

}
