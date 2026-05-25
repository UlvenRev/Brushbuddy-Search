using UnityEngine;

public class CheckboxController : MonoBehaviour
{
    public void OnValueChanged()
    {
        if (GameManager.Instance.progressiveDifficulty) GameManager.Instance.progressiveDifficulty = false;
        else GameManager.Instance.progressiveDifficulty = true;
        UIManager.Instance.ChangeSettingsAppearance();
    }
}
