using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    public class TimeOfDay : MonoBehaviour
    {
        private float _currentTime = 0.366f;

        public Light sunLight;
        public Light moonLight;

        public Crest.OceanRenderer oceanRenderer;

        [Tooltip("Ocean controller")]
        public WaterController oceanController;

        public InteriorLightController interiorLightController;
        public ExteriorLightController exteriorLightController;

        public const float SunHigh = .5f;
        public const float MoonHighStart = 0f;
        public const float MoonHighEnd = 1f;

        [Tooltip("Padding for the high points of the sun and moon (how long the sun/moon strength is at its highest)")]
        [Range(0, 0.15f)]
        public float paddingHigh = 0.15f;

        public const float MorningLow = 0.25f;
        public const float EveningLow = 0.75f;

        [Tooltip("Padding for the low points of the sun and moon (how long there is complete darkness)")]
        [Range(0, 0.1f)]
        public float paddingLow = 0.05f;

        // public TMP_Text dayTimeLabel;

        public Slider timeSlider;

        private bool _cycleDayTime = true;

        private float _cycleSpeed = 0.05f;

        public void UpdateDayTime(float progress)
        {
            _currentTime = progress;

            float sunRotation = Mathf.Lerp(-90, 270, progress); // 0 = -90 (midnight), 1 = 270 (midnight), in between 0 & 1 -> corresponding in between -90 & 270
            sunLight.transform.rotation = Quaternion.Euler(sunRotation, -50, 0);

            float moonRotation = sunRotation - 180;
            moonLight.transform.rotation = Quaternion.Euler(moonRotation, -50, 0);

            if (progress is >= MorningLow - 0.2f and <= EveningLow + 0.02f)
            {
                SwitchToNightSetting(false);
            }
            else
            {
                SwitchToNightSetting(true);
            }

            interiorLightController.LightIntensityOverDay(progress);
            exteriorLightController.CheckIfNight(progress);

            //UpdateTimeLabel(progress);

            oceanController.CausticsStrengthOverDay(progress, paddingLow, paddingHigh);

            oceanController.OceanFogDensityOverDay(progress, paddingLow);
        }

        /*
        private void UpdateTimeLabel(float progress)
        {
            TimeSpan time = TimeSpan.FromSeconds(progress * 60 * 60 * 24);

            dayTimeLabel.text = time.ToString("hh':'mm");
        }
        */

        private void SwitchToNightSetting(bool isNight)
        {
            if (isNight)
            {
                sunLight.shadows = LightShadows.None;
                moonLight.shadows = LightShadows.Soft;

                oceanRenderer._primaryLight = moonLight;
            }
            else
            {
                sunLight.shadows = LightShadows.Soft;
                moonLight.shadows = LightShadows.None;

                oceanRenderer._primaryLight = sunLight;
            }
        }

        public void UpdateCycleSpeed(float speed)
        {
            _cycleSpeed = speed;
        }

        public void ToggleDayTimeCycle()
        {
            _cycleDayTime = !_cycleDayTime;

            if (_cycleDayTime)
            {
                timeSlider.enabled = false;
            }
            else
            {
                timeSlider.enabled = true;
            }
        }

        private void FixedUpdate()
        {
            if (!_cycleDayTime) return;

            float currentTime = _currentTime;
            if (currentTime >= 1)
            {
                currentTime = 0;
            }
            currentTime += _cycleSpeed/24f * Time.deltaTime;
            timeSlider.value = currentTime;

            UpdateDayTime(currentTime);
        }

        private void Start()
        {
            UpdateDayTime(_currentTime);
        }
    }
}
