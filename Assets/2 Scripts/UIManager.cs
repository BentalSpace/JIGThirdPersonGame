using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    GameObject escPanel;
    [SerializeField]
    GameObject[] panels;
    [SerializeField]
    GameObject bgm;
    // ¿Œ∆Æ∑Œ
    GameObject startPanel;
    GameObject optionPanel;
    Animator anim;

    private void Awake() {
        if(SceneManager.GetActiveScene().buildIndex == 0) {
            startPanel = GameObject.Find("StartPanel");
            optionPanel = GameObject.Find("OptionPanel");
            anim = GameObject.Find("Char").GetComponent<Animator>();
        }
    }
    private void Start() {
        if (SceneManager.GetActiveScene().buildIndex == 0) {
            startPanel.SetActive(false);
            optionPanel.SetActive(false);
        }
    }
    void Update() {
        foreach(GameObject panel in panels) {
            if (panel.activeSelf)
                return;
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (escPanel.activeSelf) {
                ContinueBtn();
            }
            else if (!escPanel.activeSelf) {
                CamCtrl.dontCtrl = true;
                escPanel.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0;
            }
        }
    }
    public void ContinueBtn() {
        escPanel.SetActive(false);
        CamCtrl.dontCtrl = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1;
    }
    public  void QuitBtn() {
        Application.Quit();
    }
    public void RetryBtn(int sceneNum) {
        LoadingSceneManager.LoadScene(sceneNum);
        Time.timeScale = 1;
    }
    public void MainBtn() {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }
    public void OptionBtn() {
        optionPanel.SetActive(true);
    }
    public void OptionCancelBtn() {
        optionPanel.SetActive(false);
    }
    public void GameStartBtn() {
        startPanel.SetActive(true);
        anim.SetBool("Start", true);
    }
    public void GameStartCancelBtn() {
        startPanel.SetActive(false);
        anim.SetBool("Start", false);
    }
    public void GoBattle(int scene) {
        anim.SetTrigger("Go");
        StartCoroutine(GoBattle2(scene));
    }

    IEnumerator GoBattle2(int scene) {
        yield return new WaitForSeconds(2f);
        DontDestroyOnLoad(bgm);
        LoadingSceneManager.LoadScene(scene);
        //SceneManager.LoadScene(scene);
    }
}
