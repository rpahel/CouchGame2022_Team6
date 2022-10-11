using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleEat : MonoBehaviour
{
    public enum SwitchSizeSkin
    {
        little,
        average,
        big
    }  

    public SwitchSizeSkin switchSkin;
    public float NbEaten = 200f;
    public Vector3 scaler = new Vector3(1,1,1);
    public Mesh meshLittle;
    public Mesh meshAverage;
    public Mesh meshBig;
    public Mesh CurrentMesh;
    private Mesh _meshFilterGo;

    private DashCardinalDirection dashCardinalDirection;
    // public bool squashed;

    private void InitializedSize()
    {
        switchSkin = SwitchSizeSkin.little;
        dashCardinalDirection = GetComponent<DashCardinalDirection>();
    }
    private void Start()
    {
        _meshFilterGo = this.transform.GetChild(0).GetComponent<MeshFilter>().mesh;
        Debug.Log(_meshFilterGo);
        CurrentMesh = _meshFilterGo;

    }
    // Update is called once per frame
    void Update()
    {
       scaleEat();
    }

    void scaleEat()
    {
        transform.localScale = scaler;
        NbEaten = Mathf.Clamp(NbEaten, 1, 300);

        //juste pour sa soit smooth
        scaler.y = Mathf.Lerp(scaler.y, NbEaten * 0.005f, .03f);
        scaler.x = Mathf.Lerp(scaler.x, NbEaten * 0.005f, .03f);
        scaler.z = Mathf.Lerp(scaler.z, NbEaten * 0.005f, .03f);


        //les different etat du player
        switch (switchSkin)
        {
            case SwitchSizeSkin.little:
                InitializedSize();
                CurrentMesh = meshLittle;
                if (NbEaten >= 100)
                {
                    switchSkin = SwitchSizeSkin.average;
                    _meshFilterGo = meshAverage;
                }
                break;
            case SwitchSizeSkin.average:
                CurrentMesh = meshAverage;
                if (NbEaten < 100)
                {
                    switchSkin = SwitchSizeSkin.little;
                    _meshFilterGo = meshLittle;
                }
                else if (NbEaten >= 200)
                {
                    switchSkin = SwitchSizeSkin.big;
                    _meshFilterGo = meshBig;
                }
                break;
            case SwitchSizeSkin.big:
                CurrentMesh = meshBig;
            
                if (NbEaten < 100)
                {
                    switchSkin = SwitchSizeSkin.little;
                    _meshFilterGo = meshLittle;
                }
                else if (NbEaten < 200)
                {
                    switchSkin = SwitchSizeSkin.average;
                    _meshFilterGo = meshAverage;
                }
                break;
        }
    }
}
