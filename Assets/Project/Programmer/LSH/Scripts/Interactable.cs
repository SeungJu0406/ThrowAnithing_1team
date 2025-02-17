using UnityEngine;

public class Interactable : MonoBehaviour
{

    [Range(2f, 5f)][SerializeField] float overlapSphereRadius; //보물상자 근처 범위
    [Range(2f, 5f)][SerializeField] float playerAreaRadius; //플레이어 근처 랜덤생성될때 근처 범위
    Collider[] hitColliders = new Collider[1]; //해당 물체에 닿는 모든 콜라이더를 넣어둘 배열
    Collider playerCollider; //그중에서 플레이어 콜라이더만 저장할 변수

    [SerializeField] bool isInSphere; //T오버랩스피어에닿아있음 F아님
    [SerializeField] GameObject pressKeyUI;
    [SerializeField] GameObject pcText;
    [SerializeField] GameObject consoleText;

    [SerializeField] GameObject[] itemPrefabs;
    Vector3 itemRandomSpawnArea;

    [SerializeField] AudioClip bluechip; // 블루칩 생성 효과음
    [SerializeField] GameObject createEffect; // 블루칩 생성 이펙트

    private void Start()
    {
        isInSphere = false;
        SetActivePopUpUI(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Layer.Player)
        {
            Debug.Log("플레이어 들어옴");
        }
    }

    private void Update()
    {
        if (isInSphere)
        {
            if (InputKey.GetButtonDown(InputKey.Interaction))
            {
                SettingSpawnArea();
                DropRandomItems();
                SetFalseObject();
            }
        }
    }

    private void FixedUpdate()
    {
        int hitCount = Physics.OverlapSphereNonAlloc(transform.position, overlapSphereRadius, hitColliders, 1 << Layer.Player);
        if (hitCount != 0)
        {
            for(int i = 0; i < hitCount; i++)
            {

                playerCollider = hitColliders[i];
                isInSphere = true;
                SetActivePopUpUI(true);
            }
        }
        else
        {
            isInSphere = true;
            SetActivePopUpUI(false);
        }

    }


    //랜덤으로 스폰될 영역 설정
    public void SettingSpawnArea()
    {
        float range_x = playerCollider.bounds.size.x * playerAreaRadius;
        float range_z = playerCollider.bounds.size.z * playerAreaRadius;
        range_x = Random.Range((range_x / 2) * -1, range_x / 2);
        range_z = Random.Range((range_z / 2) * -1, range_z / 2);
        itemRandomSpawnArea = new Vector3(range_x, 0.5f, range_z);
    }

    //랜덤으로 스폰될 아이템 설정 후 생성
    public void DropRandomItems()
    {
        int itemRandom = Random.Range(0, itemPrefabs.Length);
        Instantiate(itemPrefabs[itemRandom], transform.position, Quaternion.identity);
        if (itemPrefabs[0])
        {
            SoundManager.PlaySFX(bluechip);
            ObjectPool.Get(createEffect, itemPrefabs[0].transform.position, Quaternion.Euler(-90, 0, 0), 5f);
            Debug.Log(ObjectPool.Get(createEffect, transform.position, Quaternion.Euler(-90, 0, 0), 5f));
        }


    }

    //생성시 보물상자 아이템은 사라짐
    public void SetFalseObject()
    {
        Destroy(gameObject);
    }

    private void SetActivePopUpUI(bool isActive)
    {
        if (pressKeyUI == null)
            return;

        GameObject _pc = pcText;
        GameObject _console = consoleText;
        // 각 디바이스에 맞는 텍스트 활성화
        _pc.SetActive(InputKey.PlayerInput.currentControlScheme == InputType.PC);
        _console.SetActive(InputKey.PlayerInput.currentControlScheme == InputType.CONSOLE);

        pressKeyUI.SetActive(isActive);
    }
}
