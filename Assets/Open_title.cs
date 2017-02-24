using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Open_title : MonoBehaviour
{

    // Use this for initialization
    public Text panic, station;

    private bool flag = false;
    private string p = "panic", s = "station";
    private int i = 0;
    private float time = 0f, deltaTime = 0.1f;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        panic.text = "";
        station.text = "";
    }

    void Update()
    {
        if (Time.time - time > deltaTime)
        {
            if (deltaTime == 4f)
            {
                Debug.Log("loading Main Menu");
                SceneManager.LoadScene("level2");
                //load MAIN MENU
            }
            if (!flag)
            {

                if (panic.text != p)
                {
                    panic.text += p[i];
                    i++;
                    deltaTime = Random.Range(0.05f, 0.2f);
                }
                else
                {
                    flag = true;
                    i = 0;
                }

            }
            else
            {
                if (station.text != s)
                {
                    station.text += s[i];
                    i++;
                    deltaTime = Random.Range(0.05f, 0.2f);
                }
                else
                {
                    deltaTime = 4f;

                }
            }
            time = Time.time;
        }
    }
}
