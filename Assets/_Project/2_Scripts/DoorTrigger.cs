using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoorTrigger : MonoBehaviour
{
    [Header("References")]
    public Animator doorAnimator;

    [Tooltip("Animator Controller의 Bool 파라미터 이름")]
    public string isOpenParamName = "IsOpen";

    [Header("Audio (Optional)")]
    public AudioSource doorAudio;

    [Header("Trigger")]
    public string playerTag = "Player";

    void Reset()
    {
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (doorAnimator != null)
            doorAnimator.SetBool(isOpenParamName, true);

        if (doorAudio != null && doorAudio.clip != null) doorAudio.Play();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (doorAnimator != null)
            doorAnimator.SetBool(isOpenParamName, false);

        if (doorAudio != null && doorAudio.clip != null) doorAudio.Play();
    }
}