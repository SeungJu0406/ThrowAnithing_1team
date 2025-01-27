using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;
    public struct PoolInfo
    {
        public Queue<GameObject> Pool;
        public GameObject Prefab;
        public Transform Parent;
    }
    /// <summary>
    /// 프리팹용
    /// </summary>
    private Dictionary<GameObject, PoolInfo> m_poolDic = new Dictionary<GameObject, PoolInfo>();
    private static Dictionary<GameObject, PoolInfo> _poolDic { get { return Instance.m_poolDic; } }
    private static Transform thisTransform => Instance.transform;
    /// <summary>
    /// 인스턴스용
    /// </summary>
    private Dictionary<GameObject, PoolInfo> m_poolObjectDic = new Dictionary<GameObject, PoolInfo>();
    private static Dictionary<GameObject, PoolInfo> _poolObjectDic { get { return Instance.m_poolObjectDic; } }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// 풀 생성
    /// </summary>
    public static ObjectPool CreateObjectPool()
    {

        if (Instance != null)
        {
            return Instance;
        }
        else
        {
            // 새롭게 풀 오브젝트 생성
            GameObject newPool = new GameObject("ObjectPool");
            ObjectPool pool = newPool.AddComponent<ObjectPool>();
            return pool;
        }

    }
    #region GetPool
    public static GameObject GetPool(GameObject prefab)
    {     
        PoolInfo info = FindPool(prefab);
        if (info.Pool.Count > 0)
        {
            GameObject instance = info.Pool.Dequeue();
            instance.transform.position = Vector3.zero;
            instance.transform.rotation = Quaternion.identity;
            instance.transform.SetParent(null);
            instance.gameObject.SetActive(true);
            return instance.gameObject;
        }
        else
        {
            GameObject instance = Instantiate(info.Prefab);
            _poolObjectDic.Add(instance, info);
            return instance;
        }
    }
    public static GameObject GetPool(GameObject prefab, Transform transform)
    {
        PoolInfo info = FindPool(prefab);
        if (info.Pool.Count > 0)
        {
            GameObject instance = info.Pool.Dequeue();
            instance.transform.SetParent(transform);
            instance.transform.position = transform.position;
            instance.transform.rotation = transform.rotation;
            instance.gameObject.SetActive(true);
            return instance.gameObject;
        }
        else
        {
            GameObject instance = Instantiate(info.Prefab, transform);
            instance.transform.position = transform.position;
            instance.transform.rotation = transform.rotation;
            _poolObjectDic.Add(instance, info);
            return instance;
        }
    }
    public static GameObject GetPool(GameObject prefab, Transform transform, bool worldPositionStay)
    {
        PoolInfo info = FindPool(prefab);
        if (info.Pool.Count > 0)
        {
            GameObject instance = info.Pool.Dequeue();
            instance.transform.SetParent(transform);
            if (worldPositionStay == true)
            {
                instance.transform.position = prefab.transform.position;
                instance.transform.rotation = prefab.transform.rotation;
            }
            else
            {
                instance.transform.position = transform.position;
                instance.transform.rotation = transform.rotation;
            }
            instance.gameObject.SetActive(true);

            return instance.gameObject;
        }
        else
        {
            GameObject instance = Instantiate(info.Prefab, transform, worldPositionStay);
            _poolObjectDic.Add(instance, info);
            return instance;
        }
    }
    public static GameObject GetPool(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        PoolInfo info = FindPool(prefab);
        if (info.Pool.Count > 0)
        {
            GameObject instance = info.Pool.Dequeue();
            instance.transform.position = pos;
            instance.transform.rotation = rot;
            instance.gameObject.SetActive(true);
            instance.transform.SetParent(null);
            return instance.gameObject;
        }
        else
        {
            GameObject instance = Instantiate(info.Prefab, pos, rot);
            _poolObjectDic.Add(instance, info);
            return instance;
        }
    }
    public static T GetPool<T>(T prefab) where T : Component
    {
        PoolInfo info = FindPool(prefab.gameObject);
        if (info.Pool.Count > 0)
        {
            GameObject instance = info.Pool.Dequeue();
            instance.transform.position = Vector3.zero;
            instance.transform.rotation = Quaternion.identity;
            instance.transform.SetParent(null);
            instance.gameObject.SetActive(true);
            T component = instance.GetComponent<T>();
            return component;
        }
        else
        {
            GameObject instance = Instantiate(info.Prefab);
            _poolObjectDic.Add(instance, info);
            T component = instance.GetComponent<T>();
            return component;
        }
    }
    public static T GetPool<T>(T prefab, Transform transform) where T : Component
    {
        PoolInfo info = FindPool(prefab.gameObject);
        if (info.Pool.Count > 0)
        {
            GameObject instance = info.Pool.Dequeue();
            instance.transform.SetParent(transform);
            instance.transform.position = transform.position;
            instance.transform.rotation = transform.rotation;
            instance.gameObject.SetActive(true);
            T component = instance.GetComponent<T>();
            return component;
        }
        else
        {
            GameObject instance = Instantiate(info.Prefab, transform);
            _poolObjectDic.Add(instance, info);
            instance.transform.position = transform.position;
            instance.transform.rotation = transform.rotation;
            T component = instance.GetComponent<T>();
            return component;
        }
    }
    public static T GetPool<T>(T prefab, Transform transform, bool worldPositionStay) where T : Component
    {
        PoolInfo info = FindPool(prefab.gameObject);
        if (info.Pool.Count > 0)
        {
            GameObject instance = info.Pool.Dequeue();
            instance.transform.SetParent(transform);
            if (worldPositionStay == true)
            {
                instance.transform.position = prefab.transform.position;
                instance.transform.rotation = prefab.transform.rotation;
            }
            else
            {
                instance.transform.position = transform.position;
                instance.transform.rotation = transform.rotation;
            }
            instance.gameObject.SetActive(true);
            T component = instance.GetComponent<T>();
            return component;
        }
        else
        {
            GameObject instance = Instantiate(info.Prefab, transform, worldPositionStay);
            _poolObjectDic.Add(instance, info);
            T component = instance.GetComponent<T>();
            return component;
        }
    }
    public static T GetPool<T>(T prefab, Vector3 pos, Quaternion rot) where T : Component
    {
        PoolInfo info = FindPool(prefab.gameObject);
        if (info.Pool.Count > 0)
        {
            GameObject instance = info.Pool.Dequeue();
            instance.transform.position = pos;
            instance.transform.rotation = rot;
            instance.transform.SetParent(null);
            instance.gameObject.SetActive(true);
            T component = instance.GetComponent<T>();
            return component;
        }
        else
        {
            GameObject instance = Instantiate(info.Prefab, pos, rot);
            _poolObjectDic.Add(instance, info);
            T component = instance.GetComponent<T>();
            return component;
        }
    }
    public static GameObject GetPool(GameObject prefab, float returnDelay)
    {
        GameObject instance = null;
        PoolInfo info = FindPool(prefab);
        if (info.Pool.Count > 0)
        {
            instance = info.Pool.Dequeue();
            instance.transform.position = Vector3.zero;
            instance.transform.rotation = Quaternion.identity;
            instance.transform.SetParent(null);
            instance.gameObject.SetActive(true);
        }
        else
        {
            instance = Instantiate(info.Prefab);
            _poolObjectDic.Add(instance, info);
        }
        ReturnPool(instance, returnDelay);
        return instance;
    }
    public static GameObject GetPool(GameObject prefab, Transform transform, float returnDelay)
    {
        GameObject instance = null;
        PoolInfo info = FindPool(prefab);
        if (info.Pool.Count > 0)
        {
            instance = info.Pool.Dequeue();
            instance.transform.position = transform.position;
            instance.transform.rotation = transform.rotation;
            instance.transform.SetParent(transform);
            instance.gameObject.SetActive(true);
        }
        else
        {
            instance = Instantiate(info.Prefab, transform);
            instance.transform.position = transform.position;
            instance.transform.rotation = transform.rotation;
            _poolObjectDic.Add(instance, info);
        }
        ReturnPool(instance, returnDelay);
        return instance;
    }
    public static GameObject GetPool(GameObject prefab, Transform transform, bool worldPositionStay, float returnDelay)
    {
        GameObject instance = null;
        PoolInfo info = FindPool(prefab);
        if (info.Pool.Count > 0)
        {
            instance = info.Pool.Dequeue();
            instance.transform.SetParent(transform);
            if (worldPositionStay == true)
            {
                instance.transform.position = prefab.transform.position;
                instance.transform.rotation = prefab.transform.rotation;
            }
            else
            {
                instance.transform.position = transform.position;
                instance.transform.rotation = transform.rotation;
            }
            instance.gameObject.SetActive(true);
        }
        else
        {
            instance = Instantiate(info.Prefab, transform, worldPositionStay);
            _poolObjectDic.Add(instance, info);
        }
        ReturnPool(instance, returnDelay);
        return instance;
    }
    public static GameObject GetPool(GameObject prefab, Vector3 pos, Quaternion rot, float returnDelay)
    {
        GameObject instance = null;
        PoolInfo info = FindPool(prefab);
        if (info.Pool.Count > 0)
        {
            instance = info.Pool.Dequeue();
            instance.transform.position = pos;
            instance.transform.rotation = rot;
            instance.transform.SetParent(null);
            instance.gameObject.SetActive(true);
        }
        else
        {
            instance = Instantiate(info.Prefab, pos, rot);
            _poolObjectDic.Add(instance, info);
        }
        ReturnPool(instance, returnDelay);
        return instance;
    }
    public static T GetPool<T>(T prefab, float returnDelay) where T : Component
    {
        T component = null;
        PoolInfo info = FindPool(prefab.gameObject);
        if (info.Pool.Count > 0)
        {
            GameObject instance = info.Pool.Dequeue();
            instance.transform.position = Vector3.zero;
            instance.transform.rotation = Quaternion.identity;
            instance.transform.SetParent(null);
            instance.gameObject.SetActive(true);
            component = instance.GetComponent<T>();
        }
        else
        {
            GameObject instance = Instantiate(info.Prefab);
            _poolObjectDic.Add(instance, info);
            component = instance.GetComponent<T>();
        }
        ReturnPool(component, returnDelay);
        return component;
    }
    public static T GetPool<T>(T prefab, Transform transform, float returnDelay) where T : Component
    {
        T component = null;
        PoolInfo info = FindPool(prefab.gameObject);
        if (info.Pool.Count > 0)
        {
            GameObject instance = info.Pool.Dequeue();
            instance.transform.SetParent(transform);
            instance.transform.position = transform.position;
            instance.transform.rotation = transform.rotation;
            instance.gameObject.SetActive(true);
            component = instance.GetComponent<T>();
        }
        else
        {
            GameObject instance = Instantiate(info.Prefab, transform);
            _poolObjectDic.Add(instance, info);
            instance.transform.position = transform.position;
            instance.transform.rotation = transform.rotation;
            component = instance.GetComponent<T>();

        }
        ReturnPool(component, returnDelay);
        return component;
    }
    public static T GetPool<T>(T prefab, Transform transform, bool worldPositionStay, float returnDelay) where T : Component
    {
        T component = null;
        PoolInfo info = FindPool(prefab.gameObject);
        if (info.Pool.Count > 0)
        {
            GameObject instance = info.Pool.Dequeue();
            instance.transform.SetParent(transform);
            if (worldPositionStay == true)
            {
                instance.transform.position = prefab.transform.position;
                instance.transform.rotation = prefab.transform.rotation;
            }
            else
            {
                instance.transform.position = transform.position;
                instance.transform.rotation = transform.rotation;
            }
            instance.gameObject.SetActive(true);
            component = instance.GetComponent<T>();
        }
        else
        {
            GameObject instance = Instantiate(info.Prefab, transform, worldPositionStay);
            _poolObjectDic.Add(instance, info);
            component = instance.GetComponent<T>();
        }
        ReturnPool(component, returnDelay);
        return component;
    }
    public static T GetPool<T>(T prefab, Vector3 pos, Quaternion rot, float returnDelay) where T : Component
    {
        T component = null;
        PoolInfo info = FindPool(prefab.gameObject);
        if (info.Pool.Count > 0)
        {
            GameObject instance = info.Pool.Dequeue();
            instance.transform.position = pos;
            instance.transform.rotation = rot;
            instance.transform.SetParent(null);
            instance.gameObject.SetActive(true);
            component = instance.GetComponent<T>();
        }
        else
        {
            GameObject instance = Instantiate(info.Prefab, pos, rot);
            _poolObjectDic.Add(instance, info);
            component = instance.GetComponent<T>();
        }
        ReturnPool(component, returnDelay);
        return component;
    }
    #endregion
    #region ReturnPool
    public static void ReturnPool(GameObject instance)
    {
        CreateObjectPool();
        if (instance.activeSelf == false)
            return;


        PoolInfo info = default;
        if (_poolObjectDic.ContainsKey(instance) == true)
        {
            info = _poolObjectDic[instance];
        }
        else
        {
            info = FindPool(instance);
        }
        instance.transform.position = info.Prefab.transform.position;
        instance.transform.rotation = info.Prefab.transform.rotation;
        instance.transform.localScale = info.Prefab.transform.localScale;
        instance.transform.SetParent(info.Parent);
        instance.gameObject.SetActive(false);
        info.Pool.Enqueue(instance);
    }
    public static void ReturnPool<T>(T instance) where T : Component
    {
        CreateObjectPool();

        if (instance.gameObject.activeSelf == false)
            return;


        PoolInfo info = default;
        if (_poolObjectDic.ContainsKey(instance.gameObject) == true)
        {
            info = _poolObjectDic[instance.gameObject];
        }
        else
        {
            info = FindPool(instance.gameObject);
        }
        instance.transform.position = info.Prefab.transform.position;
        instance.transform.rotation = info.Prefab.transform.rotation;
        instance.transform.localScale = info.Prefab.transform.localScale;
        instance.transform.SetParent(info.Parent);
        instance.gameObject.SetActive(false);
        info.Pool.Enqueue(instance.gameObject);
    }
    public static void ReturnPool(GameObject instance, float delay)
    {
        if (instance.gameObject.activeSelf == false)
            return;
        Instance.StartCoroutine(ReturnRoutine(instance, delay));
    }
    public static void ReturnPool<T>(T instance, float delay) where T : Component
    {
        if (instance.gameObject.activeSelf == false)
            return;
        Instance.StartCoroutine(ReturnRoutine(instance, delay));
    }
    static IEnumerator ReturnRoutine(GameObject instance, float delay)
    {
        yield return delay.GetDelay();
        if (instance.activeSelf == false)
            yield break;

        ReturnPool(instance);
    }
    static IEnumerator ReturnRoutine<T>(T instance, float delay) where T : Component
    {
        yield return delay.GetDelay();
        if (instance.gameObject.activeSelf == false)
            yield break;

        ReturnPool(instance);
    }
    #endregion
    #region GetAutoPool
    public static void GetPool(GameObject prefab, float intervalTime,float returnDelay,ref Coroutine coroutine)
    {
        CreateObjectPool();
        if (coroutine == null)
        {
            coroutine = Instance.StartCoroutine(Instance.GetAutoPoolRoutine(prefab, intervalTime, returnDelay));
        }
    }
    public static void GetPool(GameObject prefab, Transform transform,float intervalTime, float returnDelay, ref Coroutine coroutine)
    {
        CreateObjectPool();
        if (coroutine == null)
        {
            coroutine = Instance.StartCoroutine(Instance.GetAutoPoolRoutine(prefab, transform,false,intervalTime, returnDelay));
        }
    }
    public static void GetPool(GameObject prefab, Transform transform, bool worldPositionStay, float intervalTime, float returnDelay, ref Coroutine coroutine)
    {
        CreateObjectPool();
        if (coroutine == null)
        {
            coroutine = Instance.StartCoroutine(Instance.GetAutoPoolRoutine(prefab, transform, worldPositionStay, intervalTime, returnDelay));
        }
    }
    public static void GetPool(GameObject prefab, Vector3 pos , Quaternion rot,float intervalTime, float returnDelay, ref Coroutine coroutine)
    {
        CreateObjectPool();
        if (coroutine == null)
        {
            coroutine = Instance.StartCoroutine(Instance.GetAutoPoolRoutine(prefab, pos, rot,intervalTime, returnDelay));
        }
    }
    public static void GetPool(GameObject prefab, float intervalTime, float returnDelay, float duration)
    {
        CreateObjectPool();
        Coroutine  coroutine = Instance.StartCoroutine(Instance.GetAutoPoolRoutine(prefab, intervalTime, returnDelay));
        Instance.StartCoroutine(Instance.GetAutoPoolDurationRoutine(coroutine,duration));
    }
    public static void GetPool(GameObject prefab, Vector3 pos, Quaternion rot, float intervalTime, float returnDelay, float duration)
    {
        CreateObjectPool();
        Coroutine coroutine = Instance.StartCoroutine(Instance.GetAutoPoolRoutine(prefab, pos, rot,intervalTime, returnDelay));
        Instance.StartCoroutine(Instance.GetAutoPoolDurationRoutine(coroutine, duration));
    }
    public static void ReturnPool(ref Coroutine coroutine)
    {
        if(coroutine != null)
        {
            Instance.StopCoroutine(coroutine);
            coroutine = null;
        }
    }
    IEnumerator GetAutoPoolRoutine(GameObject prefab, float intervalTime, float returnDelay)
    {
        while (true)
        {
            GetPool(prefab, returnDelay);
            yield return intervalTime.GetDelay();
        }
    }
    IEnumerator GetAutoPoolRoutine(GameObject prefab, Transform transform, bool worldPositionStay, float intervalTime, float returnDelay)
    {
        while (true)
        {
            GetPool(prefab, transform, worldPositionStay, returnDelay);
            yield return intervalTime.GetDelay();
        }
    }
    IEnumerator GetAutoPoolRoutine(GameObject prefab, Vector3 pos, Quaternion rot, float intervalTime, float returnDelay)
    {
        while (true)
        {
            GetPool(prefab, pos, rot, returnDelay);
            yield return intervalTime.GetDelay();
        }
    }
    IEnumerator GetAutoPoolDurationRoutine(Coroutine coroutine, float duration)
    {
        yield return duration.GetDelay();
        ReturnPool(ref coroutine);
    }
    #endregion
    private static PoolInfo FindPool(GameObject poolPrefab)
    {
        CreateObjectPool();
        PoolInfo pool = default;
        if (_poolDic.ContainsKey(poolPrefab) == false)
        {
            Transform newParent = new GameObject(poolPrefab.name).transform;
            newParent.SetParent(thisTransform, true); // parent
            Queue<GameObject> newPool = new Queue<GameObject>(); // pool
            PoolInfo newPoolInfo = GetPoolInfo(newPool, poolPrefab, newParent);
            _poolDic.Add(poolPrefab, newPoolInfo);
        }
        pool = _poolDic[poolPrefab];
        return pool;
    }
    private static PoolInfo GetPoolInfo(Queue<GameObject> pool, GameObject prefab, Transform parent)
    {
        PoolInfo info = new PoolInfo();
        info.Pool = pool;
        info.Parent = parent;
        info.Prefab = prefab;
        return info;
    }
}