﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cinemachine;

#if UNITY_EDITOR
using UnityEditor;
using Unity.EditorCoroutines.Editor;
#endif

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class Script_CameraShake : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin noise;

    public void Shake(float duration, float amp, float freq, Action cb)
    {
        StartCoroutine(ShakeCoroutine(duration, amp, freq, cb));
    }
    
    private IEnumerator ShakeCoroutine(float duration, float amp, float freq, Action cb)
    {
        float timer = duration;
        CinemachineBasicMultiChannelPerlin noise = virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        
        // Set Cinemachine Camera Noise parameters
        noise.m_AmplitudeGain = amp;
        noise.m_FrequencyGain = freq;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        timer = 0;
        noise.m_AmplitudeGain = 0f;
        noise.m_FrequencyGain = 0f;
        if (cb != null) cb();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Script_CameraShake))]
public class Script_CameraEffectsTester : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        Script_CameraShake camera = (Script_CameraShake)target;
        if (GUILayout.Button("Shake Camera"))
        {
            camera.Shake(0.5f, 1f, 1f, null);
        }
    }
}
#endif