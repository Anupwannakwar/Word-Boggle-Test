using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MM : MonoBehaviour
{
    public void Endless (int index)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + index);
    }

    public void Back()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
