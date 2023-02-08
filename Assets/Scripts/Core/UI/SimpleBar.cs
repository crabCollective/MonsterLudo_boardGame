using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Core.UI
{
    /// <summary>
    /// Basic class for fill bars
    /// </summary>
    public class SimpleBar : MonoBehaviour
    {
        public bool SetOnStart = false;
        [SerializeField] private float _maxValue = 100;
        public float MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                if (SliderComponent != null)
                    SliderComponent.maxValue = _maxValue;
            }
        }
        public float StartValue = 100;

        private float _currentValue;
        public float CurrentValue
        {
            get { return _currentValue; }
            private set { _currentValue = value; }
        }

        [SerializeField] private Slider SliderComponent = null;

        void Awake()
        {
            if (SliderComponent == null)
                Debug.LogError(gameObject.name + ", SimpleBar.cs: Unity UI.Slider has not been assigned!");
        }

        void Start()
        {
            if (SetOnStart)
            {
                Init(StartValue, MaxValue);
            }
        }

        //Init() might or might not be called on start
        public virtual void Init(float startValue, float maxValue)
        {
            StartValue = startValue;
            MaxValue = maxValue;

            SliderComponent.maxValue = maxValue;
            SetValue(startValue);
        }

        public virtual void ResetToDefault()
        {
            Init(StartValue, MaxValue);
        }

        public virtual void SetValue(float valToSet)
        {
            _currentValue = valToSet < 0 ? 0 : valToSet;
            SliderComponent.value = _currentValue;
        }
    }
}
