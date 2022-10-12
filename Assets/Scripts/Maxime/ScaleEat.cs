using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleEat : MonoBehaviour
{
    public enum SwitchSizeSkin
    {
        Little,
        Medium,
        Big
    }  

    public SwitchSizeSkin switchSkin;
    public float NbEaten = 1;
    public Vector3 scaler = new Vector3(1,1,1);
    public Mesh meshLittle;
    public Mesh meshAverage;
    public Mesh meshBig;
    public Mesh CurrentMesh;
    private MeshFilter _meshFilterGo;

    private void InitializedSize()
    {
        switchSkin = SwitchSizeSkin.Little;
    }
    private void Start()
    {
        _meshFilterGo = this.transform.GetChild(0).GetComponent<MeshFilter>();
        CurrentMesh = _meshFilterGo.mesh;

    }
    void Update()
    {
       scaleEat();
    }

    void scaleEat()
    {
        transform.localScale = scaler;
        NbEaten = Mathf.Clamp(NbEaten, 0.2f, 1f);

        //juste pour sa soit smooth
        scaler.y = Mathf.Lerp(scaler.y, NbEaten, .03f); 
        scaler.x = Mathf.Lerp(scaler.x, NbEaten, .03f);
        scaler.z = Mathf.Lerp(scaler.z, NbEaten, .03f);

        if (NbEaten >= 0.7f)
        {
            switchSkin = SwitchSizeSkin.Big;
            _meshFilterGo.mesh = meshBig;
            
        }
        else if (NbEaten <= 0.3f)
        {
            switchSkin = SwitchSizeSkin.Little;
            _meshFilterGo.mesh = meshLittle;
        }
        else
        {
            switchSkin = SwitchSizeSkin.Medium;
            _meshFilterGo.mesh = meshAverage;
        }
    }
}
