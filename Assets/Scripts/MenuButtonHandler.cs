using System;
using UnityEngine;

public enum PlayState
{
    NONE,
    PLACE_FURNITURE,
    RELOCATE_FURNITURE,
    DELETE_FURNITURE
}

public class MenuButtonHandler : Singleton<MenuButtonHandler>
{
    [System.Serializable]
    public struct PlayStateButton
    {
        public MenuButton Button;
        public PlayState PlayState;
    }

    [SerializeField] private PlayStateButton[] m_PlayStateButtons;
    [SerializeField] private FurnitureDropdown m_FurnitureDropdown;

    [SerializeField] private FurniturePlacer m_FurniturePlacer;
    [SerializeField] private FurnitureRelocator m_FurnitureRelocator;
    [SerializeField] private FurnitureDeleter m_FurnitureDeleter;

    public PlayState CurrPlayState {get; private set;} = PlayState.NONE;
    public FurnitureInfo CurrFurnitureInfo => m_FurnitureDropdown.CurrSelectedFurnitureInfo;
    public event Action OnFurnitureSelectedEvent;
    private MenuButton m_CurrSelectedButton = null;

    public bool HadButtonPressThisFrame {get; private set;}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        foreach (PlayStateButton playStateButton in m_PlayStateButtons)
        {
            playStateButton.Button.OnPressedAction += () => OnPlayStateButtonPressed(playStateButton);
        }

        m_FurnitureDropdown.OnSwitchFurnitureEvent += OnFurnitureSelectedEvent;

        m_FurniturePlacer.enabled = false;
        m_FurnitureRelocator.enabled = false;
        m_FurnitureDeleter.enabled = false;
    }

    private void OnDestroy()
    {
        foreach (PlayStateButton playStateButton in m_PlayStateButtons)
        {
            playStateButton.Button.ClearEvents();
        }

        m_FurnitureDropdown.OnSwitchFurnitureEvent -= OnFurnitureSelectedEvent;
    }

    private void Update()
    {
        HadButtonPressThisFrame = false;
    }

    private void OnPlayStateButtonPressed(PlayStateButton playStateButton)
    {
        HadButtonPressThisFrame = true;
        if (CurrPlayState == playStateButton.PlayState)
            return;

        if (m_CurrSelectedButton)
            m_CurrSelectedButton.ToggleSelected(false);
        playStateButton.Button.ToggleSelected(true);
        m_CurrSelectedButton = playStateButton.Button;

        if (CurrPlayState == PlayState.PLACE_FURNITURE)
        {
            m_FurnitureDropdown.ToggleDropdown(false);
        }

        CurrPlayState = playStateButton.PlayState;

        if (CurrPlayState == PlayState.PLACE_FURNITURE)
        {
            m_FurnitureDropdown.ToggleDropdown(true);
        }

        m_FurniturePlacer.enabled = (CurrPlayState == PlayState.PLACE_FURNITURE);
        m_FurnitureRelocator.enabled = (CurrPlayState == PlayState.RELOCATE_FURNITURE);
        m_FurnitureDeleter.enabled = (CurrPlayState == PlayState.DELETE_FURNITURE);
    }
}
