
using UnityEngine;
using System.Collections;

public class depalcementvitefait : MonoBehaviour
{

    public int speed = 10;

    private Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        float mouveHorizontal = Input.GetAxis("Horizontal");
        float mouveVertical = Input.GetAxis("Vertical");

        Vector3 mouvment = new Vector3(mouveHorizontal, 0, mouveVertical);
        rb.AddForce(mouvment * speed * Time.deltaTime);

    }
}
