using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

public class OptionsTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LMotion.Create(transform.position.z, 10, 3).WithLoops(-1, LoopType.Yoyo).BindToPositionZ(transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
