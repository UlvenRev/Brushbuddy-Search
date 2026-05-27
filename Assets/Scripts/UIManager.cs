using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider roundsSlider;
    [SerializeField] private Slider swapsSlider;
    [SerializeField] private Slider speedSlider;
    [SerializeField] private Slider hatsSlider;

    [SerializeField] private TMP_Text mainTitle;
    [SerializeField] private TMP_Text subTitle;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] public GameObject MenuScreen;
    [SerializeField] public GameObject ActiveRoundUI;
    [SerializeField] private TMP_Text[] settingsText;

    [SerializeField] private TMP_Text roundsPassedLabel;

    private Color disabledColor = new Color(0.7176471f, 0.6784314f, 0.5529412f, 1f);
    private Color enabledColor = new Color(0.4666667f, 0.4235294f, 0.2980392f, 1f);

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
        GameManager.Instance.numberOfHats = (int)hatsSlider.value;
    }

    public void ChangeSettingsAppearance()
    {
        if (GameManager.Instance.progressiveDifficulty)  // Just turned ON progressive difficulty => disable the settings
        {
            roundsSlider.interactable = false;
            swapsSlider.interactable = false;
            speedSlider.interactable = false;
            hatsSlider.interactable = false;
            foreach (TMP_Text text in settingsText)
            {
                text.color = disabledColor;
            }
        } else
        {
            roundsSlider.interactable = true;
            swapsSlider.interactable = true;
            speedSlider.interactable = true;
            hatsSlider.interactable = true;
            foreach (TMP_Text text in settingsText)
            {
                text.color = enabledColor;
            }
        }
    }

    public void ShowMenu(string mainTitleText, string subTitleText, string buttonText)
    {
        mainTitle.text = mainTitleText;
        subTitle.text = subTitleText;
        this.buttonText.text = buttonText;
        roundsPassedLabel.text = "Search progress will appear here";

        ActiveRoundUI.SetActive(false);
        MenuScreen.SetActive(true);
    }

    public void UpdateRoundsPassedLabel(int roundNumber)
    {
        int totalRounds = GameManager.Instance.numberOfRounds;

        if (GameManager.Instance.progressiveDifficulty) roundsPassedLabel.text = "Rounds passed: " + roundNumber;
        else roundsPassedLabel.text = "Rounds passed: " + roundNumber + "/" + totalRounds;
    }
}
