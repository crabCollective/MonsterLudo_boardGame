using System.Collections;
using System.Collections.Generic;
using Scripts.Core.Input;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.Core
{
    /// <summary>
    /// Simple class taking in care actions like quit or restart
    /// </summary>
    public class GameRunner : MonoBehaviour
    {
        [SerializeField] private InputHandlerSO _inputHandler;

        private void OnEnable()
        {
            _inputHandler.onQuitPressed += QuitGame;
            _inputHandler.onRestartPressed += RestartGame;
        }

        private void OnDisable()
        {
            _inputHandler.onQuitPressed -= QuitGame;
            _inputHandler.onRestartPressed -= RestartGame;
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

    }
}
