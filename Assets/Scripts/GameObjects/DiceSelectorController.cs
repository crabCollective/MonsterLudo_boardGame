using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.GameObjects
{
    public enum eDiceSelectionPhase
    {
        SELECTION,
        ROLLING
    }

    /// <summary>
    /// Class representing selection of the dice - shows the dice selector screen
    /// </summary>
    public class DiceSelectorController : MonoBehaviour
    {
        [SerializeField] private DiceController _dice;
        [SerializeField] private GameObject _diceSelectionMenu;
        [SerializeField] private GameObject _regularDiceSelector;
        [SerializeField] private GameObject _superDiceSelector;
        private eDiceSelectionPhase _currentPhase = eDiceSelectionPhase.SELECTION;

        private eDiceType _currentlySelected = eDiceType.REGULAR_DICE;
        public eDiceType CurrentlySelected { get => _currentlySelected; private set => _currentlySelected = value; }

        /// <summary>
        /// Initializes selection screen
        /// </summary>
        /// <param name="ChooseRegularDice">If true, no selection screen is shown and regular dice starts run</param>
        public void InitDiceSelector(bool autoChooseRegularDice)
        {
            _currentlySelected = eDiceType.REGULAR_DICE;
            if (autoChooseRegularDice)
            {
                _dice.InitDice(_currentlySelected);
                _currentPhase = eDiceSelectionPhase.ROLLING;
            }
            else
            {
                _diceSelectionMenu.gameObject.SetActive(true);
                _currentPhase = eDiceSelectionPhase.SELECTION;
                SelectDice();
            }
        }

        public int HandleInputConfirm()
        {
            if (_currentPhase == eDiceSelectionPhase.SELECTION)
            {
                _diceSelectionMenu.gameObject.SetActive(false);
                _dice.InitDice(_currentlySelected);
                _currentPhase = eDiceSelectionPhase.ROLLING;

                return -1;
            }
            else
            {
                return _currentlySelected == eDiceType.REGULAR_DICE ? _dice.StopRolling() : _dice.ConfirmSelectedNumber();
            }
        }

        public void HandleInputLeft()
        {
            if (_currentPhase == eDiceSelectionPhase.SELECTION)
            {
                InvertSelection();
                SelectDice();
            }
            else if (_currentPhase == eDiceSelectionPhase.ROLLING && _currentlySelected == eDiceType.SUPERDICE)
            {
                _dice.MoveSelectionLeft();
            }
        }

        public void HandleInputRight()
        {
            if (_currentPhase == eDiceSelectionPhase.SELECTION)
            {
                InvertSelection();
                SelectDice();
            }
            else if (_currentPhase == eDiceSelectionPhase.ROLLING && _currentlySelected == eDiceType.SUPERDICE)
            {
                _dice.MoveSelectionRight();
            }
        }

        private void InvertSelection()
        {
            _currentlySelected = _currentlySelected == eDiceType.REGULAR_DICE ? eDiceType.SUPERDICE : eDiceType.REGULAR_DICE;
        }

        private void SelectDice()
        {
            switch (_currentlySelected)
            {
                case eDiceType.REGULAR_DICE:
                    _regularDiceSelector.gameObject.SetActive(true);
                    _superDiceSelector.gameObject.SetActive(false);
                    break;
                case eDiceType.SUPERDICE:
                    _regularDiceSelector.gameObject.SetActive(false);
                    _superDiceSelector.gameObject.SetActive(true);
                    break;
                default:
                    Debug.LogError("Selecting dice type - something wrong happened!");
                    break;
            }
        }
    }
}
