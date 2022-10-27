using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    #region Variables
    //===============================================
    private List<Transform> pTransforms = new List<Transform>();
    public List<Transform> PTransforms { get => pTransforms; set => pTransforms = value; }

    //===============================================
    private Vector2 sommeVecteurs;
    private Vector2 barycentre;
    private CinemachineVirtualCamera cineCam;

    //===============================================
    [SerializeField] private GameObject VCam;
    #endregion

    #region Unity_Functions
    private void Awake()
    {
        cineCam = VCam.GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        if(pTransforms.Count > 0)
        {
            sommeVecteurs = Vector2.zero;
            for(int i = 0; i < pTransforms.Count; i++)
            {
                sommeVecteurs += (Vector2)pTransforms[i].position;
            }
            barycentre = sommeVecteurs / pTransforms.Count;
        }

        VCam.transform.position = (Vector3)barycentre - Vector3.forward * 10;
    }
    #endregion
}
