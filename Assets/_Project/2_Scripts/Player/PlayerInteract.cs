using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] Camera camera;
    [SerializeField] float maxDistance = 3.0f;
    [SerializeField] LayerMask layerMask;
    void Update()
    {
        // 카메라 화면 기준 정중앙에서 직선으로 뻗는 ray 생성
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask))
            // ray 내부에 광선 시작점 정보 담겨 있음. ray를 maxDistance 크기로 쏴서 layerMask에 해당하는 오브젝트에 닿으면 true, 닿은 hit 대상 정보 hit로 들어감.
        {
            // hit.collider은 Ray가 부딪힌 콜라이더 그자체. TryGetComponent로 콜라이더 컴포넌트가 부착된 오브젝트에서 <IClickable> 를 구현한 컴포넌트가 있는지 확인
            if (hit.collider.TryGetComponent<IClickable>(out var clickable) && clickable.CanInteract())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    clickable.OnClicked();
                }
            }
            Debug.DrawLine(ray.origin, hit.point, Color.green);
        }
        else
        {
            Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red);
        }
    }
}
