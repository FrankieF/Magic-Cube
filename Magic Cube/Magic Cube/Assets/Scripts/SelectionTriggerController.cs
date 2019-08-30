using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionTriggerController : MonoBehaviour
{
    [SerializeField] private float _ColliderOffset = 0.25f;
    [SerializeField] public bool _XAxisSelection;
    [SerializeField] public bool _YAxisSelection;
    [SerializeField] private bool _UseCustomVector;
    [SerializeField] private Vector3 _PhysicsHalfExtents;
    [SerializeField] public  LayerMask _mask;

    private Vector3 Direction;
    private BoxCollider _collider;
    private List<GameObject> _cubes = new List<GameObject>();
    
    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
        if (_collider == null)
        {
            return;
        }
        var colliders = Physics.OverlapBox(transform.position, _UseCustomVector ? _PhysicsHalfExtents : _collider.size * _ColliderOffset, transform.rotation, _mask);
        foreach (var c in colliders)
        {
            if (_XAxisSelection)
            {
                c.GetComponent<CubeInfo>().XSelection = this;
            }
            else if (_YAxisSelection)
            {
                c.GetComponent<CubeInfo>().YSelection = this;
            }
            else
            {
                if (c.GetComponent<CubeInfo>() == null)
                {
                    Debug.LogError("");
                }
                c.GetComponent<CubeInfo>().ZSelection = this;
            }
        }
        if (_collider == null)
        {
            TriggerManager.RegisterEmptyTrigger(this);
        }
    }

    private void Start()
    {
    }

    public List<GameObject> GetListOfCubesToRotate()
    {
        _cubes.Clear();
        if (_collider == null)
        {
            LogManager.LogError($"Selection trigger {name} BoxCollider is null. Returning empty list.");
            return _cubes;
        }
        var colliders = Physics.OverlapBox(transform.position, _UseCustomVector ? _PhysicsHalfExtents : _collider.size * _ColliderOffset, transform.rotation, _mask);
        var set = new HashSet<Collider>(colliders);
        foreach (var c in colliders)
        {
            _cubes.Add(c.gameObject);
        }
        return _cubes;
    }

    public IEnumerator Rotate(List<GameObject> cubes, Vector3 axis, float rotateTime, Action onComplete)
    {
        if (cubes == null || cubes.Count < 1)
        {
            yield break;
        }
        var turnTime = 0.0f;
        var t = 0.0f;
        var rotation = Quaternion.AngleAxis(90.0f, axis);
        var originalRotations = new Quaternion[cubes.Count];
        for (int i = 0; i < cubes.Count; i++)
        {
            originalRotations[i] = cubes[i].transform.localRotation;
        }
        turnTime = Mathf.Clamp(turnTime + Time.deltaTime, 0f, rotateTime);
        while (turnTime < rotateTime)
        {
            turnTime = turnTime + Time.deltaTime;
            t = Mathf.InverseLerp(0.0f, rotateTime, turnTime);
            for (int i = 0; i < cubes.Count; i++)
            {
                //cube.transform.RotateAround(Vector3.zero, axis, speed * Time.deltaTime);

                var targetRotation = rotation * originalRotations[i];
                cubes[i].transform.localRotation = Quaternion.Lerp(originalRotations[i], targetRotation, t);
            }
            yield return null;
        }
//        float t = Mathf.InverseLerp(0f, rotateTime, turnTime);
//        for ( int i = 0; i < rotationPieces.Length; i++)
//        {
////            Transform piece = rotationPieces[i];
//            Quaternion originalRotation = originalRotations[i];
//            Quaternion targetRotation = rotation * originalRotation;
//            piece.localRotation = Quaternion.Lerp(originalRotation, targetRotation, t);
//        }
        onComplete?.Invoke();
    }
}