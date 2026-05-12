using UnityEngine;
using System.Collections;
public class PipeBurstUnit : MonoBehaviour
{
    [SerializeField] private ParticleSystem waterBurstVFX;
    [SerializeField] private ParticleSystem groundSplashVFX;
    [SerializeField] private AudioSource burstAudio;
    [SerializeField] private float groundSplashDelay = 0.15f;

    void Awake()
    {
        // 시작할 때 파티클 자식 오브젝트 비활성화
        waterBurstVFX.gameObject.SetActive(false);
        groundSplashVFX.gameObject.SetActive(false);
    }
    public void Trigger()
    {
        // 자식 오브젝트 활성화
        waterBurstVFX.gameObject.SetActive(true);
        groundSplashVFX.gameObject.SetActive(true);
        burstAudio.Play();
        waterBurstVFX.Play();
        StartCoroutine(PlayGroundSplashDelayed());
    }

    private IEnumerator PlayGroundSplashDelayed()
    {
        yield return new WaitForSeconds(groundSplashDelay);
        groundSplashVFX.Play();
    }

    public void StopBurst()
    {
        burstAudio.Stop();
        waterBurstVFX.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        groundSplashVFX.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
    public void ResetBurst()
    {
        StopAllCoroutines();

        burstAudio.Stop();

        waterBurstVFX.Stop();
        waterBurstVFX.Clear();   // 잔여 파티클까지 제거
        waterBurstVFX.gameObject.SetActive(false);

        groundSplashVFX.Stop();
        groundSplashVFX.Clear();
        groundSplashVFX.gameObject.SetActive(false);
    }
    [ContextMenu("Test Trigger")]
    private void TestTrigger() => Trigger();
}