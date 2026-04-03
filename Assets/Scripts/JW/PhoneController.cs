using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
public class PhoneController : MonoBehaviour
{
    [SerializeField] private Vector3 hiddenOffset = new Vector3(0, -0.3f, 0);
    [SerializeField] private float animDuration = 0.3f;

    private Vector3 shownPos;
    private bool isVisible = false;

    void Start()
    {
        shownPos = transform.localPosition;
        transform.localPosition = shownPos + hiddenOffset;

    }

    void Update()
    {
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            Debug.Log("Clicked");
            if (isVisible) HidePhone();
            else ShowPhone();
        }
    }

    private void ShowPhone()
    {
        isVisible = true;
        StopAllCoroutines();
        StartCoroutine(MovePhone(shownPos + hiddenOffset, shownPos));
    }

    private void HidePhone()
    {
        isVisible = false;
        StopAllCoroutines();
        StartCoroutine(MovePhone(shownPos, shownPos + hiddenOffset, deactivateOnEnd: false));
    }

    private IEnumerator MovePhone(Vector3 from, Vector3 to, bool deactivateOnEnd = false)
    {
        float elapsed = 0f;
        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / animDuration); // 부드러운 이징
            transform.localPosition = Vector3.Lerp(from, to, t);
            yield return null;
        }
        transform.localPosition = to;
        if (deactivateOnEnd) gameObject.SetActive(false);
    }
}