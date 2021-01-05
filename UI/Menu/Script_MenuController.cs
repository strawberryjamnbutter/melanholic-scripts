﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Handles overall menu state based on TopBar and settings
/// 
/// TBD DELETE ButtonMetadata & UIIds, not being used anymore
/// </summary>
public class Script_MenuController : Script_UIState
{
    
    public TopBarStates topBarState;
    public enum TopBarStates{
        inventory,
        notes,
        entries,
        thoughts,
        memories
    }
    public string inventoryState;
    public bool isSBookDisabled;
    public GameObject SBookOverviewButton;
    public GameObject initialStateSelected;
    public Script_SBookOverviewController SBookController;
    [SerializeField] private Script_NotesController notesController;
    [SerializeField] private Script_EntriesController entriesController;
    public Script_CanvasGroupController_Thoughts thoughtsController;
    [SerializeField] private Script_MemoriesController memoriesController;
    public Script_InventoryViewSettings inventorySettings;
    public Script_EntriesViewSettings entriesSettings;
    public Script_InventoryOverviewSettings inventoryOverviewSettings;
    
    [SerializeField] private Script_MenuInputManager inputManager;
    [SerializeField] private Script_InventoryManager inventoryManager;
    [SerializeField] private Script_EntriesViewController entriesViewController;
    [SerializeField] private Button entriesTopBarButton;
    private Selectable entriesTopBarSelectOnDownButton;
    [SerializeField] private Script_MemoriesViewController memoriesViewController;
    [SerializeField] private Button memoriesTopBarButton;
    private Selectable memoriesTopBarSelectOnDownButton;

    void OnEnable()
    {
        ChangeTopBarState(topBarState);
    }
    
    void Update()
    {        
        if (string.IsNullOrEmpty(inventoryState))
        {
            InitializeState();
        }
        else if (inventoryState == Const_States_InventoryOverview.Overview)
        {
            inputManager.HandleExitInput();
        }

        HandleNullViewStates();
    }

    public void ChangeStateToOverview()
    {
        inventoryState = Const_States_InventoryOverview.Overview;
    }

    public void ChangeStateToInventoryView()
    {
        Debug.Log($"MenuController inventoryState before stickers view: {inventoryState}");
        inventoryState = Const_States_InventoryOverview.InventoryView;
    }

    public void ChangeStateToEquipmentView()
    {
        inventoryState = Const_States_InventoryOverview.EquipmentView;
    }

    public void ChangeStateToEntriesView()
    {
        inventoryState = Const_States_InventoryOverview.EntriesView;
    }

    public void ChangeTopBarState(TopBarStates state)
    {
        switch(state)
        {
            case TopBarStates.inventory:
                topBarState = TopBarStates.inventory;

                SBookController.Open();
                thoughtsController.Close();
                entriesController.Close();
                memoriesController.Close();
                notesController.Close();

                break;

            case TopBarStates.notes:
                topBarState = TopBarStates.notes;

                SBookController.Close();
                notesController.Open();
                thoughtsController.Close();
                entriesController.Close();
                memoriesController.Close();

                break;            

            case TopBarStates.entries:
                topBarState = TopBarStates.entries;

                SBookController.Close();
                notesController.Close();
                entriesController.Open();
                thoughtsController.Close();
                memoriesController.Close();

                break;

            case TopBarStates.thoughts:
                topBarState = TopBarStates.thoughts;

                SBookController.Close();
                notesController.Close();
                entriesController.Close();
                thoughtsController.Open();
                memoriesController.Close();

                break;

            case TopBarStates.memories:
                topBarState = TopBarStates.memories;

                SBookController.Close();
                notesController.Close();
                entriesController.Close();
                thoughtsController.Close();
                memoriesController.Open();

                break;
                
            default:
                break;
        }
    }

    // allow menu to be "sticker" for non multiple UI
    public void ChangeRepeatDelay(float t, int a)
    {
        EventSystem.current.GetComponent<StandaloneInputModule>().repeatDelay = t;
        EventSystem.current.GetComponent<StandaloneInputModule>().inputActionsPerSecond = a;
    }

    public void EnableSBook(bool isActive)
    {
        SBookOverviewButton.SetActive(isActive);
    }

    public Script_Item[] GetInventoryItems()
    {
        return inventoryManager.GetInventoryItems();
    }
    // public Script_Inventory GetInventory()
    // {
    //     return inventoryManager.GetInventory();
    // }

    public Script_Sticker[] GetEquipmentItems()
    {
        return inventoryManager.GetEquipmentItems();
    }

    public void HighlightItem(int id, bool isOn, bool showDesc)
    {
        inventoryManager.HighlightItem(id, isOn, showDesc);
    }

    public bool AddItem(Script_Item item)
    {
        return inventoryManager.AddItem(item);
    }

    public bool AddItemInSlotById(string itemId, int i)
    {
        return inventoryManager.AddItemInSlotById(itemId, i);
    }

    public bool AddItemById(string itemId)
    {
        return inventoryManager.AddItemById(itemId);
    }

    public bool AddEquippedItemInSlotById(string equipmentId, int i)
    {
        return inventoryManager.AddEquippedItemInSlotById(equipmentId, i);
    }

    public bool CheckStickerEquipped(Script_Sticker sticker)
    {
        return inventoryManager.CheckStickerEquipped(sticker);
    }

    public bool CheckStickerEquippedById(string stickerId)
    {
        return inventoryManager.CheckStickerEquippedById(stickerId);
    }

    public Script_ItemObject InstantiateDropById(string itemId, Vector3 location, int LB)
    {
        return inventoryManager.InstantiateDropById(itemId, location, LB);
    }

    public void HandleNullViewStates()
    {
        EntriesTopBarState();
        MemoriesTopBarState();

        void EntriesTopBarState()
        {
            Navigation entriesNav = entriesTopBarButton.GetComponent<Selectable>().navigation;    
            entriesNav.selectOnDown = entriesViewController.slots.Length == 0 ? null : entriesTopBarSelectOnDownButton;
            entriesTopBarButton.GetComponent<Selectable>().navigation = entriesNav;
        }

        void MemoriesTopBarState()
        {
            Navigation memoriesNav = memoriesTopBarButton.GetComponent<Selectable>().navigation;    
            memoriesNav.selectOnDown = memoriesViewController.slots.Length == 0 ? null : memoriesTopBarSelectOnDownButton;
            memoriesTopBarButton.GetComponent<Selectable>().navigation = memoriesNav;
        }
    }

    private void CacheTopBarNav()
    {
        CacheEntriesTopBarNav();
        CacheMemoriesTopBarNav();
        
        void CacheEntriesTopBarNav()
        {
            Navigation entriesNav = entriesTopBarButton.GetComponent<Selectable>().navigation;    
            entriesTopBarSelectOnDownButton = entriesNav.selectOnDown;
        }
        
        void CacheMemoriesTopBarNav()
        {
            Navigation memoriesNav = memoriesTopBarButton.GetComponent<Selectable>().navigation;    
            memoriesTopBarSelectOnDownButton = memoriesNav.selectOnDown;
        }
    }
    

    public void InitializeState(EventSystem eventSystem = null)
    {
        if (eventSystem != null)
        {
            eventSystem.SetSelectedGameObject(initialStateSelected);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(initialStateSelected);
        }

        inventoryState = Const_States_InventoryOverview.Overview;
    }

    public void Setup()
    {
        CacheTopBarNav();
        
        inputManager = GetComponent<Script_MenuInputManager>();

        inputManager.Setup();
        thoughtsController.Setup();
        SBookController.Setup();
        entriesController.Setup();
        memoriesController.Setup();
        inventoryManager.Setup();

        isSBookDisabled = true;

        if (Debug.isDebugBuild && Const_Dev.IsDevMode)
        {
            Debug.Log("<b>SBook</b> enabled by default for debugging.");
            isSBookDisabled = false;
        }
    }
}
