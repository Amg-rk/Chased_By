﻿using System.Collections;
using System;
using UnityEngine;

enum GhostState{
    Nomal,
    Attack,
    Dead
}

public class GhostControl : MonoBehaviour
{
    const float ghostRunSpeed = 0.02f;
    const float ghostAttackSpeed = 0.05f;
    const float ghostAttackDistance = 20f;

    const int invisibleLayer = 8;
    const int visibleLayer = 0;

    byte ghostMeshAlpha;

    MeshRenderer[] ghostMeshRenderer;

    EyeJackCircleControl eyeJackCircleControl;

    GhostState ghostState;

    [SerializeField] Transform thisBody;

    Transform player = null;      

    void Awake()
    {
        ghostMeshRenderer = GetComponentsInChildren<MeshRenderer>();
        ghostState = GhostState.Nomal;

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Start()
    {
        eyeJackCircleControl = player.GetComponent<PlayerControl>().EyeJackCircleControl;

        ghostMeshAlpha = 255;

        foreach (var m in ghostMeshRenderer)
        {
            m.material.color = new Color32(255, 0, 0, ghostMeshAlpha);
        }
    }

    void FixedUpdate()
    {
        GhostMove();

        if((player.position - this.transform.position).sqrMagnitude < ghostAttackDistance)
        {
            GhostAttacking();
        }
        else
        {
            ghostState = GhostState.Nomal;
            LayerChange(this.transform, invisibleLayer);
        }
    }

    public void GhostDeath()
    {
        LayerChange(this.transform, visibleLayer);
        ghostState = GhostState.Dead;
        GhostDisappear();
    }

    void GhostAttacking()
    {
        LayerChange(this.transform, visibleLayer);
        ghostState = GhostState.Attack;
    }

    public void GhostDisappear()
    {
        PlayerControl playerControl = player.GetComponent<PlayerControl>();

        if (eyeJackCircleControl.ghostList.Contains(this.transform))
        {
            eyeJackCircleControl.ghostList.Remove(this.transform);
            playerControl.CameraStateManage();
        }
        this.gameObject.SetActive(false);
    }

    void LayerChange(Transform parentTrans, int layerNumber)
    {
        foreach (Transform trans in parentTrans)
        {
            trans.gameObject.layer = layerNumber;
        }
        thisBody.gameObject.layer = layerNumber;
    }

    void GhostMove()
    {
        Vector3 moveVec = Vector3.zero;

        if (ghostState == GhostState.Nomal)
        {
            moveVec = new Vector3(0f, 0f, ghostRunSpeed);
        }
        else if(ghostState == GhostState.Attack)
        {
            moveVec = new Vector3(0f, 0f, ghostAttackSpeed);
        }

        GhostRotate(player, this.transform);

        this.transform.Translate(moveVec);
    }

    void GhostRotate(Transform player, Transform ghost)
    {
        float ghostToPlayerVecZ = ghost.position.z - player.position.z;
        float ghostToPlayerVecX = ghost.position.x - player.position.x;

        float distance = Mathf.Pow(
            (ghostToPlayerVecX * ghostToPlayerVecX) + (ghostToPlayerVecZ * ghostToPlayerVecZ)
            ,0.5f);
        float sinY = ghostToPlayerVecZ / distance;//xz平面上におけるsin

        float angleY = 180 * Mathf.Acos(sinY) / Mathf.PI;//ラジアンを角度に変換

        if (ghostToPlayerVecX > 0)
        {
            if (ghostToPlayerVecZ > 0)
            {
                angleY += 180f;
            }
            else
            {
                angleY -= 180f;
            }
        }
        else
        {
            angleY = 180f - angleY;
        }

        ghost.rotation = Quaternion.Euler(new Vector3(ghost.rotation.x, angleY, ghost.rotation.z));
    }
}