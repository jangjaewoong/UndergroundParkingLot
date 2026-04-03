using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [Header("차량 프리팹 (10종)")]
    public GameObject[] carPrefabs;

    [Header("배치 설정")]
    public int minCars = 30;
    public int maxCars = 60;

    [Header("스폰 포인트 부모 오브젝트")]
    public Transform spawnPointParent;

    // 차종별 풀
    private Dictionary<int, Queue<GameObject>> pool = new();

    // 현재 활성화된 차량 목록
    private List<GameObject> activeCars = new();

    // 풀 부모 오브젝트
    private Transform poolParent;

    void Awake()
    {
        // 풀 컨테이너 생성
        GameObject poolObj = new GameObject("CarPool");
        poolParent = poolObj.transform;
        poolParent.SetParent(transform);

        // 차종별 풀 초기화
        for (int i = 0; i < carPrefabs.Length; i++)
        {
            pool[i] = new Queue<GameObject>();
        }

        // 최대 대수만큼 미리 생성
        PrewarmPool();
    }

    void PrewarmPool()
    {
        int perType = Mathf.CeilToInt((float)maxCars / carPrefabs.Length);

        for (int i = 0; i < carPrefabs.Length; i++)
        {
            for (int j = 0; j < perType; j++)
            {
                GameObject car = Instantiate(carPrefabs[i], poolParent);
                car.SetActive(false);
                pool[i].Enqueue(car);
            }
        }

        Debug.Log($"CarSpawner: 풀 생성 완료 ({carPrefabs.Length}종 x {perType}대 = {carPrefabs.Length * perType}대)");
    }

    GameObject GetCarFromPool(int carTypeIndex)
    {
        if (pool[carTypeIndex].Count > 0)
        {
            GameObject car = pool[carTypeIndex].Dequeue();
            car.SetActive(true);
            return car;
        }

        GameObject newCar = Instantiate(carPrefabs[carTypeIndex], poolParent);
        return newCar;
    }

    void Start()
    {
        RespawnCars();
    }

    /// <summary>
    /// 외부에서 호출: 기존 차량 전부 회수 후 랜덤 재배치
    /// </summary>
    public void RespawnCars()
    {
        ReturnAllCars();
        SpawnCars();
    }

    void ReturnAllCars()
    {
        foreach (var car in activeCars)
        {
            if (car == null) continue;

            int typeIndex = GetCarTypeIndex(car);
            car.SetActive(false);
            car.transform.SetParent(poolParent);
            pool[typeIndex].Enqueue(car);
        }

        activeCars.Clear();
    }

    void SpawnCars()
    {
        if (carPrefabs == null || carPrefabs.Length == 0)
        {
            Debug.LogError("CarSpawner: 차량 프리팹이 없습니다!");
            return;
        }

        List<Transform> spawnPointList = new List<Transform>();
        foreach (Transform group in spawnPointParent)
        {
            foreach (Transform point in group)
            {
                spawnPointList.Add(point);
            }
        }
        Transform[] spawnPoints = spawnPointList.ToArray();
        int totalPoints = spawnPoints.Length;

        for (int i = spawnPoints.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (spawnPoints[i], spawnPoints[j]) = (spawnPoints[j], spawnPoints[i]);
        }

        // 2. 랜덤 대수
        int carCount = Random.Range(minCars, maxCars + 1);
        carCount = Mathf.Min(carCount, totalPoints);

        // 3. 차종 균등 분배 리스트 생성
        List<int> carTypeList = new List<int>();

        int perType = carCount / carPrefabs.Length;
        int remainder = carCount % carPrefabs.Length;

        for (int i = 0; i < carPrefabs.Length; i++)
        {
            int count = perType + (i < remainder ? 1 : 0);
            for (int j = 0; j < count; j++)
                carTypeList.Add(i);
        }

        // 차종 리스트 셔플
        for (int i = carTypeList.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (carTypeList[i], carTypeList[j]) = (carTypeList[j], carTypeList[i]);
        }

        // 4. 배치
        for (int i = 0; i < carCount; i++)
        {
            int carTypeIndex = carTypeList[i];

            GameObject car = GetCarFromPool(carTypeIndex);
            car.transform.position = spawnPoints[i].position;
            car.transform.rotation = spawnPoints[i].rotation * Quaternion.Euler(-90f,0f,0f);

            CarPoolID poolID = car.GetComponent<CarPoolID>();
            if (poolID == null)
                poolID = car.AddComponent<CarPoolID>();
            poolID.typeIndex = carTypeIndex;

            activeCars.Add(car);
        }

        Debug.Log($"CarSpawner: {carCount}대 배치 완료 (균등 분배 + 풀링)");
    }

    int GetCarTypeIndex(GameObject car)
    {
        CarPoolID poolID = car.GetComponent<CarPoolID>();
        if (poolID != null)
            return poolID.typeIndex;
        return 0;
    }
}

/// <summary>
/// 차량에 붙는 컴포넌트. 풀 반환 시 차종 식별용.
/// </summary>
public class CarPoolID : MonoBehaviour
{
    [HideInInspector]
    public int typeIndex;
}