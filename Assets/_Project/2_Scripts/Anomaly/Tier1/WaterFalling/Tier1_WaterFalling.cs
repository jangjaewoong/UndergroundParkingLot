using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;

public class Tier1_WaterFalling : AnomalyBase
{
    [Header("References")]
    public GameObject WaterSurfacePlane;
    public Volume globalVolume;
    public Volume underwaterVolume;

    [Header("Audio")]
    public AudioSource waterFlowAudio;      // 물 흐르는 소리 (loop)
    [Tooltip("이 높이 이하면 물소리 볼륨 0")]
    public float waterSoundMinY = 0f;

    [Tooltip("이 높이 이상이면 물소리 최대 볼륨")]
    public float waterSoundMaxY = 1.5f;

    [Tooltip("물소리 최대 볼륨 (0~1)")]
    [Range(0f, 1f)]
    public float waterSoundMaxVolume = 1f;

    [Header("Timer UI")]
    public GameObject timerUIRoot;           // 타이머 UI 부모 (Canvas 하위)
    public TMP_Text timerText;               // 남은 시간 표시
    public float timerDuration = 30f;        // 총 타이머 시간

    [Header("Water Rise")]
    public float maxHeight = 2f;
    public float dieHeight = 0.9f;
    public float waterStartDelay = 2f;       // 물 상승 시작 전 대기

    private Vector3 initialPlanePosition;

    private Camera mainCam;
    private bool hasTriggeredDeath = false;
    private bool isRunning = false;          // 이상현상이 실제 진행 중인지
    private bool waterSoundPlaying = false;  // 물소리 상태 추적

    void Awake()
    {
        initialPlanePosition = WaterSurfacePlane.transform.position;
        underwaterVolume.gameObject.SetActive(false);

        mainCam = Camera.main;

        // UI 초기 비활성
        if (timerUIRoot != null) timerUIRoot.SetActive(false);
    }

    void Update()
    {
        // 물소리 상태 관리 (요청 1)
        UpdateWaterSound();

        // 진행 중이 아니면 침수 판정 스킵
        if (!isRunning) return;

        // 침수 판정
        if (mainCam == null)
        {
            mainCam = Camera.main;
            if (mainCam == null) return;
        }

        float waterSurfaceY = WaterSurfacePlane.transform.position.y;
        float nearClipY = mainCam.transform.position.y
                        + mainCam.transform.forward.y * mainCam.nearClipPlane;

        bool isUnderwater = nearClipY < waterSurfaceY;

        Debug.Log(nearClipY - waterSurfaceY);
        if (isUnderwater)
        {
            Debug.Log("under the sea");
            globalVolume.gameObject.SetActive(false);
            underwaterVolume.gameObject.SetActive(true);

            if (!hasTriggeredDeath)
            {
                hasTriggeredDeath = true;
                Vector3 pos = WaterSurfacePlane.transform.position;
                WaterSurfacePlane.transform.position = new Vector3(pos.x, maxHeight, pos.z);
               
                
            }
        }
        else
        {
            underwaterVolume.gameObject.SetActive(false);
            globalVolume.gameObject.SetActive(true);
        }
    }

    // ============================================================
    // 요청 1: 물소리 - 수면이 임계값 이상이면 재생, 아니면 정지
    // ============================================================
    private void UpdateWaterSound()
    {
        if (waterFlowAudio == null || WaterSurfacePlane == null) return;

        // 사망 후엔 즉시 정지
        if (hasTriggeredDeath)
        {
            if (waterSoundPlaying)
            {
                waterFlowAudio.Stop();
                waterSoundPlaying = false;
            }
            return;
        }

        // 이상현상 진행 중이면 항상 재생 (볼륨은 높이 비례)
        if (isRunning)
        {
            // 처음 진입 시 재생 시작 (볼륨 0부터)
            if (!waterSoundPlaying)
            {
                waterFlowAudio.volume = 0f;
                waterFlowAudio.Play();
                waterSoundPlaying = true;
            }

            // 수면 높이 → 볼륨 매핑
            float currentY = WaterSurfacePlane.transform.position.y;
            float t = Mathf.InverseLerp(waterSoundMinY, waterSoundMaxY, currentY);
            waterFlowAudio.volume = Mathf.Lerp(0f, waterSoundMaxVolume, t);
        }
        else
        {
            // 진행 중 아니면 정지
            if (waterSoundPlaying)
            {
                waterFlowAudio.Stop();
                waterSoundPlaying = false;
            }
        }
    }

    // ============================================================
    // Activate: 이상현상 "세팅"만. 아직 진행하지 않음.
    // ============================================================
    public override void Activate()
    {
        hasTriggeredDeath = false;
        isRunning = false;

        // 물과 파티클은 아직 시작하지 않음
        // 트리거 박스에 닿을 때까지 대기
        SetContentActive(true);  // ⭐ 추가
    }

    // ============================================================
    // 요청 3: 트리거 박스에 플레이어가 닿았을 때 호출
    // 실제 이상현상 진행 시작
    // ============================================================
    //public override void OnTriggered()
    //{
    //    if (isRunning) return;
    //    isRunning = true;
    //    // 타이머 UI + 물 상승 동시 시작 (요청 2)
    //    StartCoroutine(RunAnomaly());
    //}

    public void StartWaterRising()
    {
        if (isRunning) return;
        isRunning = true;
        StartCoroutine(RunAnomaly());
    }

    // ============================================================
    // 요청 2: 30초 타이머 + 물 상승 비율 맞춤
    // 타이머가 0이 되는 순간 물이 DieHeight에 도달
    // ============================================================
    private IEnumerator RunAnomaly()
    {
        // 시작 전 대기
        yield return new WaitForSeconds(waterStartDelay);

        // 타이머 UI 활성
        if (timerUIRoot != null) timerUIRoot.SetActive(true);

        // 물 상승 속도 = 목표높이 / 타이머시간 (요청 2 핵심)
        float riseSpeed = dieHeight / timerDuration;

        float remaining = timerDuration;
        float startY = WaterSurfacePlane.transform.position.y;

        while (remaining > 0f)
        {
            remaining -= Time.deltaTime;

            // 타이머 UI 업데이트
            if (timerText != null)
                timerText.text = Mathf.CeilToInt(remaining).ToString();

            // 물 상승 (시간 비례)
            if (WaterSurfacePlane.transform.position.y < startY + dieHeight)
            {
                WaterSurfacePlane.transform.position += Vector3.up * riseSpeed * Time.deltaTime;
            }

            yield return null;
        }

        // 타이머 종료 → 사망 확정
        if (timerText != null) timerText.text = "0";
        if (!hasTriggeredDeath)
        {
            // 타이머 UI 비활성
            if (timerUIRoot != null) timerUIRoot.SetActive(false);
            globalVolume.gameObject.SetActive(false);
            underwaterVolume.gameObject.SetActive(true);
            StopAllCoroutines();
            
            hasTriggeredDeath = true;
            Vector3 pos = WaterSurfacePlane.transform.position;
            WaterSurfacePlane.transform.position = new Vector3(pos.x, maxHeight, pos.z);
             
            
        }
    }

    // ============================================================
    public override void Deactivate()
    {
        isRunning = false;
        hasTriggeredDeath = false;


        StopAllCoroutines();
        underwaterVolume.gameObject.SetActive(false);
        globalVolume.gameObject.SetActive(true);
        // 물 위치 초기화
        WaterSurfacePlane.transform.position = initialPlanePosition;

        // 물소리 정지
        if (waterFlowAudio != null && waterFlowAudio.isPlaying)
            waterFlowAudio.Stop();
        waterSoundPlaying = false;

        // 타이머 UI 비활성
        if (timerUIRoot != null) timerUIRoot.SetActive(false);

     

        SetContentActive(false);  // ⭐ 추가

    }

    public override bool IsAnomaly() => true;
}