using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWallControl : MonoBehaviour
{
    const float fireWallSpeed = 0.038f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        this.transform.Translate(0f, 0f, fireWallSpeed);
    }
}
