using UnityEngine;
using UnityEngine.UI;

public class ThrusterHud : MonoBehaviour
{
    #region Variables
    private Slider _thrusterSlider;
    #endregion

    private void Start()
    {
        _thrusterSlider = GetComponent<Slider>();
    }

    public void SetThrusterSliderValue(float speed)
    {
        _thrusterSlider.value = speed;
    }
}