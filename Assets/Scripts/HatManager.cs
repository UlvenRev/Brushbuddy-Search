using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class HatManager : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (GameManager.Instance.canClick) {  // If the animation is already running, we won't ruin it by stacking it
            StartCoroutine(RaiseHat(reportGuess: true));
            GameManager.Instance.canClick = false;
        }
    }

    public IEnumerator RaiseHat(bool reportGuess = false)
    {
        float originalX = transform.position.x;

        gameObject.LeanMove(new Vector3(originalX, 1, 0), 0.4f);
        yield return new WaitForSeconds(0.4f);  // Wait for the animation above to finish

        yield return new WaitForSeconds(1f);  // Wait before starting the second animation

        gameObject.LeanMove(new Vector3(originalX, 0, 0), 0.4f);
        yield return new WaitForSeconds(0.4f);  // Wait for the second animation to end

        yield return new WaitForSeconds(0.4f);

        if (reportGuess) GameManager.Instance.CheckGuessedHat(gameObject);  // Check the guessed hat we CLICKED on
        // we know it was CLICKED and not called from the Game Manager because we flagged it with reportGuess ABOVE
    }
}
