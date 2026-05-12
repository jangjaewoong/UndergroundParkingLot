using UnityEngine;

public class EleButton : MonoBehaviour, IClickable
{
    [SerializeField] private ElevatorController eleCon;
    public void OnClicked()
    {
        eleCon.requestDeparture();
        return;
    }
    public bool CanInteract()
    {
        return true;
    }
}
