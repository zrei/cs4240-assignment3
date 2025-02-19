using UnityEngine;
using UnityEngine.UI;

public enum PlayState
{
    NONE,
    PLACE_FURNITURE,
    RELOCATE_FURNITURE,
    DELETE_FURNITURE
}

public class MenuButtonHandler : MonoBehaviour
{
    [System.Serializable]
    public struct PlayStateButton
    {
        public Button Button;
        public PlayState PlayState;
    }

    [SerializeField] private PlayStateButton[] m_PlayStateButtons;
    [SerializeField] private FurnitureDropdown m_FurnitureDropdown;

    public PlayState CurrPlayState {get; private set;} = PlayState.NONE;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        foreach (PlayStateButton playStateButton in m_PlayStateButtons)
        {
            playStateButton.Button.onClick.AddListener(() => OnPlayStateButtonPressed(playStateButton));
        }
    }

    private void Oestroy()
    {
        foreach (PlayStateButton playStateButton in m_PlayStateButtons)
        {
            playStateButton.Button.onClick.RemoveAllListeners();
        }
    }

    private void OnPlayStateButtonPressed(PlayStateButton playStateButton)
    {
        if (CurrPlayState == playStateButton.PlayState)
            return;

        playStateButton.Button.Select();

        if (CurrPlayState == PlayState.PLACE_FURNITURE)
        {
            // compress dropdown
        }

        CurrPlayState = playStateButton.PlayState;

        if (CurrPlayState == PlayState.PLACE_FURNITURE)
        {
            // expand dropdown
        }
    }
}
