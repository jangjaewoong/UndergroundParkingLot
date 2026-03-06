// EngineAmbience.cs - 카메라에 붙이기
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EmissionMaterialEntry
{
    public Material material;
    public float delay = 0f; // 0이면 바로 끄기
}
public class EngineAmbience : MonoBehaviour
{
    [Header("진동 설정")]
    [SerializeField] private float amplitude = 0.02f;
    [SerializeField] private float frequency = 10f;

    [Header("소리 설정")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip engineStandClip;
    [SerializeField] private AudioClip engineStopClip;

    [SerializeField] private EmissionMaterialEntry[] emissionEntries;
    [SerializeField] private float emfadeDuration = 1.5f;
    [SerializeField] private float enginefadeDuration = 3.5f;


    private Dictionary<Material, Color> originalColors = new Dictionary<Material, Color>();
    private Vector3 originalPos;

    void Start()
    {
        // 원본 색상 저장 후 켜기
        foreach (var entry in emissionEntries)
        {
            if (entry.material == null) continue;
            originalColors[entry.material] = entry.material.GetColor("_EmissionColor");
            entry.material.EnableKeyword("_EMISSION");
        }
        originalPos = transform.localPosition;
        StartCoroutine(ShakeRoutine());
        PlayEngineStandSound();
    }

    // 플레이 종료 시 원본 복구
    void OnDisable()
    {
        foreach (var entry in emissionEntries)
        {
            if (entry.material == null) continue;
            if (originalColors.TryGetValue(entry.material, out Color original))
            {
                entry.material.SetColor("_EmissionColor", original);
                entry.material.EnableKeyword("_EMISSION");
            }
        }
    }
    private IEnumerator ShakeRoutine()
    {
        while (true)
        {
            float x = Mathf.Sin(Time.time * frequency) * amplitude;
            float y = Mathf.Sin(Time.time * frequency * 1.3f) * amplitude;
            transform.localPosition = originalPos + new Vector3(x, y, 0);
            yield return null;
        }
    }

    private void PlayEngineStandSound()
    {
        if (audioSource == null || engineStandClip == null) return;
        audioSource.clip = engineStandClip;
        audioSource.loop = true;
        audioSource.Play();
    }

    private void PlayEngineStopSound()
    {
        if (audioSource == null || engineStandClip == null) return;
        audioSource.clip = engineStopClip;
        audioSource.loop = false;
        audioSource.Play();
    }

    private IEnumerator FadeOutEmission(EmissionMaterialEntry entry)
    {
        yield return new WaitForSeconds(entry.delay);

        Material mat = entry.material;

        // 저장된 원본 색상으로 페이드 시작
        Color originalColor = originalColors.ContainsKey(mat)
            ? originalColors[mat]
            : mat.GetColor("_EmissionColor");

        float elapsed = 0f;
        while (elapsed < emfadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = 1f - Mathf.Clamp01(elapsed / emfadeDuration);
            mat.SetColor("_EmissionColor", originalColor * t);
            yield return null;
        }

        mat.SetColor("_EmissionColor", Color.black);
        mat.DisableKeyword("_EMISSION");
    }
    private IEnumerator FadeOutShake()
    {
        float startAmplitude = amplitude;
        float elapsed = 0f;

        while (elapsed < enginefadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = 1f - Mathf.Clamp01(elapsed / enginefadeDuration);
            float currentAmplitude = startAmplitude * t;

            float x = Mathf.Sin(Time.time * frequency) * currentAmplitude;
            float y = Mathf.Sin(Time.time * frequency * 1.3f) * currentAmplitude;
            transform.localPosition = originalPos + new Vector3(x, y, 0);

            yield return null;
        }

        // 완전히 멈추고 원래 위치로
        transform.localPosition = originalPos;
    }

    // GameStart 버튼에서 호출
    public void StopAll()
    {
        StopAllCoroutines();
        transform.localPosition = originalPos;
        audioSource.Stop();
        PlayEngineStopSound();
        StartCoroutine(FadeOutShake());
        foreach (var entry in emissionEntries)
            StartCoroutine(FadeOutEmission(entry));
        
    }
}