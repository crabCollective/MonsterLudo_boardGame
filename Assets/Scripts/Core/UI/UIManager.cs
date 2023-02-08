using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Scripts.GameObjects.Figures;
using TMPro;
using UnityEngine;

namespace Scripts.Core.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _roundNumText;
        [SerializeField] private TextMeshProUGUI _redTeamText;
        [SerializeField] private TextMeshProUGUI _blueTeamText;
        [SerializeField] private TextMeshProUGUI _inPortalRedText;
        [SerializeField] private TextMeshProUGUI _inPortalBlueText;
        [SerializeField] private SimpleBar _superDiceBarRed;
        [SerializeField] private SimpleBar _superDiceBarBlue;
        [SerializeField] private GameObject _titleScreen;
        [SerializeField] private GameObject _finalScreen;
        [SerializeField] private TextMeshProUGUI _finalScreenBlueTeamText;
        [SerializeField] private TextMeshProUGUI _finalScreenRedTeamText;

        [Header("Feedbacks")]
        [SerializeField] private MMF_Player _turnAgainFeedback;
        [SerializeField] private MMF_Player _leftInPortalFeedback;
        [SerializeField] private MMF_Player _rightInPortalFeedback;
        [SerializeField] private MMF_Player _newRoundFeedback;

        public void SetRoundNumber(int round)
        {
            if (_roundNumText != null)
                _roundNumText.text = "ROUND " + round;

            _newRoundFeedback?.PlayFeedbacks();
        }

        public void SetTeamPlayingSign(eTeamName teamName)
        {
            if (_blueTeamText != null) _blueTeamText.gameObject.SetActive(teamName == eTeamName.BLUE);
            if (_redTeamText != null) _redTeamText.gameObject.SetActive(teamName == eTeamName.RED);
        }

        public void SetInPortalText(eTeamName teamName, int numInPortal)
        {
            if (teamName == eTeamName.RED)
            {
                if (_inPortalRedText != null) _inPortalRedText.text = "In portal: " + numInPortal + "/4";
                _leftInPortalFeedback?.PlayFeedbacks();
            }
            else if (_inPortalBlueText != null)
            {
                _inPortalBlueText.text = "In portal: " + numInPortal + "/4";
                _rightInPortalFeedback?.PlayFeedbacks();
            }
        }

        public void SetSuperDiceSliderBar(eTeamName teamName, int barValue)
        {
            if (teamName == eTeamName.RED)
            {
                if (_superDiceBarRed != null) _superDiceBarRed.SetValue(barValue);
            }
            else if (_superDiceBarBlue != null) _superDiceBarBlue.SetValue(barValue);
        }

        public void ActivateTitleScreen(bool isActive)
        {
            _titleScreen.gameObject.SetActive(isActive);
        }

        public void ShowFinalScreen(eTeamName teamName)
        {
            _finalScreenBlueTeamText.gameObject.SetActive(teamName == eTeamName.BLUE);
            _finalScreenRedTeamText.gameObject.SetActive(teamName == eTeamName.RED);
            _finalScreen.gameObject.SetActive(true);
        }

        public void ShowTurnAgainText()
        {
            _turnAgainFeedback?.PlayFeedbacks();
        }
    }
}
