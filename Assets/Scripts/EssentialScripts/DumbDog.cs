using UnityEngine;

public class DumbDog : MonoBehaviour
{

    [SerializeField] private Transform bone;

    [SerializeField] private float rotationCoeff = 3f;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float activationRange = 1f;

    private Animator animator;
    private Rigidbody rb;
 
    private void Start() {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Apply rotation
        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(bone.position.x, transform.position.y, bone.position.z) - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationCoeff * Time.deltaTime);

        //if distance to bone is less than 2, do not move
        if (Vector3.Distance(transform.position, bone.position) > activationRange)
        {
            transform.position += transform.forward * speed * Time.deltaTime;
            animator.SetFloat("speed", 1f);
        }
        else
        {
            animator.SetFloat("speed", 0f);
        }
    }
}
