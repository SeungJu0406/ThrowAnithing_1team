using System.Collections;
using UnityEngine;

/// <summary>
/// 코루틴 대리 실행 클래스
/// </summary>
public class CoroutineHandler : MonoBehaviour
{
    public static CoroutineHandler Instance;

    private void Awake()
    {
        InitSingleTon();
    }

    /// <summary>
    /// 코루틴 실행
    /// </summary>
    public static Coroutine StartRoutine(IEnumerator enumerator)
    {
        CreateCoroutineHandler();
        return Instance.StartCoroutine(enumerator);
    }

    public static Coroutine StartRoutine(Coroutine coroutine, IEnumerator enumerator)
    {
        CreateCoroutineHandler();
        if (coroutine == null)
        {
            return Instance.StartCoroutine(enumerator);
        }
        return null;
    }

    /// <summary>
    /// 코루틴 중지
    /// </summary>
    public static Coroutine StopRoutine(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            Instance.StopCoroutine(coroutine);
            coroutine = null;
        }
        return coroutine;
    }


    private static CoroutineHandler CreateCoroutineHandler()
    {
        if (Instance != null)
        {
            return Instance;
        }
        else
        {
            // 새롭게 풀 오브젝트 생성
            GameObject newPool = new GameObject("CoroutineHandler");
            CoroutineHandler pool = newPool.AddComponent<CoroutineHandler>();
            return pool;
        }
    }
    private void InitSingleTon()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
