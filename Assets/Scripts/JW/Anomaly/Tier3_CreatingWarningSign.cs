using UnityEngine;

public class Tier3_CreatingWarningSign : AnomalyBase
{
    public GameObject warningSign;
    public override void Activate()
    {
        warningSign.SetActive(true);
    }

    public override void Deactivate() 
    {
        warningSign.SetActive(false);
    }

    public override bool IsAnomaly() => false;
}
