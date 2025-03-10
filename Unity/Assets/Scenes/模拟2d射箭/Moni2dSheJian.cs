using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Moni2dSheJian : MonoBehaviour
{
    //射箭的角度
    [Range(0, 85)] public float angle = 10;

    //射箭的初速度
    [Range(1, 100)] public int orignSpeed = 10;

    public float massOfTheObject = 1;
    public Vector2 curMove;

    public float globalAcceleration = 9.81f;


    void Start()
    {
        
        curMove.y = orignSpeed * Mathf.Sin(angle * Mathf.PI / 180);

        curMove.x = orignSpeed * MathF.Cos(angle * Mathf.PI / 180);


        Debug.Log(curMove);
    }

    // Update is called once per frame
    void Update()
    {
        if ( transform.position.y < 0.4) return;
        curMove.y -= globalAcceleration * Time.deltaTime;

        transform.position += (Vector3)curMove;
    }
}