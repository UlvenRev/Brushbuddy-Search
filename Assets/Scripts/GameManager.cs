using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject brushbuddyPrefab;  // The prefab we use for instantiating the object, we will NOT delete this
    private GameObject brushbuddyInstance;  // Where we store the actual instance we will delete later
    private GameObject[] hatsList;
    [SerializeField] private GameObject hatPrefab;

    // ------------ Game Settings values --------------
    public bool progressiveDifficulty;

    public int numberOfSwaps;
    public int numberOfRounds;
    public float swapSpeed;
    public int numberOfHats;

    // These are used only when progressive difficulty is ON:
    private int progNumOfSwaps = 3;  // Starting from 3 swaps, increse by 1
    private float progSwapSpeed = 0.85f;  // Decrese by 0.05 each time


    private int roundsCompleted;
    public bool canClick;
    private GameObject correctHat;


    public static GameManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void BeginGame()  // Action for the canvas button
    {
        RemoveHats();

        // Get the settings for the round if the settings are custom
        if (!progressiveDifficulty) UIManager.Instance.GetRoundSettings();
        else
        {
            // Take default values:
            numberOfSwaps = 4; 
            numberOfRounds = 3;  
            swapSpeed = 0.85f / 3;  
            numberOfHats = 3;
        }

        InstantiateHats();
        
        roundsCompleted = 0;
        UIManager.Instance.MenuScreen.SetActive(false);
        UIManager.Instance.ActiveRoundUI.SetActive(true);
        StartCoroutine(GameRound());   

    }

    private void InstantiateHats()
    {
        hatsList = new GameObject[numberOfHats];

        float gap;
        float startPoint;
        if (numberOfHats % 2 == 0)  // For even number of hats, they need to have an offset which 
        {
            gap = 2f;
            startPoint = -1 - gap * (Mathf.Floor(numberOfHats / 2) - 1);  // will "start" from -1 and not 0, so e.g. -3 -1 1 3 for 4 hats (look centered on the screen)
        }
        else  // For odd number of hats, they have the middle hat at 0
        {
            gap = 2.5f;
            startPoint = 0 - gap * Mathf.Floor(numberOfHats / 2);  // So e.g. -2.5 0 2.5 for 3 hats (and they look centered on the screen)
        }

        for (int i = 0; i < numberOfHats; i++) 
        {
            GameObject hat = Instantiate(hatPrefab, new Vector3(startPoint + gap * i, 0, 0), Quaternion.identity);
            hatsList[i] = hat;
        }
    }

    private void RemoveHats()
    {
        GameObject[] oldHats;
        oldHats = GameObject.FindGameObjectsWithTag("Hat");
        foreach (var i in oldHats)
        {
            Destroy(i);
        }
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

            if (!progressiveDifficulty)
            {
                if (roundsCompleted < numberOfRounds)  // Custom play mode with limited rounds
                {
                    guessedHat = null;
                    UIManager.Instance.UpdateRoundsPassedLabel(roundsCompleted);

                    StartCoroutine(GameRound());    
                } 
                else UIManager.Instance.ShowMenu("Well done!", "You found Brushbuddy! Attentiveness is a great trait for a witch!", "Play again"); 
            } else  // Inifnite playmode so just start a new round with increased difficulty
            {
                if (progSwapSpeed - 0.05f > 0.05f) progSwapSpeed -= 0.05f;  // Speed increases each round
                else progSwapSpeed = 0.05f;

                if (roundsCompleted % 2 == 0) progNumOfSwaps++;  // Each 2 rounds the number of hat swaps will increase

                UIManager.Instance.UpdateRoundsPassedLabel(roundsCompleted);
                StartCoroutine(GameRound()); 
            }
            
        } else  // Lost the game - show the menu screen with a different title
        {
            UIManager.Instance.ShowMenu("Brushbuddy got away...", "Better find him before he deals any trouble around Master Qifrey's atelier...", "Try again");

            // Reset the values in case we were playing the progressive difficulty mode
            progNumOfSwaps = 3;
            progSwapSpeed = 0.85f;
        }
    }

    IEnumerator GameRound()
    {
        canClick = false;
        correctHat = hatsList[1];  // Always under the middle hat for now

        RemoveBrushbuddy();
        PlaceBrushbuddy();
        
        yield return StartCoroutine(RaiseAllHats());  // and raise all hats to show where it is (no parameter because we don't need to CHECK the raised hat)
        yield return new WaitForSeconds(0.1f);   
        RemoveBrushbuddy();

        if (progressiveDifficulty) yield return StartCoroutine(RunSwaps(progNumOfSwaps));
        else yield return StartCoroutine(RunSwaps(numberOfSwaps));

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

    IEnumerator RunSwaps(int swaps)
    {
        for (int i = 0; i < swaps; i++)
        {
            int hat1Index = UnityEngine.Random.Range(0, hatsList.Length);
            int hat2Index = UnityEngine.Random.Range(0, hatsList.Length);
            while (hat1Index == hat2Index) hat2Index = UnityEngine.Random.Range(0, hatsList.Length);  // To avoid choosing the same hat for a swap

            if (progressiveDifficulty) yield return StartCoroutine(SwapTwoHats(hatsList[hat1Index], hatsList[hat2Index], progSwapSpeed));  // yield return here will make the code WAIT for the coroutine to actually finish before executing the code below   
            else yield return StartCoroutine(SwapTwoHats(hatsList[hat1Index], hatsList[hat2Index], swapSpeed));  // The custom speed set by user
        }
    }

    IEnumerator SwapTwoHats(GameObject hat1, GameObject hat2, float speed)
    {
        Vector3 hat1Pos = hat1.transform.position;
        Vector3 hat2Pos = hat2.transform.position;
        hat1.LeanMove(hat2Pos, speed).setEaseInSine();
        hat2.LeanMove(hat1Pos, speed).setEaseInSine();

        yield return new WaitForSeconds(speed + 0.5f);  // Wait for the animation to finish + a slight pause
    }
}
