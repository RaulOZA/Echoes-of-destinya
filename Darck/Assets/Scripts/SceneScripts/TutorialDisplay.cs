using System.Collections;
using UnityEngine;

public class TutorialDisplay : MonoBehaviour
{
    public GameObject controlsUI;     // Assign the Controls GameObject in the Inspector
    public float displayTime = 3.0f;  // Time to display the Controls UI
    private CanvasGroup canvasGroup;  // Reference to the CanvasGroup component

    void Start()
    {
        // Get the CanvasGroup component from the controlsUI GameObject
        canvasGroup = controlsUI.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            // Add a CanvasGroup component if one doesn't exist
            canvasGroup = controlsUI.AddComponent<CanvasGroup>();
        }
        // Start the coroutine to show and fade in/out the UI
        StartCoroutine(ShowControlsWithFade());
    }

    private IEnumerator ShowControlsWithFade()
    {
        controlsUI.SetActive(true);
        canvasGroup.alpha = 0f;  // Start fully transparent

        // Fade in
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f, 1.0f));

        // Wait for the specified display time
        yield return new WaitForSeconds(displayTime);

        // Fade out
        yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0f, 1.0f));

        controlsUI.SetActive(false);   // Disable the Controls UI
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, float duration)
    {
        float elapsedTime = 0f;
        cg.alpha = start;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, elapsedTime / duration);
            yield return null;
        }
        cg.alpha = end;
    }
}
