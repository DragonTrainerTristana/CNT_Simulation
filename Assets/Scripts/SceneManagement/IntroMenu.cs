using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroMenu : MonoBehaviour
{
    ONE_CNT onecnt;

    //INPUTFIELD
    [SerializeField]
    private InputField cnt_NumInput;
    private string cnt_Num;
    private int cnt_data;

    [SerializeField]
    private InputField reflectionNum;
    private int reflection;

    private void Start()
    {
        onecnt = GameObject.Find("CNT_Control").GetComponent<ONE_CNT>(); // 이곳에 에러가...!
    }

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

    public void OnClickStart() {


        cnt_data = int.Parse(cnt_NumInput.text);
        Debug.Log(cnt_data);
        onecnt.numCnt = cnt_data;
        onecnt.startState = true;

    }

    public void OnClickExit() {

        Application.Quit();
     
    }


    public void OnClickScenetoBack() {
        // 변수 초기화
        onecnt.startState = false;
        SceneManager.LoadScene("IntroScene");

    }

}
