using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor.SearchService;
using UnityEngine.Animations;

public class CameraController : MonoBehaviour
{
    public int maxZ;
    private float _minZ;

    private List<Transform> _playersTransforms = new List<Transform>();

    private bool _IsInitialized = false;
    private Camera _cam;

    private void Awake()
    {
        _minZ = gameObject.transform.position.z;
        _cam = gameObject.GetComponent<Camera>();
    }

    public void Initialize(Transform tr)
    {
        _playersTransforms.Add(tr);
    }

    public void EndOfInit()
    {
        _IsInitialized = true;
    }

    private void Update()
    {
        if (_IsInitialized)
        {
            Vector2 vec = FindCenterOfPlayers();
            gameObject.transform.position = new Vector3(vec.x, vec.y, gameObject.transform.position.z);

            if(_playersTransforms.Any(r => CheckIfPlayerIsOnScreen(r) == false &&  gameObject.transform.position.z > maxZ))
            {
                var position = transform.position;
                gameObject.transform.position = new Vector3(position.x, position.y, position.z - 0.003f);
            }
            else if (_playersTransforms.All(r => CheckIfPlayerIsOnScreen(r) == true) && gameObject.transform.position.z < _minZ)
            {
                var position = gameObject.transform.position;
                gameObject.transform.position = new Vector3(position.x, position.y, position.z + 0.003f);
            }
        }
    }

    private Vector2 FindCenterOfPlayers()
    {
        float floatX = 0;
        float floatY = 0;
        
        for (int x = 0; x < _playersTransforms.Count; x++)
        {
            floatX += _playersTransforms[x].transform.position.x;
            floatY += _playersTransforms[x].transform.position.y;
        }

        floatX /= _playersTransforms.Count;
        floatY /= _playersTransforms.Count;

        Vector2 centerPos = new Vector3(floatX, floatY);
        return centerPos;
    }

    private bool CheckIfPlayerIsOnScreen(Transform pd)
    {
        Vector3 screenPos = _cam.WorldToScreenPoint(pd.transform.position);
        if (screenPos.x > Screen.currentResolution.width * 0.9 || screenPos.x < 0 || screenPos.y < 0 ||
            screenPos.y > Screen.currentResolution.height * 0.9)
            return false;
        return true;
    }
}
