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
    public void RetryBtn() {
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
    }
    public void MainBtn() {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }
}
