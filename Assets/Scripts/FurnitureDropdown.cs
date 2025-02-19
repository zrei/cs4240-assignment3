using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureDropdown : MonoBehaviour
{
    [System.Serializable]
    public struct FurnitureButton
    {
        public Sprite FurnitureSprite;
        public GameObject FurniturePrefab;
    }

    [SerializeField] private RectTransform m_ViewportRectTransform;
    [SerializeField] private RectTransform m_ContentRectTransform;
    [SerializeField] private ScrollRect m_ScrollRect;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private FurnitureButton[] m_FurnitureButtons;
    [SerializeField] private MenuButton m_MenuButtonPrefab;
    [SerializeField] private float m_TotalTime;

    private Coroutine m_CurrPlayingCoroutine = null;

    public GameObject CurrSelectedFurniturePrefab {get; private set;} = null;

    private List<MenuButton> m_MenuButtons = new();

    private RectTransform m_ViewportParent;
    private MenuButton m_CurrSelectedButton = null;

    private void Start()
    {
        m_ViewportParent = m_ViewportRectTransform.parent.GetComponent<RectTransform>();
        foreach (FurnitureButton furnitureButton in m_FurnitureButtons)
        {
            MenuButton menuButton = Instantiate(m_MenuButtonPrefab, m_ContentRectTransform);
            menuButton.SetSprite(furnitureButton.FurnitureSprite);
            menuButton.OnPressedAction += () => OnSelectFurniture(menuButton, furnitureButton);
            m_MenuButtons.Add(menuButton);
        }
    }

    private void OnDestroy()
    {
        foreach (MenuButton menuButton in m_MenuButtons)
        {
            menuButton.ClearEvents();
        }
    }

    private void OnSelectFurniture(MenuButton button, FurnitureButton furnitureButton)
    {
        if (m_CurrSelectedButton)
            m_CurrSelectedButton.ToggleSelected(false);
        button.ToggleSelected(true);
        m_CurrSelectedButton = button;
        CurrSelectedFurniturePrefab = furnitureButton.FurniturePrefab;
    }

    public void ToggleDropdown(bool expand)
    {
        if (m_CurrPlayingCoroutine != null)
        {
            StopCoroutine(m_CurrPlayingCoroutine);
        }

        m_CurrPlayingCoroutine = StartCoroutine(ToggleDropdownCoroutine(expand));
    }

    private IEnumerator ToggleDropdownCoroutine(bool expand)
    {
        if (!expand)
        {
            if (m_CurrSelectedButton)
                m_CurrSelectedButton.ToggleSelected(false);
            m_CurrSelectedButton = null;
            CurrSelectedFurniturePrefab = null;
            m_CanvasGroup.blocksRaycasts = false;
            m_CanvasGroup.interactable = false;
        }
        else
        {
            m_ScrollRect.verticalNormalizedPosition = 1f;
        }

        float initialHeight = m_ViewportRectTransform.offsetMin.y;
        float targetHeight = expand ? 0f : m_ViewportParent.rect.height;
        
        float initialAlpha = m_CanvasGroup.alpha;
        float targetAlpha = expand ? 1f : 0f;

        float time = Mathf.Abs(targetHeight - initialHeight) / m_ViewportParent.rect.height * m_TotalTime;

        Debug.Log(initialHeight + ", " + targetHeight + ", " + time);
        float t = 0f;
        while (t < time)
        {
            yield return null;
            t += Time.deltaTime;
            m_ViewportRectTransform.offsetMin = new Vector2(m_ViewportRectTransform.offsetMin.x, Mathf.Lerp(initialHeight, targetHeight, t / time));
            m_CanvasGroup.alpha = Mathf.Lerp(initialAlpha, targetAlpha, t / time);
        }

        m_ViewportRectTransform.offsetMin = new Vector2(m_ViewportRectTransform.offsetMin.x, targetHeight);

        if (expand)
        {
            m_CanvasGroup.interactable = true;
            m_CanvasGroup.blocksRaycasts = true;
        }

        Debug.Log("!!!!");
        m_CanvasGroup.alpha = targetAlpha;
    }
}
