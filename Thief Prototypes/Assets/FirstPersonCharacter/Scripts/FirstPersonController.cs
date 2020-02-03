using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof(Highlighter))]
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Inventory))]
    [RequireComponent(typeof(InventoryDisplay))]
    [RequireComponent(typeof(FootStepController))]
    public class FirstPersonController : MonoBehaviour
    {
        public enum MovingType { SLOWWALK, WALK, RUN };

        private bool m_IsWalking;
        [SerializeField] private float m_SlowWalkSpeed;
        [SerializeField] private float m_WalkSpeed;
        [SerializeField] private float m_RunSpeed;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private bool m_UseFovKick;
        [SerializeField] private FOVKick m_FovKick = new FOVKick();
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] private float m_StepInterval;


        private Camera m_Camera;
        private bool m_Jump;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;

        private Inventory m_Inventory;
        private InventoryDisplay m_InventoryDisplay;

        private FootStepController m_FootStepController;
        private Highlighter m_Highlighter;

        private float m_cameraHeight;
        private float m_standHeight;
        bool m_standing = true;

        public MovingType m_MovingType;

        [Space]
        public float m_MaxDistanceCheck = 5;

        bool m_IsMovementEnabled = true;
        private Vector3 m_playerVelocity;
        private Vector3 m_oldPos;

        // Use this for initialization
        private void Start()
        {
            m_Highlighter = GetComponent<Highlighter>();
            m_FootStepController = GetComponent<FootStepController>();
            m_InventoryDisplay = GetComponent<InventoryDisplay>();
            m_Inventory = GetComponent<Inventory>();
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle / 2f;
            m_Jumping = false;
            m_MouseLook.Init(transform, m_Camera.transform);

            targetPlayerHeight = m_standHeight = m_CharacterController.height;
            targetCameraHeight = m_cameraHeight = m_Camera.transform.localPosition.y;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        bool canClimbLadder = true;
        // Update is called once per frame
        private void Update()
        {
            // If ladder collision ended or jumping while climbing is on, turn of ladder climbing
            if (isClimbingLadder)
            {
                RaycastHit hit;
                if (!isCollidingWithLadder)
                {
                    ToggleLadderClimbing(false);
                }
                else if (Input.GetButtonDown("Jump"))
                {
                    ToggleLadderClimbing(false);
                    transform.position -= transform.forward * 0.2f;
                }
                else if (Input.GetAxis("Vertical") < 0 && (Input.GetButtonDown("Jump") || Physics.Raycast(transform.position, transform.up * -1, out hit, 1)))
                {
                    ToggleLadderClimbing(false);
                }
                else
                {
                    transform.Translate(Vector3.up * Input.GetAxis("Vertical") * climbingSpeed * Time.deltaTime);
                }
            }
            // If colliding with ladder in the right angle and moving vertically or in air, turn on ladder climbing
            else if (isCollidingWithLadder && canClimbLadder)
            {
                if (Mathf.Abs(Vector3.Dot(ladderTransform.forward, transform.forward)) >= 0.9f && (Input.GetAxis("Vertical") > 0f || !m_CharacterController.isGrounded))
                {
                    ToggleLadderClimbing(true);
                }
            }

            RotateView();

            // the jump state needs to read here to make sure it is not missed
            if (m_CharacterController.isGrounded)
            {
                canClimbLadder = true;

                if (!m_Jump)
                {
                    m_Jump = Input.GetButtonDown("Jump");
                }

                if (!m_PreviouslyGrounded)
                {
                    StartCoroutine(m_JumpBob.DoBobCycle());
                    if (m_playerVelocity.y < -1)
                        m_FootStepController.PlayLandingSound();
                    m_NextStep = m_StepCycle + .5f;
                    m_MoveDir.y = 0f;
                    m_Jumping = false;
                }
            }

            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;

            GetInput();
        }


        void FixedUpdate()
        {
            Vector3 NewPos = transform.position;  // each frame track the new position
            m_playerVelocity = (NewPos - m_oldPos) / Time.fixedDeltaTime;  // velocity = dist/time
            m_oldPos = NewPos;  // update position for next frame calculatio

            if (m_IsMovementEnabled)
            {
                float speed;
                GetMovmentInput(out speed);
                // always move along the camera forward as it is the direction that it being aimed at
                Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;


                // get a normal for the surface that is being touched to move along it
                RaycastHit hitInfo;
                Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo, m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);

                desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

                if (!m_CharacterController.isGrounded)
                {
                    speed *= 0.7f;
                }

                m_MoveDir.x = desiredMove.x * speed;
                m_MoveDir.z = desiredMove.z * speed;
                m_FootStepController.GetWalkingSound(hitInfo);



                if (m_CharacterController.isGrounded)
                {
                    m_MoveDir.y = -m_StickToGroundForce;

                    if (m_Jump)
                    {
                        m_MoveDir.y = m_JumpSpeed;
                        m_FootStepController.PlayJumpSound();
                        m_Jump = false;
                        m_Jumping = true;
                    }
                }
                else
                {
                    m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
                }
                m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);


                ProgressStepCycle(speed);
                //m_MouseLook.UpdateCursorLock();
            }
        }


        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * (m_IsWalking ? 1f : m_RunstepLenghten))) *
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            m_FootStepController.PlayFootStepAudio(!m_CharacterController.isGrounded, m_standing);
        }


        float crouchTime = 0.0f;
        float playerHeight = 0;
        float targetPlayerHeight = 0;
        float cameraHeight = 0;
        float targetCameraHeight = 0;

        private void GetInput()
        {
            if (Input.GetKeyDown(KeyCode.C) && !isClimbingLadder)
            {
                playerHeight = m_CharacterController.height;
                cameraHeight = m_Camera.transform.localPosition.y;
                crouchTime = 0;

                if (m_standing)
                {
                    targetCameraHeight = m_cameraHeight * .5f;
                    targetPlayerHeight = m_standHeight * .5f;
                    m_standing = false;
                }
                else
                {
                    if (!Physics.Raycast(transform.position, transform.up, 1.5f))
                    {
                        targetCameraHeight = m_cameraHeight;
                        targetPlayerHeight = m_standHeight;
                        m_standing = true;
                    }
                }
            }
            if (Math.Abs(m_CharacterController.height - targetPlayerHeight) > 0)
            {
                transform.position += Vector3.up * 0.04f;
                crouchTime += 4f * Time.deltaTime;
                m_CharacterController.height = Mathf.Lerp(playerHeight, targetPlayerHeight, crouchTime);
                m_Camera.transform.localPosition = Vector3.Lerp(Vector3.up * cameraHeight, Vector3.up * targetCameraHeight, crouchTime);
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0f || Input.GetKeyDown(KeyCode.T))
            {
                m_Inventory.IncrementCurrentItem();
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f || Input.GetKeyDown(KeyCode.R))
            {
                m_Inventory.DecrementCurrentItem();
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                m_InventoryDisplay.ToggleInventory();
            }



            RaycastHit hit;
            if (Physics.Raycast(m_Camera.transform.position, m_Camera.transform.forward, out hit) && (m_Camera.transform.position - hit.point).sqrMagnitude < m_MaxDistanceCheck * m_MaxDistanceCheck)
            {
                m_Highlighter.highlight(hit);

                if (Input.GetMouseButtonDown(0))
                    LeftClick(hit);


                if (Input.GetMouseButtonDown(1))
                    RightClick(hit);

                if (Input.GetMouseButton(1))
                {
                    HoldRightClick(hit);
                }
            }
            else
            {
                m_Highlighter.highlight(null);
            }
        }

        private void HoldRightClick(RaycastHit hit)
        {
            GameObject colliderGameObject = hit.collider.gameObject;
            Transform colliderTransform = hit.collider.transform;

            if (colliderGameObject.layer == LayerConstants.Door)
            {
                DoorManager door = colliderTransform.parent.GetComponent<DoorManager>();
                if (door == null)
                {
                    Debug.Log("DoorManager Not Found for " + colliderTransform.name);
                }
                else
                {
                    int LockpickVaule = -1;

                    Item item = m_Inventory.GetCurrentItem();

                    if (item != null && item.itemType == Item.ItemType.LOCKPICK && m_InventoryDisplay.IsInventoryOpen())
                    {
                        LockpickVaule = item.Value;

                        if (!door.IsOpen())
                        {
                            door.PickDoor(LockpickVaule);
                        }
                    }
                }
            }
        }

        private void LeftClick(RaycastHit hit)
        {
            GameObject colliderGameObject = hit.collider.gameObject;
            Transform colliderTransform = hit.collider.transform;
            if (colliderGameObject.layer == LayerConstants.Dial)
            {
                Dial _dial = colliderTransform.GetComponent<Dial>();
                if (_dial != null)
                {
                    StartCoroutine(_dial.DialUp());
                }
            }
        }

        private void RightClick(RaycastHit hit)
        {
            GameObject colliderGameObject = hit.collider.gameObject;
            Transform colliderTransform = hit.collider.transform;
            if (colliderGameObject.layer == LayerConstants.Door)
            {
                DoorManager door = colliderTransform.parent.GetComponent<DoorManager>();
                if (door == null)
                {
                    Debug.Log("DoorManager Not Found for " + colliderTransform.name);
                }
                else
                {
                    int keyValue = -1;

                    Item item = m_Inventory.GetCurrentItem();

                    if (item != null && item.itemType == Item.ItemType.KEY && m_InventoryDisplay.IsInventoryOpen())
                        keyValue = item.Value;

                    if (!door.IsOpen())
                    {
                        door.OpenDoor(keyValue);
                    }
                    else if (door.IsOpen())
                    {
                        door.CloseDoor();
                    }
                }
            }
            else if (colliderGameObject.layer == LayerConstants.Switch)
            {
                Switch _switch = colliderTransform.GetComponent<Switch>();
                if (_switch != null)
                {
                    _switch.Toggle();
                }
            }
            else if (colliderGameObject.layer == LayerConstants.Dial)
            {
                Dial _dial = colliderTransform.GetComponent<Dial>();
                if (_dial != null)
                {
                    StartCoroutine(_dial.DialDown());
                }
            }

        }

        private void GetMovmentInput(out float speed)
        {
            // Read input
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            bool waswalking = m_IsWalking;

            // keep track of whether or not the character is walking or running
            //Can only run while standing
            if (m_standing && Input.GetKey(KeyCode.LeftControl))
            {
                m_MovingType = MovingType.RUN;
            }
            else
            {
                m_IsWalking = true;
                m_MovingType = MovingType.WALK;
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                m_MovingType = MovingType.SLOWWALK;
            }

            // set the desired speed to be slow walking, walking, or running
            switch (m_MovingType)
            {
                case MovingType.SLOWWALK:
                    speed = m_SlowWalkSpeed;
                    break;
                case MovingType.WALK:
                    speed = m_WalkSpeed;
                    break;
                case MovingType.RUN:
                    speed = m_RunSpeed;
                    break;
                default:
                    speed = 1;
                    break;
            }

            //if we are crouched modify speed
            if (!m_standing)
            {
                speed *= 0.5f;
            }

            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }

        private void RotateView()
        {
            m_MouseLook.LookRotation(transform, m_Camera.transform);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }
            if (m_CollisionFlags == CollisionFlags.Above && m_MoveDir.y > 0)
            {
                m_MoveDir = new Vector3(m_MoveDir.x, 0, m_MoveDir.z);
                return;
            }

            Rigidbody body = hit.collider.attachedRigidbody;
            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }

        bool isClimbingLadder = false;
        bool isCollidingWithLadder = false;
        Transform ladderTransform;
        public float climbingSpeed;

        private void ToggleLadderClimbing(bool isEnabled)
        {
            canClimbLadder = isEnabled;
            isClimbingLadder = isEnabled;
            m_IsMovementEnabled = !isEnabled;
        }

        private void OnTriggerEnter(Collider hit)
        {
            if (hit.gameObject.layer == LayerConstants.Ladder)
            {
                isCollidingWithLadder = true;
                ladderTransform = hit.transform;
            }
        }

        /// If not colliding with ladder anymore set the flag to false.
        private void OnTriggerExit(Collider hit)
        {
            if (hit.gameObject.layer == LayerConstants.Ladder)
            {
                isCollidingWithLadder = false;
            }
        }
    }
}
