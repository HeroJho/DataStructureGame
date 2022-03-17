using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	private Animator anim;
	private Rigidbody rigid;

	public float speed = 600.0f;
	public float jump = 1f;
	public float turnSpeed = 400.0f;
	private Vector3 moveDirection = Vector3.zero;
	public float gravity = 20.0f;

	public float Speed = 10.0f;
	public float rotateSpeed = 10.0f;
	float v;
	public bool isJump = false;

	void Start () 
	{
		anim = gameObject.GetComponent<Animator>();
		rigid = gameObject.GetComponentInChildren<Rigidbody>();
	}

	void Update ()
	{
		v = Input.GetAxis("Vertical");

		anim.SetFloat("moveValue", Mathf.Abs(v));
	}

    void FixedUpdate()
    {
		Quaternion cameraRotation = Camera.main.transform.rotation;
		cameraRotation.x = cameraRotation.z = 0;

		if (v != 0)
        {
			transform.position += transform.forward * speed * Time.deltaTime;
			transform.rotation = Quaternion.Lerp(transform.rotation, cameraRotation, Time.deltaTime * rotateSpeed);
		}
		else
			transform.rotation = transform.rotation;

		if (Input.GetKeyDown(KeyCode.Space) && !isJump)
        {
			isJump = true;
			anim.SetBool("Jump", isJump);
			rigid.AddForce(Vector3.up * jump, ForceMode.Impulse);
        }
	}

    private void OnCollisionEnter(Collision collision)
    {
		isJump = false;
		anim.SetBool("Jump", isJump);
	}
}
