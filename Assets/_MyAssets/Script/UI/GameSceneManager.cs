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

    [SerializeField] Transform[] lifeImageTrans;

    [SerializeField] Transform ghostEyeImageTrans;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerControl = player.GetComponent<PlayerControl>();
    }

    void Update()
    {
        PlayerLightPointerCanvasSet();

        UiCameraTransSet();

        PlayerLifeDisplay(playerControl.PlayerLife);

        GhostEyeDisplay();
    }

    void UiCameraTransSet(){
        uiCamera.position = playerControl.PlayerCameraTrans.position;
        uiCamera.rotation = playerControl.PlayerCameraTrans.rotation;
    }

    void PlayerLightPointerCanvasSet(){
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
    }

    void PlayerLifeDisplay(int playerLifeNumber){
        foreach(var t in lifeImageTrans){
            t.gameObject.SetActive(false);
        }

        for(int i = 0; i < playerLifeNumber; i++){
            lifeImageTrans[i].gameObject.SetActive(true);
        }
    }

    void GhostEyeDisplay()
    {
        if(playerControl.EyeJackCircleControl.ghostList.Count > 0)
        {
            ghostEyeImageTrans.gameObject.SetActive(true);
        }
        else
        {
            ghostEyeImageTrans.gameObject.SetActive(false);
        }
    }
}
