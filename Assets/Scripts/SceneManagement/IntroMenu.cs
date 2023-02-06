using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroMenu : MonoBehaviour
{
    public void OnClickBackMain() {
        SceneManager.LoadScene("IntroScene");
    }

    public void OnClickMenu() {
        SceneManager.LoadScene("MenuScene");
    }
    public void OnClickScene1()
    {
        SceneManager.LoadScene("Scene1");
    }
    public void OnClickScene2()
    {
        SceneManager.LoadScene("Scene2");
    }
    public void OnClickScene3()
    {
        SceneManager.LoadScene("Scene3");
    }
    public void OnClickScene4()
    {
        SceneManager.LoadScene("Scene4");
    }
    public void OnClickScene5()
    {
        SceneManager.LoadScene("Scene5");
    }

}
