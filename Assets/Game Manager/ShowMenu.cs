using UnityEngine;
using System.Collections;

public class ShowMenu : MonoBehaviour
{
    private GameObject pauseMenu, character, ui, win;
    private MonoBehaviour blur;
    private MonoBehaviour characterController;
    void Start()
    {
        blur = Camera.main.GetComponent<UnityStandardAssets.ImageEffects.BlurOptimized>();
        character = GameObject.Find("Adam");
        characterController = character.GetComponent<Adam_Controller.Adam_Controller>();
        ui = GameObject.Find("UI");
        pauseMenu = ui.transform.GetChild(0).gameObject;
        win = ui.transform.GetChild(1).gameObject;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (pauseMenu.activeSelf == false)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0f;
                pauseMenu.SetActive(true);
                characterController.enabled = false;
                blur.enabled = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1F;
                pauseMenu.SetActive(false);
                characterController.enabled = true;
                blur.enabled = false;
            }
        }

    }

    public void ContinueButton()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1F;
        blur.enabled = false;
        pauseMenu.SetActive(false);
        characterController.enabled = true;
    }

    public void ExitButton()
    {
        Debug.Log("Quit");
        Application.Quit();

    }

    public void Win()
    {
        Debug.Log("WIN");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0F;
        win.SetActive(true);
        characterController.enabled = false;
        blur.enabled = false;
    }
}
