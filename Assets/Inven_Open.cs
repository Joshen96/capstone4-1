using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inven_Open : MonoBehaviour
{
    // Start is called before the first frame update
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("i"))
        {
            Debug.Log(animator.GetBool("Inven_Open"));
            if (animator.GetBool("Inven_Open") == false)
            {
                animator.SetBool("Inven_Open", true);
            }
            else
            {
                animator.SetBool("Inven_Open", false);
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (animator.GetBool("Inven_Open") == true)
            {
                animator.SetBool("Inven_Open", false);
            }
        }
    }
}
