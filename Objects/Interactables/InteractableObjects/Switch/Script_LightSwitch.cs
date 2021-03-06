﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Can either directly reference specific lights or reference
/// LightControllers
/// </summary>
public class Script_LightSwitch : Script_Switch
{
    public Light[] lights;
    [SerializeField] private Script_LightsController[] lightsControllers;
    
    public float onIntensity;
    public float offIntensity;
    public float volumeScale;
    public AudioSource audioSource;
    public AudioClip onOffSFX;

    protected override void Start()
    {
        base.Start();
        if (lightsControllers.Length > 0)
        {
            foreach (Script_LightsController lc in lightsControllers)   lc.ShouldUpdate = true;
            return;
        }
    }
    
    public override void TurnOn()
    {
        base.TurnOn();
        
        audioSource.PlayOneShot(onOffSFX, volumeScale);

        if (lightsControllers.Length > 0)
        {
            foreach (Script_LightsController lc in lightsControllers)   lc.Intensity = onIntensity;
            return;
        }
        
        foreach (Light l in lights)     l.intensity = onIntensity;
    }

    public override void TurnOff()
    {
        base.TurnOff();

        audioSource.PlayOneShot(onOffSFX, volumeScale);
        
        if (lightsControllers.Length > 0)
        {
            foreach (Script_LightsController lc in lightsControllers)   lc.Intensity = offIntensity;
            return;
        }
        
        foreach (Light l in lights)     l.intensity = offIntensity;
    }

    public override void SetupSwitch(
        bool _isOn,
        Sprite onSprite,
        Sprite offSprite
    )
    {
        base.SetupSwitch(_isOn, onSprite, offSprite);
        isSFXOverriden = true;
    }

    // for instantiation
    public override void SetupLights(
        Light[] _lights,
        float _onIntensity,
        float _offIntensity,
        bool isOn,
        Sprite onSprite,
        Sprite offSprite
    )
    {
        lights = _lights;
        onIntensity = _onIntensity;
        offIntensity = _offIntensity;
        SetupSwitch(isOn, onSprite, offSprite);

        foreach (Light l in lights)
        {
            l.intensity = isOn ? onIntensity : offIntensity;
        }
    }

    public void SetupSceneLights(
        bool isOn
    )
    {
        SetupSwitch(isOn, onSprite, offSprite);

        if (lightsControllers.Length > 0)
        {
            foreach (Script_LightsController lc in lightsControllers)
            {
                lc.Intensity = isOn ? onIntensity : offIntensity;
            }
            return;
        }
        
        foreach (Light l in lights)
        {
            l.intensity = isOn ? onIntensity : offIntensity;
        }
    }
}
