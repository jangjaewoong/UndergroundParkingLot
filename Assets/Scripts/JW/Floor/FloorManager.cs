using UnityEngine;

public class FloorManager : MonoBehaviour
{
    public AnomalyManager anomalyManager;
    public int currentFloor = 7;

    void Start()
    {
        anomalyManager.SetupFloor();
    }

    public void OnPlayerChoice(bool tookElevator)
    {
        bool correct = anomalyManager.CheckPlayerChoice(tookElevator);

        if (correct)
        {
            currentFloor--;
            if (currentFloor < 1) Debug.Log("CLEAR!\n");
            else NextFloor();
        }
        else
        {
            ResetToB7();
        }
    }

    void NextFloor()
    {
        // 텔레포트
        anomalyManager.SetupFloor(); // 새 이상현상 세팅
    }

    void ResetToB7()
    {
        currentFloor = 7;
        anomalyManager.SetupFloor();
    }
}