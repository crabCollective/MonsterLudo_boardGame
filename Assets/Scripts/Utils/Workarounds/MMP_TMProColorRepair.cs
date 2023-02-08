using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Scripts.Utils.Workarounds
{
    /// <summary>
    /// After scene restart, MM_Player changes TMPro to black.
    /// Call this script to change TMPRo to proper color after the MM_Player use.
    /// </summary>
    public class MMP_TMProColorRepair : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Color _desiredColor;

        void Awake()
        {
            Debug.Assert(_text);
        }

        //Call this from MMFeedback after its usage
        public void ApplyChosenColor()
        {
            _text.color = _desiredColor;
        }
    }
}
