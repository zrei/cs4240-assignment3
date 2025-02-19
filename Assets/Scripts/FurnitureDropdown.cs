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
    [SerializeField] private RectTransform m_RectTransform;
    [SerializeField] private float m_FinalPosY;
    [SerializeField] private float m_FinalHeight;
    [SerializeField] private float m_TotalTime;
    [SerializeField] private Button m_MenuButtonPrefab;

    [SerializeField] private FurnitureButton[] m_FurnitureButtons;

    private Coroutine m_CurrPlayingCoroutine = null;

    public GameObject CurrSelectedFurniturePrefab {get; private set;} = null;

    private List<Button> m_MenuButtons = new();

    private void Start()
    {
        foreach (FurnitureButton furnitureButton in m_FurnitureButtons)
        {
            Button menuButton = Instantiate(m_MenuButtonPrefab, this.transform);
            menuButton.onClick.AddListener(() => OnSelectFurniture(menuButton, furnitureButton));
            m_MenuButtons.Add(menuButton);
        }
    }

    private void OnDestroy()
    {
        foreach (Button menuButton in m_MenuButtons)
        {
            menuButton.onClick.RemoveAllListeners();
        }
    }

    private void OnSelectFurniture(Button button, FurnitureButton furnitureButton)
    {
        button.Select();
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
        float targetPos = expand ? m_FinalPosY : 0;
        float targetHeight = expand ? m_FinalHeight : 0;
        float time = Mathf.Abs(targetPos - m_RectTransform.anchoredPosition.y) / m_FinalPosY * m_TotalTime;

        float t = 0f;
        while (t < time)
        {
            yield return null;
            t += Time.deltaTime;
            m_RectTransform.anchoredPosition = new Vector2(m_RectTransform.anchoredPosition.x, Mathf.Lerp(m_RectTransform.anchoredPosition.y, targetPos, t / time));
            m_RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Lerp(m_RectTransform.rect.height, targetHeight, t / time));
        }
    }
}
