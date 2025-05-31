using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class CutsceneManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Image slideImage;          // The Image component to display slides
    public GameObject cutscenePanel;  // The panel containing the cutscene UI
    public Button nextButton;         // Button to proceed to the next slide

    [Header("Cutscene Settings")]
    public List<Sprite> slides;       // List of slides (sprites)
    public int currentSlideIndex = 0; // Tracks the current slide
    public string nextSceneName;      // Name of the scene to load after the cutscene

    [Header("Transition Settings")]
    public float transitionDuration = 1f; // Duration of the fade-in/out transition

    private void Start()
    {
        // Automatically start the cutscene when the scene loads
        StartCutscene();

        // Add button listener
        nextButton.onClick.AddListener(NextSlide);
    }

    // Call this method to start the cutscene
    public void StartCutscene()
    {
        currentSlideIndex = 0; // Start from the first slide
        if (slides.Count > 0)
        {
            slideImage.sprite = slides[currentSlideIndex];
            StartCoroutine(FadeIn());
        }
        cutscenePanel.SetActive(true); // Show the cutscene panel
    }

    // Proceed to the next slide or end the cutscene
    private void NextSlide()
    {
        currentSlideIndex++;
        if (currentSlideIndex < slides.Count)
        {
            StartCoroutine(TransitionToNextSlide());
        }
        else
        {
            EndCutscene();
        }
    }

    // Transition to the next slide with fade-out and fade-in
    private IEnumerator TransitionToNextSlide()
    {
        yield return StartCoroutine(FadeOut());

        // Change the slide
        slideImage.sprite = slides[currentSlideIndex];

        yield return StartCoroutine(FadeIn());
    }

    // Hide the cutscene panel when the cutscene ends and load the next scene
    private void EndCutscene()
    {
        cutscenePanel.SetActive(false);

        // Check if a scene name has been set
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("No next scene name specified in CutsceneManager.");
        }
    }

    // Fade-in effect
    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        Color color = slideImage.color;
        color.a = 0f;
        slideImage.color = color;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsed / transitionDuration);
            slideImage.color = color;
            yield return null;
        }

        color.a = 1f;
        slideImage.color = color;
    }

    // Fade-out effect
    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color color = slideImage.color;
        color.a = 1f;
        slideImage.color = color;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            color.a = 1f - Mathf.Clamp01(elapsed / transitionDuration);
            slideImage.color = color;
            yield return null;
        }

        color.a = 0f;
        slideImage.color = color;
    }
}
