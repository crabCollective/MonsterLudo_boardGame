using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Scripts.Core.Input;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.GameObjects
{
    public enum eDiceType
    {
        REGULAR_DICE,
        SUPERDICE
    }

    /// <summary>
    /// Class representing dice. This is simple implementation requiring external source to stop the dice.
    /// </summary>
    public class DiceController : MonoBehaviour
    {
        public const int MAX_DICE_VALUE = 6;

        [Tooltip("When rolling, how long should be one sprite shown?")]
        [SerializeField] private float _rollingSpriteDuration = 0.1f;
        [Tooltip("Assign all sprites in right order")]
        [SerializeField] private Image _diceImage;
        [SerializeField] private List<Sprite> _allDiceSprites;
        [SerializeField] private GameObject _superDiceBackground;
        [SerializeField] private GameObject _superDiceControls;
        [SerializeField] private TextMeshProUGUI _showedNumber;
        [SerializeField] private MMF_Player _showNumberFeedback;

        private eDiceType _chosenDiceType = eDiceType.REGULAR_DICE;

        private Coroutine _rollingCoroutine = null;
        private int _chosenNumber = 0;

        public void InitDice(eDiceType chosenType)
        {
            _chosenDiceType = chosenType;

            if (_superDiceBackground != null)
                _superDiceBackground.SetActive(chosenType == eDiceType.SUPERDICE);

            if (_superDiceControls != null)
                _superDiceControls.SetActive(chosenType == eDiceType.SUPERDICE);

            gameObject.SetActive(true);
            if (_chosenDiceType == eDiceType.REGULAR_DICE)
                StartRolling();
            else
            {
                _chosenNumber = 0;
                SetDiceSprite();
            }
        }

        public void StartRolling()
        {
            //check if dice is already rolling
            if (_rollingCoroutine != null)
                return;

            _rollingCoroutine = StartCoroutine(RollingCoroutine());
        }

        /// <summary>
        /// Stops dice from rolling, immediately returning the result
        /// </summary>
        /// <returns>Chosen random number</returns>
        public int StopRolling()
        {
            if (_rollingCoroutine != null)
            {
                StopCoroutine(_rollingCoroutine);
                _rollingCoroutine = null;
            }

            SetRandomNumber();
            ShowDiceSelectedNumber();
            return _chosenNumber + 1;
        }

        public void HideDice()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Used for superdice to manually select needed number
        /// </summary>
        public void MoveSelectionRight()
        {
            _chosenNumber++;
            _chosenNumber = (_chosenNumber <= _allDiceSprites.Count - 1) ? _chosenNumber : 0;
            SetDiceSprite();
        }

        /// <summary>
        /// Used for superdice to manually select needed number
        /// </summary>
        public void MoveSelectionLeft()
        {
            _chosenNumber--;
            _chosenNumber = (_chosenNumber >= 0) ? _chosenNumber : _allDiceSprites.Count - 1;
            SetDiceSprite();
        }

        public int ConfirmSelectedNumber()
        {
            ShowDiceSelectedNumber();
            return _chosenNumber + 1;
        }

        private IEnumerator RollingCoroutine()
        {
            while (true)
            {
                SetRandomNumber();
                yield return new WaitForSeconds(_rollingSpriteDuration);
            }
        }

        private int SetRandomNumber()
        {
            _chosenNumber = Random.Range(0, _allDiceSprites.Count);
            SetDiceSprite();

            return _chosenNumber + 1;
        }

        private void SetDiceSprite()
        {
            _diceImage.sprite = _allDiceSprites[_chosenNumber];
        }

        private void ShowDiceSelectedNumber()
        {
            if (_showedNumber != null) _showedNumber.text = (_chosenNumber + 1).ToString();
            _showNumberFeedback?.PlayFeedbacks();
        }
    }
}
