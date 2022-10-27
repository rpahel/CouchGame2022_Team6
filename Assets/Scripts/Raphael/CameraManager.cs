using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraManager : MonoBehaviour
{
    #region Variables
    //===============================================
    private List<Transform> pTransforms = new List<Transform>();
    public List<Transform> PTransforms { get => pTransforms; set => pTransforms = value; }

    //===============================================
    private float echelle;
    private float maxLensSizeX;
    private float maxLensSizeY;
    private float realMaxLensSize;
    private Vector2 barycentre;
    private Vector2 imageSize;
    private PolygonCollider2D camConfiner;
    private CinemachineVirtualCamera cineCam;
    private CinemachineConfiner2D cineConfiner;

    //===============================================
    [SerializeField] private GameObject VCam;
    [SerializeField] private float minLensSize;
    #endregion

    #region Unity_Functions
    private void Awake()
    {
        cineCam = VCam.GetComponent<CinemachineVirtualCamera>();
        cineConfiner = VCam.GetComponent<CinemachineConfiner2D>();
        camConfiner = GetComponent<PolygonCollider2D>();
    }

    private void Start()
    {
        echelle = GameManager.Instance.LGenerator.Echelle;
        imageSize = GameManager.Instance.LGenerator.ImageRef.Size();
        maxLensSizeX = imageSize.x * .8f;
        maxLensSizeY = imageSize.y * 1.425f;
        realMaxLensSize = (imageSize.x > imageSize.y) ? maxLensSizeX : maxLensSizeY;
        cineConfiner.m_MaxWindowSize = realMaxLensSize;

        Vector2[] tempPoints = new Vector2[4];
        for(int i = 0; i < 4; i++)
        {
            // 0 = 00, 1 = 01, 2 = 10, 3 = 11;
            tempPoints[i] = (new Vector2(((i >> 1) & 1) * imageSize.x, (i & 1) * imageSize.y) - .5f * Vector2.one) * echelle;
        }
        (tempPoints[2], tempPoints[3]) = (tempPoints[3], tempPoints[2]); // On échange les deux derniers index
        camConfiner.SetPath(0, tempPoints);
        cineConfiner.InvalidateCache(); // Refresh le confiner pour prendre en compte les nouveaux points
    }

    private void LateUpdate()
    {
        if (pTransforms.Count > 0)
        {
            Bounds bounds = new((Vector2)pTransforms[0].position, Vector2.zero);
            for (int i = 1; i < pTransforms.Count; i++)
            {
                bounds.Encapsulate((Vector2)pTransforms[i].position);
            }

            barycentre = (Vector2)bounds.center;

            if (pTransforms.Count > 1)
            {
                cineCam.m_Lens.OrthographicSize = bounds.size.x > bounds.size.y ? bounds.size.x * .33f : bounds.size.y * .66f;
                cineCam.m_Lens.OrthographicSize = Mathf.Clamp(cineCam.m_Lens.OrthographicSize, minLensSize, (bounds.size.x > bounds.size.y) ? maxLensSizeX : maxLensSizeY);
            }
            else
            {
                cineCam.m_Lens.OrthographicSize = minLensSize;
            }
        }
        else
        {
            barycentre = .5f * echelle * (imageSize - Vector2.one);
            cineCam.m_Lens.OrthographicSize = realMaxLensSize;
        }

        VCam.transform.position = (Vector3)barycentre - Vector3.forward * 10;
    }
    #endregion
}
