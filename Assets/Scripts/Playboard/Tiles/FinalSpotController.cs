using System.Collections;
using System.Collections.Generic;
using Scripts.GameObjects.Figures;
using SWS;
using UnityEngine;

namespace Scripts.Playboard.Tiles
{
    public class FinalSpotController : MonoBehaviour
    {
        [SerializeField]
        private eTeamName _assignedTeam;
        public eTeamName AssignedTeam { get => _assignedTeam; private set => _assignedTeam = value; }

        [SerializeField] private ParticleSystem _occupiedEffect;
        [SerializeField] private PathManager _assignedPath;
        public PathManager AssignedPath { get => _assignedPath; private set => _assignedPath = value; }
        [SerializeField] private float _effectActivationDelay = 1f;

        private bool _occupied = false;
        public bool Occupied { get => _occupied; private set => _occupied = value; }

        void Start()
        {
            _occupied = false;
            if (_occupiedEffect != null)
                _occupiedEffect.gameObject.SetActive(false);
        }

        public void SetOccupied(bool activateEffect)
        {
            _occupied = true;
            if (activateEffect && _occupiedEffect != null)
            {
                Invoke("ActivateEffect", _effectActivationDelay);
            }
        }

        private void ActivateEffect()
        {
            _occupiedEffect.gameObject.SetActive(true);
        }
    }
}
