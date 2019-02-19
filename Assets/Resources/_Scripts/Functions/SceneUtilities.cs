using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneUtilities : MonoBehaviour {
    // VARIABLES //
    public Animator animator;
    private int levelToLoad;

    // FUNCTIONS //
    // Load scene with fading animation
    public void FadeToLevel(int levelIndex) {
        levelToLoad = levelIndex;
        animator.SetTrigger("FadeOut");
    }

    // Load level after animation
    public void OnFadeComplete() {
        LoadLevel(levelToLoad);
    }

    // Load scene utilities
    public void LoadLevel(int sceneIndex) {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    IEnumerator LoadAsynchronously(int sceneIndex) {
        AsyncOperation operaion = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operaion.isDone) {
            yield return null;
        }
    }
}
