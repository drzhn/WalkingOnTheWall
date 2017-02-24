using UnityEngine;
using System.Collections;

public class PauseFuncions : MonoBehaviour {

    public GameObject menu, character;

    private MonoBehaviour blur;
    void Start()
    {
        blur = Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ContinueButton()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1F;
        blur.enabled = false;
        menu.SetActive(false);
        character.SendMessage("PauseMessage", false);
    }

    public void ExitButton()
    {
        Debug.Log("Quit");
        Application.Quit();
        
    }
}
