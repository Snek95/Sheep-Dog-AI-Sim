using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderCounter : MonoBehaviour {

    [SerializeField] Slider sheepCountSlider;
    [SerializeField] TextMeshProUGUI sliderValueText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        sheepCountSlider.value = 10;
    }

    public void OnSliderChange(float sliderValue) {

        int sheepCounterValue = Mathf.RoundToInt(sheepCountSlider.value);

        sliderValueText.text = sheepCounterValue.ToString();
    }
}


    