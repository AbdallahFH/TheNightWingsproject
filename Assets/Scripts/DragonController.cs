using UnityEngine;

public class DragonController : MonoBehaviour
{ 
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 5f;
    public float rotationSpeed = 10f;
    public float DumpTime;
    public float AnimSmooth;
    private Rigidbody rb;
    private bool isGrounded;
    private Vector3 movementInput;
    private Vector3 movementDirection;

    private CameraController cameraController;
    Animator animator;
    public float RbVelo;
    bool IsFlying;
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        cameraController = Camera.main.GetComponent<CameraController>();
        if (cameraController == null)
        {
            Debug.LogError("CameraController script not found on the Main Camera!");
        }
    }

    void FixedUpdate()
    {
        HandleMovementInput();
        MovePlayer();
        HandleJump();
        animator.SetBool("Grounded",isGrounded);
    }

    void HandleMovementInput()
    {
        // Get movement input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if(isGrounded){
            IsFlying = false;
        }
        if(Input.GetKeyDown(KeyCode.F)){
            IsFlying = !IsFlying;
        }

        movementInput = new Vector3(horizontal, 0, vertical).normalized;

        // Determine movement direction relative to camera
        movementDirection = cameraController.transform.forward * movementInput.z + cameraController.transform.right * movementInput.x;
        movementDirection.y = 0; // Ignore Y-axis
    }
Vector3 FlyDirection;
    void MovePlayer()
    {
        float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        animator.SetBool("Fly",IsFlying);
        if(IsFlying){
            rb.useGravity = false;
            if (movementInput.magnitude >= 0.1f)
            {
                Vector3 move = transform.forward * currentSpeed * Time.deltaTime;
                float SideWays = Input.GetAxis("SideWays");

                // Move the player using Rigidbody
                rb.MovePosition(rb.position + move);
                FlyDirection = transform.eulerAngles;
                FlyDirection.x+=movementInput.z;
                FlyDirection.y+=movementInput.x;
                FlyDirection.z+=SideWays;
            }
            // Rotate player to face movement direction
            Quaternion targetRotation = Quaternion.Euler(FlyDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
          
        }else{
        rb.useGravity = true;

        if (movementInput.magnitude >= 0.1f)
        {
            Vector3 move = movementDirection.normalized * currentSpeed * Time.deltaTime;

            // Move the player using Rigidbody
            rb.MovePosition(rb.position + move);

            // Rotate player to face movement direction
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        }
        float animspeed = Input.GetKey(KeyCode.LeftShift)? 1.0f:0.5f;
        animator.SetFloat("Speed",animspeed,DumpTime,Time.deltaTime);
    }

    void HandleJump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.CrossFade("Jump",0.2f);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}