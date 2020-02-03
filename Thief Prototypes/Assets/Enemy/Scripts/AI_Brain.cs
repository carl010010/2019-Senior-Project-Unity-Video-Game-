using System.Collections;
using System.Collections.Generic;
using GizmosEditors;
using UnityEngine;

[RequireComponent(typeof(AI_Movement))]
[RequireComponent(typeof(AI_Eyes))]
public class AI_Brain : MonoBehaviour
{

    public static class BlackBoard
    {
        static BlackBoard()
        {
            Init();
        }

        static public void Init()
        {
            b_player = GameObject.FindGameObjectWithTag("Player");

            if (b_player == null)
            {
                Debug.LogError("AI BlackBoard couldn't find a player");
            }
            else
            {
                b_PlayerShadowAmount = b_player.GetComponent<DistanceToLights>();
                if (b_PlayerShadowAmount == null)
                {
                    Debug.LogError("AI BlackBoard couldn't find DistanceToLights component on Player");
                }
            }
        }

        public static List<AI_Brain> b_AIs = new List<AI_Brain>();
        public static GameObject b_player;
        public static DistanceToLights b_PlayerShadowAmount;

        public static List<SoundStim> b_sounds = new List<SoundStim>();
    }

    public float AlertDistance = 15;
    public float AlertStateDecayTime = 50;

    public float ProximityDistance;
    public float ImmediateDetectionRadius;

    [Space]
    [SerializeField]
    private float AlertStateTimer;

    private enum AlertState { OFF, LOW, MEDIUM, HIGH };
    [SerializeField]
    private AlertState m_AlertState;
    private AI_Movement m_Movement;
    private AI_Eyes m_Eyes;

    private bool CanBeAlertedByAI;

    private Vector3 LastKnownSearchPostion;
    private Vector3 playerLastKnowVelocity;

    private bool wasVisable;

    private Vector3 PrevPos;
    private Vector3 NewPos;
    private Vector3 ObjVelocity;

    [Space]
    [SerializeField]
    private float TimeHearingSound;

    private void Start()
    {
        //base.Initialize();

        BlackBoard.b_AIs.Add(this);
        m_Movement = GetComponent<AI_Movement>();
        m_Eyes = GetComponent<AI_Eyes>();
        if (BlackBoard.b_player == null)
        {
            BlackBoard.Init();
        }

        PrevPos = BlackBoard.b_player.transform.position;
        NewPos = PrevPos;

        CanBeAlertedByAI = true;
    }

    private void OnDestroy()
    {
        BlackBoard.b_AIs.Remove(this);
    }

    void FixedUpdate()
    {
        NewPos = BlackBoard.b_player.transform.position;  // each frame track the new position
        ObjVelocity = (NewPos - PrevPos) / Time.fixedDeltaTime;  // velocity = dist/time
        PrevPos = NewPos;  // update position for next frame calculation
    }

    private void Update()
    {
        bool goalVisable = m_Eyes.CanSeeGoal(BlackBoard.b_player.transform.position);
        bool canSeePlayer = false;

        //If in range to the player
        float DistanceToPlayer = (transform.position - BlackBoard.b_player.transform.position).sqrMagnitude;
        float viewDistancesqr = m_Eyes.viewDistance * m_Eyes.viewDistance;
        if (DistanceToPlayer < viewDistancesqr)
        {
            //TODO Consider player height crouching or not
            //If can see player
            if (goalVisable)
            {
                //Consider how far away the player is
                float lightPercentage = BlackBoard.b_PlayerShadowAmount.LightPercentage;
                if (DistanceToPlayer >= viewDistancesqr * .2f)
                {
                    float normal = Mathf.InverseLerp(0, 1 - 1 * .2f, 1.0f - (DistanceToPlayer / viewDistancesqr));
                    float temp = Mathf.Lerp(0, 1, normal);
                    lightPercentage = Mathf.Max(lightPercentage * temp, lightPercentage * .6f);
                }

                if (lightPercentage > 75)
                {
                    ChangeAlertState(AlertState.HIGH);
                    canSeePlayer = true;
                }
                else if (lightPercentage > 50)
                {
                    ChangeAlertState(AlertState.MEDIUM);
                    canSeePlayer = true;
                }
                else if (lightPercentage > 25)
                {
                    ChangeAlertState(AlertState.LOW);
                }
            }
        }

        //Can hear player
        //Set Alerted Level
        GameObject HeardObject = null;

        for (int i = 0, BlackBoardb_soundsCount = BlackBoard.b_sounds.Count; i < BlackBoardb_soundsCount; i++)
        {
            SoundStim s = BlackBoard.b_sounds[i];
            if ((transform.position - s.transform.position).sqrMagnitude < s.GetRadius() * s.GetRadius())
            {
                HeardObject = s.gameObject;
                break;
            }
        }

        if (HeardObject != null)
        {
            TimeHearingSound += Time.deltaTime;
        }
        else
        {
            TimeHearingSound = 0;
        }

        //Do we need to account for how loud the sound is?
        if (TimeHearingSound > 0.55)
        {
            ChangeAlertState(AlertState.HIGH);
        }
        else if (TimeHearingSound > 0.35)
        {
            ChangeAlertState(AlertState.MEDIUM);
        }
        else if (TimeHearingSound > 0.15)
        {
            ChangeAlertState(AlertState.LOW);
        }


        //Player in AI proximity and AlertState Low or Higher
        //Set Alerted Level
        bool playerInProximity = false;

        if (DistanceToPlayer < ProximityDistance * ProximityDistance)
        {
            playerInProximity = true;
            if (m_AlertState == AlertState.MEDIUM)
            {
                ChangeAlertState(AlertState.MEDIUM);
            }

            if (DistanceToPlayer < ImmediateDetectionRadius * ImmediateDetectionRadius)
            {
                ChangeAlertState(AlertState.HIGH);
            }
        }


        if (HeardObject != null && !canSeePlayer && !playerInProximity)
        {
            LastKnownSearchPostion = HeardObject.transform.position;
        }




        //If on high alert, Alert nearby guards
        //With players postion, or last know position
        AlertNearByAI();


        if (!pause)
        {
            if ((canSeePlayer || playerInProximity) && (m_AlertState == AlertState.MEDIUM || m_AlertState == AlertState.HIGH))
            {
                //TODO Change agent speed
                m_Movement.UpdateAgentDestination(BlackBoard.b_player.transform.position);
            }
            else if (m_AlertState == AlertState.MEDIUM || m_AlertState == AlertState.HIGH)
            {
                m_Movement.UpdateAgentDestination(LastKnownSearchPostion);

                if (!m_Movement.agent.pathPending && m_Movement.agent.path.corners.Length < 1)
                {
                    //Look at last know player locatation
                    Vector3 targetDir = (LastKnownSearchPostion + playerLastKnowVelocity) - transform.position;

                    // The step size is equal to speed times frame time.
                    float step = 4 * Time.deltaTime;

                    Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
                    Debug.DrawRay(transform.position, newDir, Color.red);

                    // Move our position a step closer to the target.
                    transform.rotation = Quaternion.LookRotation(newDir);
                }


                //TODO Search for player
                Debug.Log(name + " is searching for the player");
            }
            else
            {
                //TODO Set Agent speed back to default
                m_Movement.Guard(wasVisable);
            }
        }

        //TODO If Ai Alerted, make noise

        if (AlertStateTimer > 0)
        {
            AlertStateTimer -= Time.deltaTime;

            if (m_AlertState == AlertState.HIGH && AlertStateTimer < (AlertStateDecayTime / 1.5))
            {
                m_AlertState--;
                //m_AlertState = (AlertState)(((int)m_AlertState) - 1);
            }
            else if (m_AlertState == AlertState.MEDIUM && AlertStateTimer < (AlertStateDecayTime / 2.5))
            {
                m_AlertState--;
                //m_AlertState = (AlertState)(((int)m_AlertState) - 1);
            }
        }
        else
        {
            //TODO Makes noise to let the player know the enemy lost interest
            m_AlertState = AlertState.OFF;
            LastKnownSearchPostion = Vector3.zero;
            playerLastKnowVelocity = Vector3.zero;
            CanBeAlertedByAI = true;
        }

        wasVisable = canSeePlayer;
    }

    public void AI_Hit(Vector3 hitPoint)
    {
        if (m_AlertState == AlertState.OFF || m_AlertState == AlertState.LOW)
        {

            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.AddForceAtPosition(hitPoint - transform.position, hitPoint);
            m_Movement.agent.enabled = false;
            this.enabled = false;
        }
    }


    private void AlertNearByAI()
    {
        if (m_AlertState == AlertState.HIGH)
        {
            foreach (AI_Brain ai in BlackBoard.b_AIs)
            {
                if (ai != this && ai.CanBeAlertedByAI)
                {
                    float DistanceToAI = (transform.position - ai.transform.position).sqrMagnitude;
                    if (DistanceToAI <= (AlertDistance * AlertDistance) * 1.5)
                    {

                        if (DistanceToAI <= AlertDistance * AlertDistance)
                        {
                            ai.ChangeAlertStateForAlly(AlertState.HIGH);
                        }
                        else
                        {
                            ai.ChangeAlertStateForAlly(AlertState.MEDIUM);
                        }

                        ai.UpdatePlayerLastKnowLocation(LastKnownSearchPostion, playerLastKnowVelocity);

                    }
                }
            }
        }
    }

    void UpdatePlayerLastKnowLocation(Vector3 pos, Vector3 velocity)
    {
        LastKnownSearchPostion = pos;
        playerLastKnowVelocity = velocity;
    }

    void ChangeAlertStateForAlly(AlertState state)
    {
        if (CanBeAlertedByAI)
        {
            ChangeAlertState(state);
            CanBeAlertedByAI = false;
            AlertStateTimer = AlertStateDecayTime;
        }
    }

    void ChangeAlertState(AlertState state)
    {
        if (state == AlertState.HIGH)
        {
            AlertStateTimer = AlertStateDecayTime;
            LastKnownSearchPostion = BlackBoard.b_player.transform.position;
            playerLastKnowVelocity = ObjVelocity;
            m_AlertState = AlertState.HIGH;
        }
        else if (state == AlertState.MEDIUM && (m_AlertState != AlertState.HIGH))
        {
            AlertStateTimer = AlertStateDecayTime * 0.8f;
            LastKnownSearchPostion = BlackBoard.b_player.transform.position;
            playerLastKnowVelocity = ObjVelocity;
            m_AlertState = AlertState.MEDIUM;
        }
        else if (state == AlertState.LOW && (m_AlertState == AlertState.OFF))
        {
            AlertStateTimer = AlertStateDecayTime * 0.5f;
            m_AlertState = AlertState.LOW;
        }
    }


    [GizmoMethod]
    private void DrawAlertState()
    {
        Gizmos.color = Color.white;
        if (m_AlertState != AlertState.OFF)
        {
            switch (m_AlertState)
            {
                case AlertState.LOW:
                    Gizmos.color = Color.gray;
                    break;
                case AlertState.MEDIUM:
                    Gizmos.color = Color.yellow;
                    break;
                case AlertState.HIGH:
                    Gizmos.color = Color.red;
                    break;
                default:
                    Gizmos.color = Color.magenta;
                    break;
            }
        }
        Gizmos.DrawWireSphere(transform.position + (Vector3.up * 2), .5f);

        Gizmos.color = Color.white;
        Utils.DebugUtilities.DrawCircle(transform.position, ProximityDistance);

        Gizmos.color = Color.red;
        Utils.DebugUtilities.DrawCircle(transform.position, ImmediateDetectionRadius);

        Gizmos.color = Color.gray;
        Utils.DebugUtilities.DrawCircle(transform.position, AlertDistance);

    }

    [GizmoMethod]
    private void DrawLastKnowPlayerLocation()
    {
        if (LastKnownSearchPostion != Vector3.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(LastKnownSearchPostion, 1);
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(LastKnownSearchPostion + playerLastKnowVelocity * .5f, 1);
        }
    }


    public bool pause;

}
