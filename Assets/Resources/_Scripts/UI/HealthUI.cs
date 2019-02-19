using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour {
    // VARIABLE //
    [Header("Game Object")]
    public GameObject healthBarPrefab;
    public Transform target;

    [Header("Game Settings")]
    public float visibleTime = 5;
    
    private Transform UI, cam;
    private Image healthSlider;
    private float lastVisible;
    private Canvas worldCanvas;

    // FUNCTION //
	// Use this for initialization
	void Start () {
        // Get camera position
        cam = Camera.main.transform;

        // Get canvas
        GameObject canvasObject = GameObject.FindWithTag("World") as GameObject;
        worldCanvas = canvasObject.GetComponent<Canvas>();

        // Instantiate health bar and hide it 
        UI = Instantiate(healthBarPrefab, worldCanvas.transform).transform;
        healthSlider = UI.GetChild(0).GetComponent<Image>();
        UI.gameObject.SetActive(false);

        GetComponent<ObjectStats>().OnHealthChanged += OnHealthChanged;
	}
	
	// Update is called once per frame
	void LateUpdate () {
        UI.position = target.position;
        UI.forward = -cam.forward;

        if (Time.time - lastVisible > visibleTime) {
            UI.gameObject.SetActive(false);
        }
	}

    // On health changed, changed health bar
    private void OnHealthChanged(int maxHealth, int currentHealth) {
        float ratio = currentHealth / (float)maxHealth;

        // Set UI to active and fill the slider
        UI.gameObject.SetActive(true);
        healthSlider.fillAmount = ratio;

        // Set last visable time
        lastVisible = Time.time;

        if (currentHealth <= 0) {
            Destroy(UI.gameObject);
        }
    }
}
