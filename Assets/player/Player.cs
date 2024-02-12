using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody rigidbody;

    public ParticleSystem jump_particles;

    public Animator animator;

    public int ground_touching;

    public bool do_jump = false;
    public bool jump = false;

    void Start()
    {

    }
    void Update()
    {
        if (ground_touching > 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                animator.SetBool("jump", true);
            }
        }
        if (do_jump)
        {
            if (!jump)
            {
                jump = true;
                rigidbody.AddForce(new Vector3(0, 7, 0), ForceMode.Impulse);
                jump_particles.Play();
            }
        }

        do_jump = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        ground_touching += 1;
        animator.SetBool("jump", false);
        jump = false;
    }

    private void OnTriggerExit(Collider other)
    {
        ground_touching -= 1;
    }
}
