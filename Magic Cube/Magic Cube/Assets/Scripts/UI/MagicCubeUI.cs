using UnityEngine;

public class MagicCubeUI : MonoBehaviour
{
    [SerializeField] private Transform _cube;
    private CubeInfo[] _cubes;
    private Vector3 _rotationDirection;

    private void Awake()
    {
        BuildCubeArray();
    }
    private void BuildCubeArray()
    {
        _cubes = new CubeInfo[_cube.childCount];
        for (int i = 0; i < _cube.childCount; i++)
        {
            _cubes[i] = _cube.GetChild(i).gameObject.GetComponent<CubeInfo>();
        }
    }
    
    public CubeInfo[] GetCubes()
    {
        return _cubes;
    }
}
