using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//credits from one of my unreleased projects
public class PlayerController : MonoBehaviour {
    public float horizontalSpeed = 1;
    public float verticalSpeed = 1;

    public float movespeed = 1;
    public float jumpspeed = 1;
    [Range(0.0f, 1.0f)]
    public float drag = .4f;
    [Range(0.0f, 1.0f)]
    public float airDrag = .95f;
    public float gravity = 4;
    public bool grounded = false;

    public Transform center;
    public Transform model;
    private Rigidbody rb;
    public int maxJumps = 1;
    public int jumps = 1;

    public float jumpLength = 1;
    private float jumpHeld = 0;
    public bool isJumping;

    public Vector3 velocity;
    public float fallSpeed;

    private CharacterController character;

    public GameObject bullet;
    public float fireRate = .05f;
    private float fireBuffer;



    // Start is called before the first frame update
    void Start() {
        character = GetComponent<CharacterController>();
        velocity = Vector3.zero;
        fallSpeed = 0;
        rb = GetComponent<Rigidbody>();
    }

    Vector3 forward => model.forward * movespeed;
    Vector3 right => model.right * movespeed;
    Vector3 up => model.up;
    // Update is called once per frame

    private void UpdateVelocity() {
        if (character.isGrounded) velocity *= drag;
        else velocity *= airDrag;

        velocity += right * Input.GetAxis("Horizontal") + forward * Input.GetAxis("Vertical");

        if (!character.isGrounded && !isJumping) {
            fallSpeed -= gravity * Time.deltaTime;
        }
        else if (!isJumping)
        {
            velocity = new Vector3(velocity.x, 0, velocity.z);
            jumps = maxJumps;
            fallSpeed = 0;
        }

        if (Input.GetKeyDown(KeyCode.Space) && jumps > 0) {
            velocity = new Vector3(velocity.x, 0, velocity.z);
            isJumping = true;
            fallSpeed += jumpspeed;
            jumps--;
        }

        velocity += Vector3.up * fallSpeed * Time.timeScale;


    }


    void Update() {

        UpdateVelocity();

        character.Move(velocity * Time.deltaTime * Time.timeScale);

        if (Input.GetKey(KeyCode.Space)) {
            jumpHeld += Time.deltaTime;
        }
        if (Input.GetKeyUp(KeyCode.Space) || jumpHeld > jumpLength) {
            isJumping = false;
            jumpHeld = 0;
        }
        fireBuffer -= Time.deltaTime;
        if (Input.GetKey(KeyCode.Mouse0) && fireBuffer < 0)
        {
            fireBuffer = fireRate;
            GameObject b = Instantiate(bullet, center.position, Quaternion.identity);
            Vector3 lookPos = Camera.main.transform.position + Camera.main.transform.forward * 100;
            
            b.transform.LookAt(lookPos);
            b.transform.RotateAround(b.transform.position, b.transform.right, 90);
            b.transform.position += (lookPos - center.position).normalized * b.transform.lossyScale.y;
        }

        
    }

    private void FixedUpdate() {

        //rb.velocity = velocity ;
    }
}
