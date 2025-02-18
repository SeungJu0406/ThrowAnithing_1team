using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoading : MonoBehaviour
{
    Coroutine loadingRoutine; //로딩 코루틴
    [SerializeField] Image loadingImage; //로딩중에만 나타날 이미지
    [SerializeField] Slider loadingBar; //로딩중임을 알려주는 프로그레스 바
    [SerializeField] Button startBtn; 
    [SerializeField] SceneField nextScene; //이동할 씬 이름


    private void Start()
    {
        loadingImage.gameObject.SetActive(false);
        startBtn.onClick.AddListener(() => ChangeScene(nextScene));
    }


    public void ChangeScene(SceneField SceneName)
    {
        if (loadingRoutine != null)
            return;

        loadingImage.gameObject.SetActive(true);
        loadingRoutine = StartCoroutine(LoadingRoutine(SceneName));
    }


    IEnumerator LoadingRoutine(SceneField SceneName)
    {
        AsyncOperation oper = SceneManager.LoadSceneAsync(SceneName);

        oper.allowSceneActivation = false;


        while (oper.isDone == false)
        {

            if (oper.progress < 0.9f)
            {
                //로딩중
                Debug.Log($"loading = {oper.progress}");
                loadingBar.value = oper.progress;
            }
            else
            {
                //로딩 완료 (바로 넘어가지는 코드)
                //Debug.Log("loading success");
                //oper.allowSceneActivation = true;
                break;
            }
            yield return null;
        }


        //페이크 로딩 (억지로 로딩바 채우는 눈속임버전)
        float time = 0f;
        while (time < 5f)
        {
            time += Time.deltaTime;
            loadingBar.value = time / 5f;
            yield return null;
        }
        Debug.Log("loading success");
        while (time >= 5f)
        {            

            if (Input.GetKeyDown(KeyCode.Return))
            {
                oper.allowSceneActivation = true;

            }

            yield return null;
        }

    }



}
