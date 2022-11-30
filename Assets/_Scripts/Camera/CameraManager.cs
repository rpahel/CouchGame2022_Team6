using Cinemachine;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    #region Variables
    //===============================================
    [HideInInspector] public List<Transform> pTransforms = new List<Transform>();

    //===============================================
    [SerializeField] private CinemachineTargetGroup targetGroup;
    public CinemachineTargetGroup TargetGroup => targetGroup;

    //===============================================
    private float scale;
    private float maxLensSizeX;
    private float maxLensSizeY;
    private float realMaxLensSize;
    private float screenFormat;
    private Vector2 barycentre;
    private Vector2 realImageSize;
    private Vector2 imageSizeToScreenFormat;
    private PolygonCollider2D camConfiner;
    private CinemachineVirtualCamera cineCam;
    private CinemachineConfiner2D cineConfiner;

    //===============================================
    [SerializeField] private GameObject VCam;
    [SerializeField, Tooltip("La taille (ou zoom) minimale de la caméra.")]
    private float minLensSize;
    [SerializeField, Tooltip("La hauteur, en nombre de cubes, de l'espace libre entre le haut/bas du niveau et les limites de la caméra.")]
    private int excess;
    #endregion

    #region Unity_Functions
    private void Awake()
    {
        screenFormat = 16f / 9f; // TODO : Prendre le format de l'écran au lieu de le mettre en dur
        cineCam = VCam.GetComponent<CinemachineVirtualCamera>();
        cineConfiner = VCam.GetComponent<CinemachineConfiner2D>();
        camConfiner = GetComponent<PolygonCollider2D>();
    }

    private void Start()
    {
        scale = GameManager.Instance.LevelGenerator.Scale;
        realImageSize = new Vector2(GameManager.Instance.LevelGenerator.ImageRef.width, GameManager.Instance.LevelGenerator.ImageRef.height);

        // On converti la taille de l'image pour s'adapter au format de l'écran
        /*imageSizeToScreenFormat = realImageSize.x > realImageSize.y ?
                         new Vector2(realImageSize.x, realImageSize.x / screenFormat) :
                         new Vector2(realImageSize.y * screenFormat, realImageSize.y);*/
        imageSizeToScreenFormat = new Vector2((realImageSize.y + excess * 2) * screenFormat, realImageSize.y + excess * 2);

        // Ici on setup les tailles min et max de la lentille
        maxLensSizeX = imageSizeToScreenFormat.x * .8f * (scale / 2.857143f);
        maxLensSizeY = imageSizeToScreenFormat.y * 1.425f * (scale / 2.857143f);
        realMaxLensSize = (realImageSize.x > realImageSize.y) ? maxLensSizeX : maxLensSizeY;
        cineConfiner.m_MaxWindowSize = realMaxLensSize;

        Vector2 imageCenterInWorld = .5f * scale * (realImageSize - Vector2.one);

#if UNITY_EDITOR
        {
            Debug.DrawLine(imageCenterInWorld, imageCenterInWorld + Vector2.right * 20, Color.cyan, 5f);
            Debug.DrawLine(imageCenterInWorld, imageCenterInWorld + Vector2.down * 20, Color.cyan, 5f);
        }
#endif

        // On setup les limites du camera confiner
        Vector2[] tempPoints = new Vector2[4];
        for (int i = 0; i < 4; i++)
        {
            // 0 = 00, 1 = 01, 2 = 10, 3 = 11;
            tempPoints[i] = (new Vector2(((i >> 1) & 1) * imageSizeToScreenFormat.x, (i & 1) * imageSizeToScreenFormat.y) - .5f * Vector2.one) * scale;
        }
        (tempPoints[2], tempPoints[3]) = (tempPoints[3], tempPoints[2]); // On échange les deux derniers index

        Bounds bounds = new(tempPoints[0], Vector2.zero);
        for (int i = 1; i < tempPoints.Length; i++)
        {
            bounds.Encapsulate(tempPoints[i]);
        }

        Vector2 imageCenterToBoundsCenter = (Vector2)bounds.center - imageCenterInWorld;

        // On ajuste le camera confiner pour que le niveau soit bien au centre de l'écran
        for (int i = 0; i < 4; i++)
        {
            // Déplacer à gauche ou vers le haut
            tempPoints[i] -= imageCenterToBoundsCenter;
        }

        bounds = new(tempPoints[0], Vector2.zero);
        for (int i = 1; i < tempPoints.Length; i++)
        {
            bounds.Encapsulate(tempPoints[i]);
        }

        camConfiner.SetPath(0, tempPoints);
        cineConfiner.InvalidateCache(); // Refresh le confiner pour prendre en compte les nouveaux points

#if UNITY_EDITOR
        {
            Debug.DrawLine(new Vector3(bounds.min.x, bounds.max.y, -8), new Vector3(bounds.max.x, bounds.min.y, -8), Color.red, 5);
            Debug.DrawLine(bounds.min - Vector3.forward * 8, bounds.max - Vector3.forward * 8, Color.red, 5);
        }
#endif

        // On setup la camera bien au centre du level
        cineCam.m_Lens.OrthographicSize = realMaxLensSize;
        barycentre = .5f * scale * (realImageSize - Vector2.one);
        VCam.transform.position = (Vector3)barycentre - Vector3.forward * 10;

        cineCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_MinimumOrthoSize = minLensSize;
        cineCam.GetCinemachineComponent<CinemachineFramingTransposer>().m_MaximumOrthoSize = realMaxLensSize;
    }
    #endregion

    #region Custom_Functions

    /// <summary>
    /// On donne ou on retire un joueur du target group de la camera.
    /// </summary>
    /// <param name="pTransform">Le transform du joueur à suivre. S'il est déjà dans la liste, il sera retiré.</param>
    public void UpdatePlayers(Transform pTransform)
    {
        if (pTransforms.Contains(pTransform))
        {
            pTransforms.Remove(pTransform);
        }
        else
        {
            pTransforms.Add(pTransform);
        }

        targetGroup.m_Targets = new CinemachineTargetGroup.Target[pTransforms.Count];

        for (int i = 0; i < pTransforms.Count; i++)
        {
            CinemachineTargetGroup.Target target;
            target.target = pTransforms[i];
            target.weight = 1;
            target.radius = 6;
            targetGroup.m_Targets.SetValue(target, i);
        }
    }
    #endregion
}