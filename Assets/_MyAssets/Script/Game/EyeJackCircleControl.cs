using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeJackCircleControl : MonoBehaviour
{
    internal List<Transform> ghostList = new List<Transform>();
    internal List<Camera> ghostCameraList = new List<Camera>();

    void OnTriggerEnter(Collider other)
    {
        if (!other.transform.CompareTag("Ghost")) { return; }
        ghostList.Add(other.transform);
        ghostCameraList.Add(other.transform.GetComponentInChildren<Camera>());
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.transform.CompareTag("Ghost")) { return; }
        ghostList.Remove(other.transform);
    }

    void Update()
    {
        GhostListSort(ghostList, this.transform);
    }

    //バブルソートによりプレイヤーとの距離が近い順にゴーストリストを整列
    void GhostListSort(List<Transform> list, Transform player)
    {
        int n = list.Count;
        for (int i = 0; i < n -1; i++)
        {
            for (int j = n-1; j > i; j--)
            {
                float ghostDis1 = (list[j-1].position - player.position).sqrMagnitude;
                float ghostDis2 = (list[j].position - player.position).sqrMagnitude;

                if (ghostDis1 > ghostDis2)
                {
                    Transform trans = list[j];
                    list[j] = list[j - 1];
                    list[j - 1] = trans;
                }
            }
        }
    }
}
