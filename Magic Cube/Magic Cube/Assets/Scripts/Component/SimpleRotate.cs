using UnityEngine;

public class SimpleRotate : MonoBehaviour
{
    [SerializeField] private float _Speed = 1;
    [SerializeField] public Vector3 _Axis;

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position, _Axis, _Speed * Time.deltaTime);
    }
}
