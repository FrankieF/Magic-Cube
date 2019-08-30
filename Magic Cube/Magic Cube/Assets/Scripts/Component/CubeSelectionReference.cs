using UnityEngine;

public class CubeSelectionReference : MonoBehaviour
{
    public CubeInfo Cube;
    
    public enum Direction {Right, Left, Forward, Back, Up, Down}

    public Direction direction;

    private void Start()
    {
        RaycastHit hit;
        Vector3 dir = Vector3.zero;
        switch (direction)
        {
            case Direction.Right:
                dir = Vector3.right;
                break;
            case Direction.Left:
                dir = Vector3.left;
                break;
            case Direction.Forward:
                dir = Vector3.forward;
                break;
            case Direction.Back:
                dir = Vector3.back;
                break;
            case Direction.Up:
                dir = Vector3.up;
                break;
            case Direction.Down:
                dir = Vector3.down;
                break;
        }
        if (Physics.Raycast(transform.position, dir, out hit, 1))
        {
            Cube = hit.collider.gameObject.GetComponent<CubeInfo>();
        }
    }
}
