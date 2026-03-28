using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 이상현상: 애기 웃음소리
/// - 멀리서 들리다가 점점 가까워짐
/// - 거리에 따라 Vignette 빨개짐/돌아옴
/// </summary>
public class BabyLaughAnomaly : MonoBehaviour
{
    [Header("사운드 설정")]
    public AudioClip laughClip;
    public float minVolume = 0.3f;
    public float maxVolume = 1.0f;
    public float duration = 8f;

    [Header("거리 설정")]
    public float startDistance = 20f;
    public float endDistance = 2f;

    [Header("Vignette 설정")]
    public Volume postProcessVolume;
    public float maxVignetteIntensity = 0.7f;

    private AudioSource audioSource;
    private Transform player;
    private Vignette vignette;

    // 원래 Vignette 값 저장
    private float originalIntensity;
    private Color originalColor;

    private bool isActive = false;
    private float timer = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        // Vignette 가져오고 원래 값 저장
        if (postProcessVolume != null)
        {
            postProcessVolume.profile.TryGet(out vignette);
            if (vignette != null)
            {
                originalIntensity = vignette.intensity.value;
                originalColor = vignette.color.value;
            }
        }

        if (audioSource != null)
        {
            audioSource.spatialBlend = 0f;
            audioSource.loop = true;
            audioSource.volume = minVolume;
            audioSource.Stop();
        }
    }

    void Update()
    {
        if (!isActive || player == null) return;

        timer += Time.deltaTime;

        // 소리 위치를 플레이어 쪽으로 점점 이동
        float timerProgress = Mathf.Clamp01(timer / duration);
        Vector3 direction = (player.position - transform.position).normalized;
        float currentDist = Mathf.Lerp(startDistance, endDistance, timerProgress);
        transform.position = player.position - direction * currentDist;

        // 거리 기반 progress (가까울수록 1, 멀수록 0)
        float dist = Vector3.Distance(transform.position, player.position);
        float distProgress = Mathf.Clamp01(1f - (dist - endDistance) / (startDistance - endDistance));

        // 볼륨 거리에 따라 조절
        if (audioSource != null)
            audioSource.volume = Mathf.Lerp(minVolume, maxVolume, distProgress);

        // Vignette 거리에 따라 변화
        if (vignette != null)
        {
            vignette.intensity.overrideState = true;
            vignette.color.overrideState = true;
            vignette.intensity.value = Mathf.Lerp(originalIntensity, maxVignetteIntensity, distProgress);
            vignette.color.value = Color.Lerp(originalColor, Color.red, distProgress);
        }

        if (timer >= duration)
            EndAnomaly();
    }

    void EndAnomaly()
    {
        isActive = false;

        if (audioSource != null)
            audioSource.Stop();

        RestoreVignette();
    }

    void RestoreVignette()
    {
        if (vignette != null)
        {
            vignette.intensity.value = originalIntensity;
            vignette.color.value = originalColor;
        }
    }

    public void Activate()
    {
        if (player == null) return;

        isActive = true;
        timer = 0f;

        Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
        transform.position = player.position + randomDir * startDistance;

        if (audioSource != null)
        {
            audioSource.clip = laughClip;
            audioSource.volume = minVolume;
            audioSource.loop = true;
            audioSource.Play();
        }

        Debug.Log("[BabyLaughAnomaly] 애기 웃음소리 시작");
    }

    public void ResetAnomaly()
    {
        isActive = false;
        timer = 0f;

        if (audioSource != null)
            audioSource.Stop();

        RestoreVignette();
    }
}