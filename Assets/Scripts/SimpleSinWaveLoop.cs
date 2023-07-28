using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleSinWaveLoop : MonoBehaviour
{
    public float amplitude = 1f; // 振幅
    public float period = 1f; // 周期
    public float speed = 1f; // 速度

    private float oriY = 0;

    private float timeElapsed = 0f; // 已经过去的时间

    private void Start()
    {
        oriY = transform.position.y;
        timeElapsed = period * Random.Range(0f, 1f);
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime * speed; // 更新已经过去的时间

        float newYPos = oriY+Mathf.Sin(timeElapsed / period) * amplitude; // 计算新的y轴位置

        transform.position = new Vector3(transform.position.x, newYPos, transform.position.z); // 更新物体的位置
    }
}
