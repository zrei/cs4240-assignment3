using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FurnitureDropdown : MonoBehaviour
{
    [SerializeField] private RectTransform m_ViewportRectTransform;
    [SerializeField] private RectTransform m_ContentRectTransform;
    [SerializeField] private ScrollRect m_ScrollRect;
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private FurnitureInfo[] m_FurnitureInfo;
    [SerializeField] private MenuButton m_MenuButtonPrefab;
    [SerializeField] private float m_TotalTime;

    public FurnitureInfo CurrSelectedFurnitureInfo { get; private set; } = null;

    private Coroutine m_CurrPlayingCoroutine = null;

    public GameObject CurrSelectedFurniturePrefab { get; private set; } = null;

    private List<MenuButton> m_MenuButtons = new();

    private RectTransform m_ViewportParent;
    private MenuButton m_CurrSelectedButton = null;
    [SerializeField] private FurnitureGridTest m_FurnitureGridTest;

    public float GridSize => m_FurnitureGridTest.GridSize;

    private void Start()
    {
        m_ViewportParent = m_ViewportRectTransform.parent.GetComponent<RectTransform>();
        foreach (FurnitureInfo furnitureInfo in m_FurnitureInfo)
        {
            MenuButton menuButton = Instantiate(m_MenuButtonPrefab, m_ContentRectTransform);
            menuButton.SetInfo(furnitureInfo);
            menuButton.OnPressedAction += () => OnSelectFurniture(menuButton, furnitureInfo);
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

    private void OnSelectFurniture(MenuButton button, FurnitureInfo furnitureInfo)
    {
        if (m_CurrSelectedButton)
            m_CurrSelectedButton.ToggleSelected(false);
        button.ToggleSelected(true);
        m_CurrSelectedButton = button;
        CurrSelectedFurniturePrefab = furnitureInfo.FurniturePrefab;
        CurrSelectedFurnitureInfo = furnitureInfo; // Store the full info
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

        m_CanvasGroup.alpha = targetAlpha;
    }
    public FurnitureInfo GetCurrentFurnitureInfo()
    {
        return CurrSelectedFurnitureInfo;
    }



#if UNITY_EDITOR
    public void SetDropdownContent(FurnitureInfo[] furnitureInfo)
    {
        m_FurnitureInfo = furnitureInfo;
        EditorUtility.SetDirty(this.gameObject);
    }
#endif
}
