using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] hatsList;
    [SerializeField] private GameObject brushbuddyPrefab;  // The prefab we use for instantiating the object, we will NOT delete this
    private GameObject brushbuddyInstance;  // Where we store the actual instance we will delete later

    
    public int numberOfSwaps = 4;
    public int numberOfRounds = 3;
    public float swapSpeed = 0.85f / 3;

    private int roundsCompleted;
    public bool canClick;
    private GameObject correctHat;

    public bool progressiveDifficulty;

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void BeginGame()  // Action for the canvas button
    {
        // Get the settings for the round
        UIManager.Instance.GetRoundSettings();
        roundsCompleted = 0;

        UIManager.Instance.MenuScreen.SetActive(false);
        UIManager.Instance.RestartButton.SetActive(true);
        StartCoroutine(GameRound());
    }

    public void RestartGame()
    {
        StopAllCoroutines();
        UIManager.Instance.ShowMenu("Brushbuddy Search!", "Follow the caps closely and choose wisely to find Brushbuddy...", "Begin");
    }

    public void CheckGuessedHat(GameObject guessedHat)  // Called only when the hats are lowered completely
    {
        if (guessedHat == correctHat)
        {
            roundsCompleted++;
            RemoveBrushbuddy();
            if (roundsCompleted < numberOfRounds)
            {
                guessedHat = null;
                StartCoroutine(GameRound());    
            } 
            else  // Won the full game - show the menu screen with a different title
            {
                UIManager.Instance.ShowMenu("Well done!", "You found Brushbuddy! Attentiveness is a great trait for a witch!", "Play again");
            }
            
        } else  // Lost the game - show the menu screen with a different title
        {
            RemoveBrushbuddy();
            UIManager.Instance.ShowMenu("Brushbuddy got away...", "Better find him before he deals any trouble around Master Qifrey's atelier...", "Try again");
        }
    }

    IEnumerator GameRound()
    {
        canClick = false;
        correctHat = hatsList[1];  // Always under the middle hat for now

        if (brushbuddyInstance == null) PlaceBrushbuddy();  // Instantiate the brushbuddy if it's not in the scene
        yield return StartCoroutine(RaiseAllHats());  // and raise all hats to show where it is (no parameter because we don't need to CHECK the raised hat)
        yield return new WaitForSeconds(0.1f);   
        RemoveBrushbuddy();

        yield return StartCoroutine(RunSwaps());
        PlaceBrushbuddy();

        canClick = true;
    }

    IEnumerator RaiseAllHats()
    {
        foreach (GameObject hat in hatsList)
        {
            HatManager hatManagerScript = hat.GetComponent<HatManager>();
            StartCoroutine(hatManagerScript.RaiseHat());  // No parameter because we don't need to CHECK the raised hat
        }
        yield return new WaitForSeconds(3f);  // Each RaiseHat() animation is 3 seconds and they all start at the SAME TIME
        // so wait for 3 seconds for them to finish completely before moving on to the next animations
    }

    private void PlaceBrushbuddy()  // For now brushbuddy is always under Hat 2
    {
        Vector3 brushbuddyPos = hatsList[1].transform.position;
        brushbuddyPos.y = -0.55f;
        brushbuddyInstance = Instantiate(brushbuddyPrefab, brushbuddyPos, Quaternion.identity); 
    }

    private void RemoveBrushbuddy()
    {
        Destroy(brushbuddyInstance);
        brushbuddyInstance = null;
    }

    IEnumerator RunSwaps()
    {
        for (int i = 0; i < numberOfSwaps; i++)
        {
            int hat1Index = UnityEngine.Random.Range(0, hatsList.Length);
            int hat2Index = UnityEngine.Random.Range(0, hatsList.Length);
            while (hat1Index == hat2Index) hat2Index = UnityEngine.Random.Range(0, hatsList.Length);  // To avoid choosing the same hat for a swap

            yield return StartCoroutine(SwapTwoHats(hatsList[hat1Index], hatsList[hat2Index]));  // yield return here will make the code WAIT for the coroutine to actually finish before executing the code below   
        }
    }

    IEnumerator SwapTwoHats(GameObject hat1, GameObject hat2)
    {
        Vector3 hat1Pos = hat1.transform.position;
        Vector3 hat2Pos = hat2.transform.position;
        hat1.LeanMove(hat2Pos, swapSpeed).setEaseInSine();
        hat2.LeanMove(hat1Pos, swapSpeed).setEaseInSine();

        yield return new WaitForSeconds(swapSpeed + 0.5f);  // Wait for the animation to finish + a slight pause
    }
}
