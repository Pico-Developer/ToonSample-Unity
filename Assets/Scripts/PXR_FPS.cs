﻿using Unity.XR.PXR;
using UnityEngine;
using UnityEngine.UI;

public class PXR_FPS : MonoBehaviour
{
    private Text fpsText;

    private float updateInterval = 1.0f;
    private float timeLeft = 0.0f;
    private string strFps = null;

    void Awake()
    {
        fpsText = GetComponent<Text>();
    }

    void Update()
    {
        if (fpsText != null)
        {
            ShowFps();
        }
    }

    private void ShowFps()
    {
        timeLeft -= Time.unscaledDeltaTime;

        if (timeLeft <= 0.0)
        {
            float fps = PXR_Plugin.System.UPxr_GetConfigInt(ConfigType.RenderFPS);
                
            strFps = string.Format("FPS: {0:f0}", fps);
            fpsText.text = strFps;

            timeLeft += updateInterval;
        }
    }
}