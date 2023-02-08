using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Scripts.Core.Input
{
    /// <summary>
    /// This SO class is used for input handling. Just reference it from any part of the game, bind to the required events and you are good to go 
    /// </summary>
    [CreateAssetMenu(fileName = "InputHandlerSO", menuName = "Game/Input Handler")]
    public class InputHandlerSO : ScriptableObject, GameInput.IDefaultInputActions
    {
        public event Action onConfirmPressed;
        public event Action onLeftPressed;
        public event Action onRightPressed;
        public event Action onQuitPressed;
        public event Action onRestartPressed;

        private GameInput _gameInput;

        private void OnEnable()
        {
            if (_gameInput == null)
            {
                _gameInput = new GameInput();
                _gameInput.DefaultInput.SetCallbacks(this);
            }

            EnableInput();
        }

        private void OnDisable()
        {
            DisableInput();
        }

        public void OnConfirm(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                onConfirmPressed?.Invoke();
        }

        public void OnLeft(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                onLeftPressed?.Invoke();
        }

        public void OnRight(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                onRightPressed?.Invoke();
        }

        public void EnableInput()
        {
            _gameInput.DefaultInput.Enable();
        }

        public void DisableInput()
        {
            _gameInput.DefaultInput.Disable();
        }

        public void OnQuit(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                onQuitPressed?.Invoke();
        }

        public void OnRestart(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                onRestartPressed?.Invoke();
        }
    }
}
