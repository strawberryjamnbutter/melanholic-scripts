﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Audio;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(Script_LBSwitchHandler))]
[RequireComponent(typeof(Script_TimelineController))]
[RequireComponent(typeof(AudioSource))]
public class Script_LevelBehavior_26 : Script_LevelBehavior
{
    /* =======================================================================
        STATE DATA
    ======================================================================= */
    public bool[] switchesState;
    public bool isPuzzleComplete;
    public bool didActivateDramaticThoughts;
    public bool didPickUpWinterStone;
    /* ======================================================================= */
    
    [SerializeField] private Transform switchParent;
    [SerializeField] private Script_UrselkAttacks urselkAttacks;
    [SerializeField] private float attackInterval;
    [SerializeField] private float timer;
    [SerializeField] private Script_Switch puzzleSwitch;
    [SerializeField] private PlayableDirector spikeCageDirector;
    [SerializeField] private PlayableDirector dramaticThoughtsDirector;
    [SerializeField] Script_TriggerEnterOnce dramaticThoughtsCutSceneTrigger;
    [SerializeField] Script_CollectibleObject winterStone;
    [SerializeField] Transform spikeCage;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private FadeSpeeds musicFadeOutSpeed;
    [SerializeField] private Script_BgThemePlayer bgThemePlayer;
    [SerializeField] private Transform textParent;

    private Script_LBSwitchHandler switchHandler;
    private bool isPauseSpikes;
    private bool isInitialize = true;
    
    protected override void OnEnable()
    {
        Script_InteractableObjectEventsManager.OnSwitchOff += OnSwitchOff;
        spikeCageDirector.stopped += OnSpikeCageDownDone;
        dramaticThoughtsDirector.stopped += OnDramaticThoughtsDone;
        Script_ItemsEventsManager.OnItemPickUp += OnItemPickUp;
    }

    protected override void OnDisable()
    {
        Script_InteractableObjectEventsManager.OnSwitchOff -= OnSwitchOff;
        spikeCageDirector.stopped -= OnSpikeCageDownDone;
        dramaticThoughtsDirector.stopped -= OnDramaticThoughtsDone;
        Script_ItemsEventsManager.OnItemPickUp -= OnItemPickUp;

        bgThemePlayer.gameObject.SetActive(false);

        DefaultBgMusicLevels();
    }

    protected override void Update()
    {
        AttackTimer();
        HandleDramaticThoughtsCutScene();
    }

    private void OnItemPickUp(string itemId)
    {
        if (itemId == winterStone.Item.id)
        {
            didPickUpWinterStone = true;
        }
    }

    private void AttackTimer()
    {
        if (timer == 0) timer = attackInterval;

        timer -= Time.deltaTime;

        if (timer <= 0 && switchesState[0])
        {
            if (!isPauseSpikes)     Attack();
            timer = 0;
        }
    }
    
    public void Attack()
    {
        urselkAttacks.AlternatingSpikesAttack();
    }

    public override bool ActivateTrigger(string Id)
    {
        if (Id == "dramatic-thoughts")
        {
            game.ChangeStateCutScene();
            isPauseSpikes = true;
            return true;
        }
        else if (Id == "drama-done")
        {
            if (!isPuzzleComplete)
                FadeOutDramaticMusic();
        }

        return false;

        void FadeOutDramaticMusic()
        {
            StartCoroutine(
                Script_AudioMixerFader.Fade(
                    audioMixer,
                    Const_AudioMixerParams.ExposedBGVolume,
                    Script_AudioEffectsManager.GetFadeTime(musicFadeOutSpeed),
                    0f,
                    () => bgThemePlayer.gameObject.SetActive(false)
                )
            );
        }
    }

    private void HandleDramaticThoughtsCutScene()
    {
        if (isPauseSpikes && timer == 0 && !didActivateDramaticThoughts)
        {
            didActivateDramaticThoughts = true;
            GetComponent<Script_TimelineController>().PlayableDirectorPlayFromTimelines(1, 1);
            
            StartCoroutine(
                Script_AudioMixerFader.Fade(
                    audioMixer,
                    Const_AudioMixerParams.ExposedBGVolume,
                    Script_AudioEffectsManager.GetFadeTime(musicFadeOutSpeed),
                    0f,
                    () => game.StopBgMusic()
                )
            );
        }
    }

    private void OnDramaticThoughtsDone(PlayableDirector aDirector)
    {
        GetComponent<AudioSource>().PlayOneShot(Script_SFXManager.SFX.ThoughtsDone, Script_SFXManager.SFX.ThoughtsDoneVol);
        
        game.ChangeStateInteract();
        bgThemePlayer.gameObject.SetActive(true);
        Script_AudioMixerVolume.SetVolume(
            audioMixer, Const_AudioMixerParams.ExposedBGVolume, 1f
        );
        isPauseSpikes = false;
        timer = 0.001f; // make Attack instantly after
    }

    private void OnSwitchOff(string switchId)
    {
        if (switchId == puzzleSwitch.nameId)
        {
            game.ChangeStateCutScene();
            StartCoroutine(WaitSpikeCageDown());
        }

        IEnumerator WaitSpikeCageDown()
        {
            yield return new WaitForSeconds(attackInterval);
            
            GetComponent<Script_TimelineController>().PlayableDirectorPlayFromTimelines(0, 0);
            isPuzzleComplete = true;
        }
    }

    private void OnSpikeCageDownDone(PlayableDirector aDirector)
    {
        game.ChangeStateInteract();
    }

    public override void SetSwitchState(int Id, bool isOn)
    {
        print("override SetSwitchState()");
        switchHandler.SetSwitchState(switchesState, Id, isOn);
    }

    private void DefaultBgMusicLevels()
    {
        Script_AudioMixerVolume.SetVolume(
            audioMixer,
            Const_AudioMixerParams.ExposedBGVolume,
            1f
        );   
    }

    private void Awake()
    {
        switchHandler = GetComponent<Script_LBSwitchHandler>();
        switchHandler.Setup(game);

        if (didActivateDramaticThoughts)
        {
            dramaticThoughtsCutSceneTrigger.gameObject.SetActive(false);
        }
        else
        {
            dramaticThoughtsCutSceneTrigger.gameObject.SetActive(true);
        }

        if (isPuzzleComplete)       spikeCage.gameObject.SetActive(false);
        else                        spikeCage.gameObject.SetActive(true);

        if (didPickUpWinterStone)   winterStone.gameObject.SetActive(false);
        else                        winterStone.gameObject.SetActive(true);
    }
    
    public override void Setup()
    {
        game.SetupInteractableObjectsText(textParent, isInitialize);
        switchesState = switchHandler.SetupSwitchesState(
            switchParent,
            switchesState,
            isInitialize
        );
        
        timer = attackInterval;
        
        // If player activated the dramatic thoughts, and came back later
        // start off with dramatic music
        if (didActivateDramaticThoughts && !isPuzzleComplete)
        {
            bgThemePlayer.gameObject.SetActive(true);
            game.PauseBgMusic();
        }
        // if puzzle if done just use default music
        else
        {
            bgThemePlayer.gameObject.SetActive(false);
        }

        isInitialize = false;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Script_LevelBehavior_26))]
public class Script_LevelBehavior_26Tester : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        Script_LevelBehavior_26 lb = (Script_LevelBehavior_26)target;
        if (GUILayout.Button("SetNewElleniaPassword()"))
        {

        }
    }
}
#endif