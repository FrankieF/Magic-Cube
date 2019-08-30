using UnityEngine;

public class PivotController : MonoBehaviour
{
    private void Awake()
    {
        ServiceManager.Register(this, GetComponent<PivotController>());
    }
}
