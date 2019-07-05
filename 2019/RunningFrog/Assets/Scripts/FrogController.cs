using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogController : MonoBehaviour
{
    private static float idle_xpos = -2f;
    private static float ground_lvl = -1.84f;

    public static float jump_force = 25f;
    public static float jump_height = 4f;

    private Rigidbody myRigidbody;
    public GameObject RunningFrog;
    public GameObject AnimFrog;
    public GameObject SmallFrog;

    public GameObject Ground;

    private bool landed = true;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        SmallFrog.SetActive(false);
        
    }

    public float jump_cycle = 0.0f;
    public float jump_pos = 0.0f;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && landed)
        {
            landed = false;
        }
        if (!landed)
        {

            jump_cycle += Time.deltaTime * 3f;
            if (jump_cycle > 3.1)
            {
                landed = true;
                jump_cycle = 0;
            }

        }

        jump_pos = Mathf.Min(Mathf.Sin(jump_cycle) * jump_height,2.25f);
        transform.position = new Vector3(idle_xpos, jump_pos + ground_lvl, 0);

        //Hop
        if (Input.GetButtonDown("Fire2"))
        {
            RunningFrog.SetActive(false);
            AnimFrog.SetActive(false);
            SmallFrog.SetActive(true);
        }
        if (Input.GetButtonUp("Fire2"))
        {
            RunningFrog.SetActive(true);
            AnimFrog.SetActive(true);
            SmallFrog.SetActive(false);
        }
    }
}
