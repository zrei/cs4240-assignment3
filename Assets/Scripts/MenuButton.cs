using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private Button m_Button;
    [SerializeField] private Image m_Background;
    [SerializeField] private Color m_NormalColor = Color.white;
    [SerializeField] private Color m_SelectedColor = Color.green;
    [SerializeField] private Image m_SpriteImage;
    [SerializeField] private TextMeshProUGUI m_Text;

    public event Action OnPressedAction;

    private void Start()
    {
        m_Button.onClick.AddListener(OnPressed);
        ToggleSelected(false);
    }

    private void Onestroy()
    {
        m_Button.onClick.RemoveAllListeners();   
    }

    private void OnPressed()
    {
        OnPressedAction?.Invoke();
    }

    public void ClearEvents()
    {
        OnPressedAction = null;
    }

    public void ToggleSelected(bool isSelected)
    {
        m_Background.color = isSelected ? m_SelectedColor : m_NormalColor;
    }

    public void SetInfo(FurnitureInfo furnitureInfo)
    {
        if (furnitureInfo.FurnitureSprite == null)
        {
            m_SpriteImage.enabled = false;
            m_Text.enabled = true;
            m_Text.text = furnitureInfo.name;
        }
        else
        {
            m_SpriteImage.sprite = furnitureInfo.FurnitureSprite;
            m_SpriteImage.enabled = true;
            m_Text.enabled = false;
        }
        
    }
}