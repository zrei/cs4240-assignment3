using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : Singleton<InputHandler>
{
    [SerializeField] private InputActionReference m_TouchPositionActionReference;
    [SerializeField] private InputActionReference m_TapAndReleaseActionReference;
    
    public Vector2 TouchPosition {get; private set;} = Vector2.zero;

    public event Action TapBeginEvent;
    public event Action TapCompleteEvent;

    private void Start()
    {
        m_TouchPositionActionReference.action.performed += UpdateTouchPosition;
        m_TapAndReleaseActionReference.action.started += OnTapBegin;
        m_TapAndReleaseActionReference.action.performed += OnTapComplete;
    }

    private void OnDestroy()
    {
        m_TouchPositionActionReference.action.performed -= UpdateTouchPosition;
        m_TapAndReleaseActionReference.action.started -= OnTapBegin;
        m_TapAndReleaseActionReference.action.performed -= OnTapComplete;
    }

    private void OnTapBegin(InputAction.CallbackContext callbackContext)
    {
        TapBeginEvent?.Invoke();
    }

    private void OnTapComplete(InputAction.CallbackContext callbackContext)
    {
        TapCompleteEvent?.Invoke();
    }

    private void UpdateTouchPosition(InputAction.CallbackContext callbackContext)
    {
        TouchPosition = callbackContext.ReadValue<Vector2>();
    }
}