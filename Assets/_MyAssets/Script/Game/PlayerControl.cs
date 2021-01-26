using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraState
{
    Player,
    Ghost
}

public enum PlayerState
{
    Grounded,
    Jump
}

public enum PlayerAdditionalState
{
    Nomal,
    Slowed
}

public enum PlayerLightState
{
    Ready,
    Preparation,
    Lighting
}

public class PlayerControl : MonoBehaviour
{
    CameraState cameraState;

    public CameraState CameraState
    {
        get { return cameraState; }
        private set { cameraState = CameraState.Player; }
    }

    PlayerState playerState;
    PlayerLightState playerLightState;
    PlayerAdditionalState playerAdditionalState;

    float playerRunSpeed = 0.1f;
    const float playerDefaultRunSpeed = 0.1f;
    const float playerSlowedSpeed = 0.05f;

    float playerRotateSpeed = 4f;
    const float playerDefaultRotateSpeed = 4f;
    const float playerSlowedRotateSpeed = 2f;

    const float playerJumpPower = 5f;

    float playerCameraAngleX;
    const float playerDefaultCameraAngleX = 40f;

    const int fps = 60;

    const float lightDistance = 10f;

    const int lightSurvivalFlame = 20;
    int lightSurvival = 0;


    int lightCoolingFlame = 0;
    const int lightCooledFlame = 10; 

    const int groundLayer = 9;

    Camera playerCamera = null;
    public Camera ghostCamera = null;
    AudioListener playerAudioListener = null;
    AudioListener ghostAudioListener = null;

    Ray lightRay;
    RaycastHit lightHit;

    Rigidbody playerRigidBody;

    //int lightStock;
    //const int firstLightStock = 5;

    GameObject attackingGhost = null;

    [SerializeField] Transform playerLightGun;

    public Transform PlayerLightGun
    {
        get { return playerLightGun; }
        private set { playerLightGun = null; }
    }

    [SerializeField] Transform playerLight;

    [SerializeField] Transform eyeJackCircle;

    [SerializeField] Transform playerCameraTrans;

    public Transform PlayerCameraTrans
    {
        get { return playerCameraTrans; }
        private set { playerCameraTrans = null; }
    }

    Vector3 playerLightPointerPos;


    public Vector3 PlayerLightPointerPos
    {
        get { return playerLightPointerPos; }
        private set { playerLightPointerPos = Vector3.zero; }
    }

    EyeJackCircleControl eyeJackCircleControl;

    public EyeJackCircleControl EyeJackCircleControl{
        get { return eyeJackCircleControl; }
        private set { eyeJackCircleControl = null; }
    }

    public Transform Player { get; private set; }

    public int PlayerLife { get; private set; }

    void Awake()
    {
        playerCamera = playerCameraTrans.GetComponent<Camera>();
        playerAudioListener = playerCameraTrans.GetComponent<AudioListener>();
        Player = this.transform;
        playerRigidBody = GetComponent<Rigidbody>();
        eyeJackCircleControl = eyeJackCircle.GetComponent<EyeJackCircleControl>();
    }

    void Start()
    {
        cameraState = CameraState.Player;
        playerState = PlayerState.Grounded;
        playerAdditionalState = PlayerAdditionalState.Nomal;
        playerLightState = PlayerLightState.Ready;

        playerCameraAngleX = playerCamera.transform.rotation.eulerAngles.x;

        //lightStock = firstLightStock;
        PlayerLife = 3;

        playerLight.gameObject.SetActive(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Kill"))
        {
            PlayerLife = 0;
        }

        if (!collision.transform.CompareTag("Ghost") || !ReferenceEquals(attackingGhost, null)) { return; }
        collision.gameObject.GetComponent<GhostControl>().GhostDisappear();
        PlayerLife--;
    }

    void OnCollisionStay(Collision collision){
        if (collision.gameObject.layer == groundLayer)
        {
            playerState = PlayerState.Grounded;
        }

        if (collision.transform.CompareTag("Slow")){
            playerAdditionalState = PlayerAdditionalState.Slowed; 
            playerRunSpeed = playerSlowedSpeed;
            playerRotateSpeed = playerSlowedRotateSpeed;
        }
        else
        {
            playerAdditionalState = PlayerAdditionalState.Nomal;
            playerRunSpeed = playerDefaultRunSpeed;
            playerRotateSpeed = playerDefaultRotateSpeed;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == groundLayer)
        {
            playerState = PlayerState.Jump;
        }

        if (!collision.transform.CompareTag("Ghost")) { return; }

        if(attackingGhost == collision.gameObject) { attackingGhost = null; }
    }

    void FixedUpdate()
    {
        CameraStateManage();
        LightPoint();

        if (playerLightState == PlayerLightState.Lighting)
        {
            lightSurvival++;
        }
        else if (playerLightState == PlayerLightState.Preparation)
        {
            lightCoolingFlame++;
        }

        if (lightCoolingFlame >= lightCooledFlame && playerLightState == PlayerLightState.Preparation)
        {
            playerLightState = PlayerLightState.Ready;
        }
        else if(lightSurvival >= lightSurvivalFlame && playerLightState == PlayerLightState.Lighting)
        {
            playerLightState = PlayerLightState.Preparation;
            lightCoolingFlame++;

            playerLight.gameObject.SetActive(false);
        }

        if (cameraState == CameraState.Player)
        {
            PlayerMove();
        }
    }

    void PlayerMove()
    {
        Vector3 moveVec = Vector3.zero;
        Vector3 rotateVec = Vector3.zero;
        //移動
        if (Input.GetKey(KeyCode.W))
        {
            moveVec = new Vector3(0f, 0f, playerRunSpeed);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveVec = new Vector3(0f, 0f, -1 * playerRunSpeed);
        }

        Player.Translate(moveVec);
        //y軸上の回転
        if (Input.GetKey(KeyCode.A))
        {
            rotateVec = new Vector3(0f, -1 * playerRotateSpeed, 0f);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rotateVec = new Vector3(0f, playerRotateSpeed, 0f);
        }

        Player.Rotate(rotateVec);

        //x軸上のカメラ移動
        if (Input.GetKey(KeyCode.R))
        {
            playerCameraAngleX -= 0.5f;
        }
        else
        {
            playerCameraAngleX = playerDefaultCameraAngleX;
        }

        //カメラのx角度はplayerDefaultCameraAngleXから20まで
        playerCameraAngleX = Mathf.Clamp(playerCameraAngleX, 20f, playerDefaultCameraAngleX);

        playerCameraTrans.transform.rotation = Quaternion.Euler(
            new Vector3(
                playerCameraAngleX,
                playerCameraTrans.rotation.eulerAngles.y,
                playerCameraTrans.rotation.eulerAngles.z ));

        //フラッシュ(攻撃)
        if (Input.GetKeyDown(KeyCode.E))
        {
            FlashLight();
        }

        //ジャンプ(接地していなければ無効)

        if (playerState != PlayerState.Grounded) { return; }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 jumpForce = new Vector3(0f, playerJumpPower, 0f);
            playerRigidBody.AddForce(jumpForce, ForceMode.Impulse);
        }
    }

    public void CameraStateManage()
    {
        bool isGhostCameraUsed = false;

        ghostCamera = null;
        ghostAudioListener = null;

        if (Input.GetKey(KeyCode.Q)){ isGhostCameraUsed = true; }

        GetGhostCameraComponent();

        GhostCameraUse(isGhostCameraUsed);
    }

    void GetGhostCameraComponent()
    {
        if (eyeJackCircleControl.ghostList.Count <= 0) { return; }

        ghostCamera = eyeJackCircleControl.ghostList[0].GetComponentInChildren<Camera>();
        ghostAudioListener = eyeJackCircleControl.ghostList[0].GetComponentInChildren<AudioListener>();
    }

    void GhostCameraUse(bool isGhostCameraUsed)
    {
        if (isGhostCameraUsed)
        {
            if (ReferenceEquals(ghostCamera, null) || ReferenceEquals(ghostAudioListener, null)) {

                playerCamera.enabled = true;
                playerAudioListener.enabled = true;

                return;
            }

            cameraState = CameraState.Ghost;

            playerCamera.enabled = false;
            playerAudioListener.enabled = false;

            ghostCamera.enabled = true;
            ghostAudioListener.enabled = true;

            return;
        }
        cameraState = CameraState.Player;

        playerCamera.enabled = true;
        playerAudioListener.enabled = true;

        if (ReferenceEquals(ghostCamera, null) || ReferenceEquals(ghostAudioListener, null)){ return; }
        ghostCamera.enabled = false;
        ghostAudioListener.enabled = false;
    }

    void LightPoint()
    {
        Ray lightPointerRay = new Ray(playerLightGun.position, playerLightGun.forward);
        RaycastHit lightPointerHit;

        playerLightPointerPos = lightPointerRay.origin + lightPointerRay.direction * lightDistance;


        if (!Physics.Raycast(lightPointerRay, out lightPointerHit, lightDistance)) { return; }

        playerLightPointerPos = lightPointerHit.point;
    }

    void FlashLight()
    {
        if(playerLightState != PlayerLightState.Ready) { return; }

        int layerMask = 1 << 8;
        lightRay = new Ray(playerLightGun.position, playerLightGun.forward);

        //Debug.DrawRay(lightRay.origin, lightRay.direction * lightDistance, Color.red, 3, false);

        lightCoolingFlame = 0;
        lightSurvival = 0;

        playerLightState = PlayerLightState.Lighting;

        playerLight.gameObject.SetActive(true);

        if (!Physics.Raycast(lightRay, out lightHit, lightDistance, layerMask)){ return; }

        if (lightHit.collider.CompareTag("Ghost"))
        {
            lightHit.collider.transform.GetComponent<GhostControl>().GhostDeath();
        }
    }
}
