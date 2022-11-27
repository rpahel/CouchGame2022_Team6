using Cinemachine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    #region Variables
    //===============================================
    private List<Transform> pTransforms = new List<Transform>();
    public List<Transform> PTransforms { get => pTransforms; set => pTransforms = value; }

    //===============================================
    [SerializeField] private CinemachineTargetGroup targetGroup;
    public CinemachineTargetGroup TargetGroup => targetGroup;

    //===============================================
    private float echelle;
    private float maxLensSizeX;
    private float maxLensSizeY;
    private float realMaxLensSize;
    private const float format = 1.77777777778f; // TODO : Prendre le format de l'écran au lieu de le mettre en dur
    private Vector2 barycentre;
    private Vector2 realImageSize;
    private Vector2 imageSize_16_9;
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
        echelle = GameManager.Instance.LevelGenerator.Echelle;
        realImageSize = GameManager.Instance.LevelGenerator.ImageRef.Size();
        imageSize_16_9 = realImageSize.x > realImageSize.y ?
                         new Vector2(realImageSize.x, realImageSize.x / format) :
                         new Vector2(realImageSize.y * format, realImageSize.y);

        maxLensSizeX = imageSize_16_9.x * .8f * (echelle / 2.857143f);
        maxLensSizeY = imageSize_16_9.y * 1.425f * (echelle / 2.857143f);
        realMaxLensSize = (realImageSize.x > realImageSize.y) ? maxLensSizeX : maxLensSizeY;
        cineConfiner.m_MaxWindowSize = realMaxLensSize;

        Vector2 imageCenterInWorld = (realImageSize - Vector2.one) * .5f * echelle;
        Debug.DrawLine(imageCenterInWorld, imageCenterInWorld + Vector2.right * 20, Color.cyan, 100000f);
        Debug.DrawLine(imageCenterInWorld, imageCenterInWorld + Vector2.down * 20, Color.cyan, 100000f);

        Vector2[] tempPoints = new Vector2[4];
        for(int i = 0; i < 4; i++)
        {
            // 0 = 00, 1 = 01, 2 = 10, 3 = 11;
            tempPoints[i] = (new Vector2(((i >> 1) & 1) * imageSize_16_9.x, (i & 1) * imageSize_16_9.y) - .5f * Vector2.one) * echelle;
        }
        (tempPoints[2], tempPoints[3]) = (tempPoints[3], tempPoints[2]); // On échange les deux derniers index

        Bounds bounds = new(tempPoints[0], Vector2.zero);
        for (int i = 1; i < tempPoints.Length; i++)
        {
            bounds.Encapsulate(tempPoints[i]);
        }

        Vector2 imageCenterToBoundsCenter = (Vector2)bounds.center - imageCenterInWorld;
        
        for (int i = 0; i < 4; i++)
        {
            //Déplacer à gauche ou vers le haut
            tempPoints[i] -= imageCenterToBoundsCenter;
        }

        bounds = new(tempPoints[0], Vector2.zero);
        for (int i = 1; i < tempPoints.Length; i++)
        {
            bounds.Encapsulate(tempPoints[i]);
        }

        camConfiner.SetPath(0, tempPoints);
        cineConfiner.InvalidateCache(); // Refresh le confiner pour prendre en compte les nouveaux points
        Debug.DrawLine(new Vector3(bounds.min.x, bounds.max.y, -8), new Vector3(bounds.max.x, bounds.min.y, -8), Color.red, 1000);
        Debug.DrawLine(bounds.min - Vector3.forward * 8, bounds.max - Vector3.forward * 8, Color.red, 1000);

        cineCam.m_Lens.OrthographicSize = realMaxLensSize;
        barycentre = .5f * echelle * (realImageSize - Vector2.one);
        VCam.transform.position = (Vector3)barycentre - Vector3.forward * 10;

        cineCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_MinimumOrthoSize = minLensSize;
        cineCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_MaximumOrthoSize = realMaxLensSize;
    }
    #endregion

    #region Custom_Functions

    public void UpdatePlayers(Transform pTransform)
    {
        PTransforms.Add(pTransform);
        targetGroup.m_Targets = new CinemachineTargetGroup.Target[pTransforms.Count];

        for(int i = 0; i < PTransforms.Count; i++)
        {
            CinemachineTargetGroup.Target target;
            target.target = PTransforms[i];
            target.weight = 1;
            target.radius = 6;
            targetGroup.m_Targets.SetValue(target, i);
        }
    }
    #endregion
}
