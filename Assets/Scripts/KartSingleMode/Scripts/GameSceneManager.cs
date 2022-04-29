using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{

    [SerializeField]
    public GameObject gameRoot;

    public static GameSceneManager instance { get { return _instance; } }
    private static GameSceneManager _instance;

    private void Awake()
    {
        if (instance == null)
        {
            _instance = this;
        }
    }



    //public void ShowGameScene()
    //{
    //    GameSceneManager.Instance.gameRoot.SetActive(true);
    //}

    //public void HideGameScene()
    //{
    //    GameSceneManager.Instance.gameRoot.SetActive(false);

    //}

}
