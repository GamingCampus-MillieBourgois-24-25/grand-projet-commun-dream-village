using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupTurnCoffee : MonoBehaviour
{
    static float rotation = 90f;
    static float rotationSpeed = 30f;

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, rotation, 0);
    }

    static public void RotateCup()
    {
        if (rotation < 360f)
        {
            rotation += rotationSpeed * Time.deltaTime;
        }
        else
        {
            rotation = 0f;
        }
    }
}
