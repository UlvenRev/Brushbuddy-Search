using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider roundsSlider;
    [SerializeField] private Slider swapsSlider;
    [SerializeField] private Slider speedSlider;

    [SerializeField] private TMP_Text mainTitle;
    [SerializeField] private TMP_Text subTitle;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] public GameObject MenuScreen;
    [SerializeField] public GameObject RestartButton;

    public static UIManager Instance { get; private set; }

    void Awake() 
    {
        Instance = this;
    }

    public void GetRoundSettings()
    {
        GameManager.Instance.numberOfSwaps = (int)swapsSlider.value;
        GameManager.Instance.numberOfRounds = (int)roundsSlider.value;
        GameManager.Instance.swapSpeed = 0.85f / speedSlider.value;
    }

    public void ChangeSettingsAppearance()
    {
        if (GameManager.Instance.progressiveDifficulty)  // Just turned ON progressive difficulty => disable the settings
        {
            roundsSlider.interactable = false;
            swapsSlider.interactable = false;
            speedSlider.interactable = false;
        } else
        {
            roundsSlider.interactable = true;
            swapsSlider.interactable = true;
            speedSlider.interactable = true;
        }
    }

    public void ShowMenu(string mainTitleText, string subTitleText, string buttonText)
    {
        mainTitle.text = mainTitleText;
        subTitle.text = subTitleText;
        this.buttonText.text = buttonText;
        RestartButton.SetActive(false);
        MenuScreen.SetActive(true);
    }
}
