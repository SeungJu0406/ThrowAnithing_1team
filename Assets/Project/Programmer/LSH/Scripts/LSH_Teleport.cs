using UnityEngine;
using UnityEngine.SceneManagement;

public class LSH_Teleport : MonoBehaviour
{
    [SerializeField] GameObject player;
    SceneField nextStage; //스테이지 이동용 변수
    SceneField randomHiddenRoom; //비밀방 입장용 변수

    Vector3 beforeTeleportPos; //기존 씬에서 비밀방으로 텔포타기 전 위치
    Vector3 afterTeleportPos; //애디티브 씬으로 텔포탄 후 위치

    [SerializeField] bool isSceneAdditive; //T애디티브 씬이 열려있음, F아님



    private void OnTriggerEnter(Collider other)
    {
        
        if (other.tag == "PortalHidden")
        {
            int randomNum = Random.Range(1, 100);

            //멀티 씬 로딩, LoadSceneMode.Additive
            PortalSceneNumber portalSceneNumber = other.GetComponent<PortalSceneNumber>();
            //randomHiddenRoom = portalSceneNumber.hiddenSceneArr[Random.Range(0, portalSceneNumber.hiddenSceneArr.Length)];
            //ChangeScene(randomHiddenRoom); //0함정 1몬스터 2블루칩
            if (randomNum <= 40)
            {
                randomHiddenRoom = portalSceneNumber.hiddenSceneArr[0];
            }
            else if (randomNum > 40 && randomNum <= 80)
            {
                randomHiddenRoom = portalSceneNumber.hiddenSceneArr[1];
            }
            else if(randomNum > 80)
            {
                randomHiddenRoom = portalSceneNumber.hiddenSceneArr[2];
            }
            ChangeScene(randomHiddenRoom); //0함정 1몬스터 2블루칩
            other.gameObject.SetActive(false);
        }


        if (other.tag == Tag.Portal)
        {
            //다중씬 로딩중일때
            if (isSceneAdditive)
            {

                //플레이어 기존씬에 있던 자리로 돌려놓고
                Vector3 beforePos = new Vector3(beforeTeleportPos.x, beforeTeleportPos.y, beforeTeleportPos.z);
                transform.position = beforeTeleportPos;
                // 전체 씬 탐색
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    Scene loadedScene = SceneManager.GetSceneAt(i);
                    // 탐색한 씬이 히든 씬 이름과 같을 때
                    if (loadedScene.name == randomHiddenRoom.SceneName)
                    {
                        SceneManager.UnloadSceneAsync(loadedScene);
                    }
                }
                ////열어둔 랜덤 씬을 저장한 뒤 언로드씬() 해줌 -> 오류, 닫지 않는것으로 해결
                //Scene additiveScene = SceneManager.GetSceneByName(randomHiddenRoom);

                // SceneManager.UnloadScene(additiveScene);

                //변수 false로 바꿈
                isSceneAdditive = false;

            }
            //스테이지 이동할때
            else
            {

                //직접 씬 이동, LoadSceneMode.Single
                //nextStage = other.GetComponent<PortalSceneNumber>().nextScene;
                //SceneManager.LoadScene(nextStage);
                ToStage._ToStage();


            }

        }



    }

    public void ChangeScene(SceneField SceneName)
    {
        //애디티브로 다중 씬 열기
        SceneManager.LoadScene(SceneName, LoadSceneMode.Additive);
        isSceneAdditive = true;

        //플레이어 기존씬 위치 저장
        beforeTeleportPos = player.transform.position;
        afterTeleportPos = new Vector3(400, 1, 400);

        //플레이어 기존씬에서 애디티브 씬으로 위치 이동
        transform.position = afterTeleportPos;

    }

}
