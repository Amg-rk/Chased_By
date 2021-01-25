using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CommonUIManager : MonoBehaviour
{
    const string gameSceneName = "GameScene";

    [SerializeField] Transform GameSceneUI;
    [SerializeField] Transform ClearSceneUI;
    [SerializeField] Transform MenuSceneUI;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        SceneManager.sceneLoaded += SceneLoaded;

        SceneManager.LoadScene(gameSceneName);

    }

    void SceneLoaded(Scene loadedScene, LoadSceneMode mode)
    {
        switch (loadedScene.name)
        {
            case gameSceneName:
                GameSceneUI.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }
}
