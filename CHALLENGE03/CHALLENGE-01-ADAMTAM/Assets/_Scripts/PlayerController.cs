using Cinemachine;
using DG.Tweening;
using DitzelGames.FastIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class PlayerController : MonoBehaviour, PlayerInputActions.IPlayActions {
    private PlayerInputActions inputActions;
    private Rigidbody rb;
    private Transform cam, currentHeldObject;
    private CapsuleCollider coll;
    private Animator animator;
    Vector3 rawMove = Vector3.zero, relMove = Vector3.zero;
    Vector2 lookDelta;
    float verticalRotation = 0;
    bool grounded, canMove, crouching;

    readonly int IDLE = Animator.StringToHash("Idle");
    readonly int WALK = Animator.StringToHash("Walk");


    [SerializeField] EventManager eventManager;
    [SerializeField] CinemachineVirtualCamera vcam;
    [SerializeField] float moveSpeed, jumpSpeed, sens, gravity, interactionRange;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform rotatable, leftHandTarget, rightHandTarget;
    [SerializeField] FastIKFabric leftHandIK, rightHandIK;


    public bool CanMove { get { return canMove; } set { canMove = value; } }
    public Transform LeftHandTarget { get { return leftHandTarget; } set {  leftHandTarget = value; } }
    public Transform RightHandTarget { get { return rightHandTarget; } set {  rightHandTarget = value; } }
    public FastIKFabric LeftHandIK { get { return leftHandIK; } set { leftHandIK = value; } }
    public FastIKFabric RightHandIK { get { return rightHandIK; } set { rightHandIK = value; } }

    IInteractable currentInteractable;
    private void Awake() {
        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();
        cam = vcam.transform;
        Cursor.lockState = CursorLockMode.Locked;
        coll = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();

        leftHandIK.enabled = false;
        rightHandIK.enabled = false;
    }
    private void OnEnable() {
        inputActions.Play.SetCallbacks(this);
        eventManager.OnMessageBroadcast += ProcessEvent;
    }

    private void OnDisable() {
        eventManager.OnMessageBroadcast -= ProcessEvent;
    }
    void Start() {
        inputActions.Play.Enable();
        canMove = true;
    }

    void Update() {
        // calculate input relative to camera
        relMove = cam.right * rawMove.x + cam.forward * rawMove.z;
        relMove.y = 0;
        relMove = relMove.normalized;
    }

    private void FixedUpdate() {
        ProcessMovement();
        // grounded check
        grounded = Physics.Raycast(coll.bounds.center, Vector3.down, coll.bounds.extents.y + .2f, groundLayer);

        // gravity
        if (!grounded) rb.velocity += Vector3.down * gravity;

        CheckForInteractables();
    }
    private void LateUpdate() {
        verticalRotation = Mathf.Clamp(verticalRotation - lookDelta.y * sens * Time.deltaTime, -90, 90);
        cam.eulerAngles = new Vector3(verticalRotation, cam.eulerAngles.y + lookDelta.x * sens * Time.deltaTime);
        transform.eulerAngles = Vector3.up * transform.eulerAngles.y;

        rotatable.rotation = cam.rotation;
        rotatable.eulerAngles = new Vector3(0, rotatable.eulerAngles.y, 0);
    }
    public void OnMove(InputAction.CallbackContext context) {
        if (!canMove) {
            rawMove = Vector3.zero;
            animator.CrossFade(IDLE, .2f);
            return;
        }
        if (context.phase == InputActionPhase.Started) {
            animator.CrossFade(WALK, .2f);
        }
        if (context.phase == InputActionPhase.Performed) {
            Vector2 input = context.ReadValue<Vector2>();
            rawMove.x = input.x;
            rawMove.z = input.y;
        } else if (context.phase == InputActionPhase.Canceled) {
            rawMove = Vector3.zero;
            animator.CrossFade(IDLE, .2f);
        }
    }

    public void OnLook(InputAction.CallbackContext context) {
        if (!canMove) { 
            lookDelta = Vector3.zero;
            return; 
        }
        lookDelta = context.ReadValue<Vector2>();
    }

    public void OnInteract(InputAction.CallbackContext context) {
        if (context.phase != InputActionPhase.Performed) return;
        if (currentInteractable == null || currentHeldObject != null) return;
        currentInteractable.Interact(this);
    }

    public void OnJump(InputAction.CallbackContext context) {
        if (context.phase != InputActionPhase.Performed) return;
        if (grounded) rb.AddForce(Vector3.up * jumpSpeed, ForceMode.VelocityChange);
    }

    public void OnCrouch(InputAction.CallbackContext context) {
        if (context.phase != InputActionPhase.Performed) return;
        crouching = !crouching;
        float crouchProgress = crouching ? 0 : 1;
        DOTween.To(() => crouchProgress, x => crouchProgress = x, crouching ? 1 : 0, .4f).OnUpdate(() => {
            //cam.localPosition = Vector3.Lerp(Vector3.up * 1.3f, new Vector3(-.51f, .56f, .06f), crouchProgress);
            animator.SetLayerWeight(2, crouchProgress);
        });

    }

    public void OnDrop(InputAction.CallbackContext context) {
        if (context.phase != InputActionPhase.Performed) return;
        DropObject();
    }

    void ProcessEvent(Message message) {
        // Lever pulled
        if (message.message == EventCodes.LEVEL_PULLED) {
            Vector3 position = message.transform.position - (.6f * message.transform.forward);
            position.y = coll.bounds.extents.y;
            transform.DOMove(position, .4f);
            cam.DORotate(message.transform.position - cam.position, .3f);
            canMove = false;
        }
    }

    void CheckForInteractables() {
        Ray interactableRay = new Ray(cam.position, cam.forward);
        if (Physics.Raycast(interactableRay, out RaycastHit hit, interactionRange)) {
            IInteractable tempInteractable = hit.transform.GetComponent<IInteractable>();
            if (tempInteractable == null || !tempInteractable.CanInteract) {
                currentInteractable = null;
                eventManager.BroadcastMessage(new Message(EventCodes.INTERACTABLE_UNHOVERED));
                return;
            }
            currentInteractable = hit.transform.GetComponent<IInteractable>();
            eventManager.BroadcastMessage(new Message(EventCodes.INTERACTABLE_HOVERED, currentInteractable.Description));
        } else {
            currentInteractable = null;
            eventManager.BroadcastMessage(new Message(EventCodes.INTERACTABLE_UNHOVERED));
        }
    }

    void ProcessMovement() {
        if (!canMove) {
            rb.velocity = Vector3.zero;
            return;
        }
        Vector3 temp = relMove * moveSpeed;
        temp.y = rb.velocity.y;
        rb.velocity = temp;
    }

    public void AnimateHandIK(bool left, Transform target, float time) {
        if (left) {
            leftHandIK.enabled = true;
            leftHandIK.Target = target;
        } else {
            rightHandIK.enabled = true;
            rightHandIK.Target = target;
        }
    }

    public void ResetHands() {
        rightHandIK.enabled = false;
        leftHandIK.enabled = false;
    }

    public void AttachToHand(Transform t) {
        t.parent = rightHandIK.transform;
        t.position = RightHandIK.transform.position;
        currentHeldObject = t;
        animator.SetFloat("Blend", (currentHeldObject.GetComponent<Holdable>().twoHanded) ? 1 : 0);
        float handProgress = 0;
        DOTween.To(() => handProgress, x => handProgress = x, 1, .4f).OnUpdate(() => {
            animator.SetLayerWeight(1, handProgress);
        });
    }

    void DropObject() {
        if (!currentHeldObject) return;
        currentHeldObject.GetComponent<IInteractable>().CanInteract = true;
        currentHeldObject.GetComponent<Rigidbody>().useGravity = true;
        currentHeldObject.GetComponent<Rigidbody>().isKinematic = false;
        currentHeldObject.GetComponent<Collider>().enabled = true;
        currentHeldObject.parent = null;
        currentHeldObject = null;
        float handProgress = 1;
        DOTween.To(() => handProgress, x => handProgress = x, 0, .4f).OnUpdate(() => {
            animator.SetLayerWeight(1, handProgress);
        });
    }
}

public interface IInteractable {
    public string Description { get; set; }
    public bool CanInteract { get; set; }

    public void Interact(PlayerController controller);
}

public abstract class Holdable : MonoBehaviour, IInteractable {
    public bool twoHanded = false;
    bool canInteract = true;
    protected abstract string description { get; set; }    
    public string Description { get => description; set { } }
    public bool CanInteract { get => canInteract; set => canInteract = value; }

    public void Interact(PlayerController controller) {
        controller.AttachToHand(transform);
        CanInteract = false;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Collider>().enabled = false;
    }
}