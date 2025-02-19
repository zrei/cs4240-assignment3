using UnityEngine;
using UnityEngine.UI;

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

    public PlayState CurrPlayState {get; private set;} = PlayState.NONE;
    public GameObject CurrFurniturePrefab => m_FurnitureDropdown.CurrSelectedFurniturePrefab;
    private MenuButton m_CurrSelectedButton = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        foreach (PlayStateButton playStateButton in m_PlayStateButtons)
        {
            playStateButton.Button.OnPressedAction += () => OnPlayStateButtonPressed(playStateButton);
        }
    }

    private void Oestroy()
    {
        foreach (PlayStateButton playStateButton in m_PlayStateButtons)
        {
            playStateButton.Button.ClearEvents();
        }
    }

    private void OnPlayStateButtonPressed(PlayStateButton playStateButton)
    {
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
    }
}
