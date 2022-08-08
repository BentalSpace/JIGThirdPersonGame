using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour {
    public static int nextScene;
    [SerializeField]
    GameObject slime;
    [SerializeField]
    GameObject ufo;
    GameObject sound;
    void Awake() {
        sound = GameObject.Find("SOUND");
    }
    void Start() {
        StartCoroutine(LoadScene());
        if(nextScene == 1) {
            slime.SetActive(true);
            // ΩΩ∂Û¿”
        }
        if(nextScene == 2) {
            ufo.SetActive(true);
            // ufo
        }
    }
    public static void LoadScene(int sceneName) {
        nextScene = sceneName;
        SceneManager.LoadScene("Loading");
    }
    IEnumerator LoadScene() {
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;
        while (!op.isDone) {
            yield return null;
            if (op.progress < 0.9f) {
            }
            else if(op.progress >= 0.9f && LoadingCam.isReady) {
                op.allowSceneActivation = true;
                Destroy(sound);
                yield break;
            }
        }
    }
    //IEnumerator LoadScene() {
    //    yield return null;
    //    AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
    //    op.allowSceneActivation = false;
    //    float timer = 0.0f;
    //    while (!op.isDone) {
    //        yield return null;
    //        timer += Time.deltaTime;
    //        if (op.progress < 0.9f) {
    //            progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, op.progress, timer);
    //            if (progressBar.fillAmount >= op.progress) {
    //                timer = 0f;
    //            }
    //        }
    //        else {
    //            progressBar.fillAmount = Mathf.Lerp(progressBar.fillAmount, 1f, timer);
    //            if (progressBar.fillAmount == 1.0f) {
    //                op.allowSceneActivation = true;
    //                yield break;
    //            }
    //        }
    //    }
    //}
}
