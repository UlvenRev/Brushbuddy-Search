using UnityEngine;
using TMPro;

public class SliderController : MonoBehaviour
{
    [SerializeField] private TMP_Text valueText;
    public void OnSliderChanged(float value)
    {
        valueText.text = value.ToString();
    }
}
