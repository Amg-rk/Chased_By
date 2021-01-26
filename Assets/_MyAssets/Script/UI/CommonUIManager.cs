using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CommonUIManager : MonoBehaviour
{
    const string sampleSceneName = "GameScene";
    const string stage1SceneName = "Stage5";

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

        SceneManager.LoadScene(stage1SceneName);

    }

    void SceneLoaded(Scene loadedScene, LoadSceneMode mode)
    {
        switch (loadedScene.name)
        {
            case sampleSceneName:
                GameSceneUI.gameObject.SetActive(true);
                break;
            case stage1SceneName:
                GameSceneUI.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }
}
