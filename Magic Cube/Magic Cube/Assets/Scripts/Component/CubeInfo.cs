using UnityEngine;

public class CubeInfo : MonoBehaviour
{
    public CubeInfo CenterCube;
    public SelectionTriggerController XSelection;
    public SelectionTriggerController YSelection;
    public SelectionTriggerController ZSelection;
    public Quaternion InitialRotation;

    private void Awake()
    {
        InitialRotation = transform.localRotation;
    }

    public bool IsSolved()
    {
        var correctRotation = (CenterCube.transform.localRotation * Quaternion.Inverse(CenterCube.InitialRotation)).eulerAngles;
        var currentRotation = (transform.localRotation * Quaternion.Inverse(InitialRotation)).eulerAngles;
        return correctRotation == currentRotation;
    }
}
