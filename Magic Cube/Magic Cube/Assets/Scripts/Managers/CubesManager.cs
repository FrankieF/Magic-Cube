using System;
using System.Collections.Generic;
using UnityEngine;

public class CubesManager : MonoBehaviour
{
    [SerializeField] private Transform _CubeParent;

    private bool _currentCubeNew = false;
    private int _currentCubeSize = -1;
    private string _activeCube = string.Empty;
    private Dictionary<string, GameObject> _cubesPool = new Dictionary<string, GameObject>(); 
    
    private void Awake()
    {
        ServiceManager.Register(this, GetComponent<CubesManager>());
        GameEventsManager.Register(GameEventConstants.ON_RETURN_TO_MENU, ReturnCubeToPool);
    }

    private void OnDestroy()
    {
        ServiceManager.Unregister(this);
        GameEventsManager.Unregister(GameEventConstants.ON_RETURN_TO_MENU, ReturnCubeToPool);
    }

    public int GetCurrentCubeSize()
    {
        return _currentCubeSize;
    }

    public bool IsCurrentCubeNew()
    {
        return _currentCubeNew;
    }

    public CubeInfo[] GetCubes()
    {
        if (!_cubesPool.ContainsKey(_activeCube))
        {
            LogManager.LogError($"Current active cube {_activeCube} is not in pool. Returning null for GetCubes().");
            return null;
        }
        return _cubesPool[_activeCube].GetComponent<MagicCubeUI>().GetCubes();
    }
    
    private void ReturnCubeToPool()
    {
        if (_cubesPool.ContainsKey(_activeCube))
        {
            _cubesPool[_activeCube].SetActive(false);
        }
        else
        {
            LogManager.LogError($"Active cube {_activeCube} has reference to non active cube! Unable to return current cube to pool. Destroying pool to clear memory.");
            foreach (var cube in _cubesPool)
            {
                Destroy(cube.Value);
            }
            _cubesPool.Clear();
        }
    }

    public void LoadCube(int cubeSize, bool newGame = true)
    {
        var key = string.Format(Consts.CUBE_PREFAB_NAME, cubeSize);
        if (!_cubesPool.ContainsKey(key) || newGame)
        {
            if (_cubesPool.ContainsKey(key))
            {
                var cube = _cubesPool[key];
                DestroyImmediate(cube);
                _cubesPool.Remove(key);
            }

            var prefabPath = string.Format(Consts.CUBE_PREFAB_PATH, cubeSize);
            var prefab = Resources.Load<GameObject>(prefabPath);
            if (prefab != null)
            {
                var cube = Instantiate(prefab, _CubeParent);
                _activeCube = key;
                _cubesPool.Add(key, cube);
                _currentCubeSize = cubeSize;
                _currentCubeNew = true;
            }
            else
            {
                var path = string.Format(Consts.CUBE_PREFAB_PATH, cubeSize);
                LogManager.LogError($"Failed to load {path}");
            }
        }
        else
        {
            _activeCube = key;
            var cube = _cubesPool[key];
            cube.SetActive(true);
            cube.transform.SetParent(_CubeParent, false);
            _currentCubeSize = cubeSize;
            _currentCubeNew = false;
        }
    }
}
