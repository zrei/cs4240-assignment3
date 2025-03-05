using UnityEngine;

public class FurniturePlacer : FurniturePositionHandler
{
    protected override void OnEnable()
    {
        base.OnEnable();
        
        MenuButtonHandler.Instance.OnFurnitureSelectedEvent += OnSwitchFurniture;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        
        MenuButtonHandler.Instance.OnFurnitureSelectedEvent -= OnSwitchFurniture;
    }

    private void OnSwitchFurniture()
    {
        if (m_CurrentPreview == null)
            return;
        ClearPreview();
        CreatePreviewInstance();
    }

    protected override bool ShouldPreview()
    {
        return base.ShouldPreview() && MenuButtonHandler.Instance.CurrFurnitureInfo != null;
    }

    protected override FurnitureInfo GetFurnitureInfo()
    {
        return MenuButtonHandler.Instance.CurrFurnitureInfo;
    }

    protected override GameObject GetFurniturePrefab()
    {
        return MenuButtonHandler.Instance.CurrFurnitureInfo.FurniturePrefab;
    }
}
