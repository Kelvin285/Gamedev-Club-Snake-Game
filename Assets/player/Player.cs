using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody rigidbody;

    public ParticleSystem jump_particles;

    public Animator animator;

    public bool ground_touching;

    public bool do_jump = false;
    public bool jump = false;

    public bool do_walk = false;

    public float timer = 0;

    public Transform camera;
    public Vector3 camera_offset = new Vector3(0, 1, -1).normalized * 12.0f;

    public bool KABOOM = false;
    private bool do_kick = false;

    public Vector3 dir = new Vector3(0, 0, 0);

    void Start()
    {

    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            animator.SetBool("kick", true);
        }
        else
        {
            animator.SetBool("kick", false);
        }

        if (KABOOM)
        {
            if (!do_kick)
            {
                do_kick = true;

                var objects = FindObjectsOfType<Rigidbody>();

                foreach (Rigidbody obj in objects)
                {
                    if (obj.gameObject != gameObject)
                    {
                        if (Vector3.Distance(obj.gameObject.transform.position, transform.position + dir * 5) <= 6)
                        {
                            obj.AddForceAtPosition(new Vector3(dir.x, 1, dir.z) * 16, transform.position, ForceMode.Impulse);
                        }
                    }
                }
            }
        } else
        {
            do_kick = false;
        }
    }
    void FixedUpdate()
    {
        camera.transform.position = Vector3.Lerp(camera.transform.position, transform.position + camera_offset, Time.fixedDeltaTime * 4);
        camera.LookAt(transform);

        if (ground_touching)
        {
            bool not_jumping = !jump;
            if (Input.GetKey(KeyCode.Space))
            {
                if (not_jumping && !do_jump)
                {
                    animator.SetBool("jump", true);
                    timer = 0;
                }
            }
            Vector3 dir = new Vector3(0, 0, 0);
            if (Input.GetKey(KeyCode.W))
            {
                dir.z += 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                dir.z -= 1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                dir.x -= 1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                dir.x += 1;
            }

            this.dir = dir;
            if (this.dir.magnitude > 0)
            {
                this.dir.Normalize();
            }

            if (dir.magnitude > 0)
            {

                if (not_jumping && !do_walk)
                {
                    transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
                    animator.SetBool("walk", true);
                    timer = 0;
                }
            }
        }

        if (do_jump)
        {
            if (!jump)
            {
                jump = true;
                rigidbody.AddForce(new Vector3(0, 10, 0), ForceMode.Impulse);
                jump_particles.Play();
                ground_touching = false;
                timer = 0.35f;

                if (do_walk)
                {
                    Vector3 forwards = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;

                    rigidbody.AddForce(new Vector3(forwards.x * 5, 0, forwards.z * 5), ForceMode.Impulse);
                }
            }
        }

        if (do_walk)
        {
            if (!jump)
            {
                jump = true;
                Vector3 forwards = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;

                rigidbody.AddForce(new Vector3(forwards.x * 5, 5, forwards.z * 5), ForceMode.Impulse);

                ground_touching = false;
                timer = 0.35f;
            }
        }


        if (timer <= 0)
        {
            if (timer < 0)
            {
                animator.SetBool("jump", false);
                animator.SetBool("walk", false);
            }
            var down = Physics.RaycastAll(transform.position + Vector3.up * 0.1f, Vector3.down, 0.5f);

            bool hit = false;
            foreach (var collision in down)
            {
                if (collision.collider.gameObject != gameObject && !collision.collider.isTrigger)
                {
                    if (!ground_touching)
                    {
                        animator.SetBool("jump", false);
                        animator.SetBool("walk", false);
                        jump = false;
                    }
                    ground_touching = true;
                    hit = true;
                }
            }
            if (hit)
            {
                ground_touching = true;
            }
        }

        timer -= Time.fixedDeltaTime;

    }
}
