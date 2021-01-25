using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    Transform player;
    PlayerControl playerControl;

    [SerializeField] Transform playerLightPointerCanvas;

    [SerializeField] Transform uiCamera;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerControl = player.GetComponent<PlayerControl>();
    }

    void Update()
    {
        playerLightPointerCanvas.position = playerControl.PlayerLightPointerPos;
        playerLightPointerCanvas.rotation = playerControl.PlayerLightGun.rotation;

        if(playerControl.CameraState == CameraState.Ghost)
        {
            playerLightPointerCanvas.gameObject.SetActive(false);
        }
        else
        {
            playerLightPointerCanvas.gameObject.SetActive(true);
        }

        uiCamera.position = playerControl.PlayerCameraTrans.position;
        uiCamera.rotation = playerControl.PlayerCameraTrans.rotation;
    }
}
