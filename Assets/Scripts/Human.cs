using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Profiling;

public class Human : Entity, IDataPersistence
{
    [SerializeField] private string id;
    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    [Header("Speed Variables")]
    readonly float rotatingSpeed = 500;
    private readonly float walkSpeed = 2;
    private readonly float runSpeed = 5;

    private readonly float maintainDistance = 2.3f;

    [Header("Layer Variables")]
    string attack;
    readonly string attackLayer = "Attack Layer";
    readonly string attackInMovingLayer = "Attack In Moving Layer";
    readonly string blockLayer = "Block Layer";
    readonly string deathLayer = "Death Layer";
    readonly string getHitLayer = "GetHit Layer";

    [Header("Tag Variables")]
    readonly string playerTag = "Player";
    readonly string npc_HumanEnemyTag = "NPC_Human_Enemy";

    [Header("Variables for gravity")]
    float ySpeed;
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] Vector3 groundCheckOffset;
    [SerializeField] LayerMask groundLayer;
    bool isGrounded;

    PlayerController playerController;
    CharacterController characterController;
    Animator animator;
    GameObject player;
    [SerializeField] GameObject sword;
    Collider swordCollider;

    [Header("Rotation Variables")]
    Quaternion targetRotation;
    Vector3 moveDirection;

    bool isWalk;
    bool isAttack;

    [Header("Attributes SO")]
    [SerializeField] AttributesScriptableObject playerAttributesSO;

    private void Awake()
    {
        Application.targetFrameRate = 1000;
        QualitySettings.vSyncCount = 0;
    }
    private void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        speed = 5;
        player = GameObject.Find("Player");
        swordCollider = sword.GetComponent<Collider>();
    }

    public void LoadData(GameData data)
    {
        // load the values from our game data into the scriptable object
        playerAttributesSO.vitality = data.playerAttributesData.vitality;
        playerAttributesSO.strength = data.playerAttributesData.strength;
        playerAttributesSO.intellect = data.playerAttributesData.intellect;
        playerAttributesSO.endurance = data.playerAttributesData.endurance;

        if (data.playersPosition.TryGetValue(id, out var position))
        {
            transform.position = position;
        }
        if (data.playersRotation.TryGetValue(id, out var rotation))
        {
            transform.rotation = rotation;
        }

       
    }

    public void SaveData(GameData data)
    {
        //store the values from our scriptable object into the game data
        data.playerAttributesData.vitality = playerAttributesSO.vitality;
        data.playerAttributesData.strength = playerAttributesSO.strength;
        data.playerAttributesData.intellect = playerAttributesSO.intellect;
        data.playerAttributesData.endurance = playerAttributesSO.endurance;

        if (data.playersPosition.ContainsKey(id))
        {
            Debug.Log("Contains key " + id);
            data.playersPosition.Remove(id);
        }
        data.playersPosition.Add(id, transform.position);

        if (data.playersRotation.ContainsKey(id))
        {
            data.playersRotation.Remove(id);
        }
        data.playersRotation.Add(id, transform.rotation);
    }

    private void Update()
    {
        if (gameObject.CompareTag(playerTag))
        {
            if (animator.GetLayerWeight(animator.GetLayerIndex(deathLayer)) == 0 && 
                animator.GetLayerWeight(animator.GetLayerIndex(getHitLayer)) == 0)
            {
                isAttack = playerController.CheckOnAttack();
                isWalk = playerController.CheckOnWalk();
                moveDirection = playerController.TrackPlayerInput();

                MoveHuman();
                //transform.rotation = playerController.PlanarRotation();

                ApplyAttack();
                Block();
            }
            
        } 

        if (gameObject.CompareTag(npc_HumanEnemyTag))
        {
            if (animator.GetLayerWeight(animator.GetLayerIndex(deathLayer)) == 0 &&
                animator.GetLayerWeight(animator.GetLayerIndex(getHitLayer)) == 0)
            {
                if (Vector3.Distance(transform.position, player.transform.position) < 10 &&
                    Vector3.Distance(transform.position, player.transform.position) > 2.3f)
                {
                    moveDirection = (player.transform.position - transform.position).normalized;
                }
                else if (Vector3.Distance(transform.position, player.transform.position) <= 2.3f)
                {
                    moveDirection = Vector3.zero;
                    var moveDir = (player.transform.position - transform.position).normalized 
                        * maintainDistance;
                    targetRotation = Quaternion.LookRotation(moveDir);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
                        rotatingSpeed * Time.deltaTime);
                }
                else
                {
                    moveDirection = Vector3.zero;
                }

                if (Vector3.Distance(transform.position, player.transform.position) <= 2.3f)
                {
                    isAttack = true;
                }
                else
                {
                    isAttack = false;
                }

                MoveHuman();
                ApplyAttack();
            }
            
        }
    }
    protected override void MoveEntity(Vector3 moveDirection, CharacterController characterController)
    {
        base.MoveEntity(moveDirection, characterController);
    }

    private void Idle()
    {
        speed = 0;
        animator.SetFloat("moveAmount", 0, 0.1f, Time.deltaTime);
    }

    private void Walk()
    {
        speed = walkSpeed;
        animator.SetFloat("moveAmount", 0.2f, 0.1f, Time.deltaTime);
    }

    private void Run()
    {
        speed = runSpeed;
        animator.SetFloat("moveAmount", 1, 0.1f, Time.deltaTime);
    }

    protected override IEnumerator Attack(string attackLayer)
    {
        Debug.Log("Attack coroutine applied");
        animator.SetLayerWeight(animator.GetLayerIndex(attackLayer), 1);
        animator.SetTrigger("Attack");

        swordCollider.enabled = true;
        Debug.Log("swordCollider is enabled");
        
        yield return new WaitForSeconds(0.2f);

        swordCollider.enabled = false;

        yield return new WaitForSeconds(0.7f);

        animator.SetLayerWeight(animator.GetLayerIndex(attackLayer), 0);
    }

    private void ApplyAttack()
    {
        if (isAttack && animator.GetLayerWeight(animator.GetLayerIndex(attackLayer)) == 0 &&
                animator.GetLayerWeight(animator.GetLayerIndex(attackInMovingLayer)) == 0 &&
                animator.GetLayerWeight(animator.GetLayerIndex(getHitLayer)) == 0)
        {
            SelectAttackLayer();
            StartCoroutine(Attack(attack));
        }
    }

    void SelectAttackLayer()
    {
        if (moveDirection == Vector3.zero)
        {
            attack = attackLayer;
        }
        if (moveDirection != Vector3.zero)
        {
            attack = attackInMovingLayer;
        }
    }

    protected override void Block()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {  
            animator.SetLayerWeight(animator.GetLayerIndex(blockLayer), 1);
            animator.SetBool("Block", true);
        } 
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            animator.SetBool("Block", false);
            animator.SetLayerWeight(animator.GetLayerIndex(blockLayer), 0);
        }
    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }

    void DefineGravity()
    {
        if (isGrounded)
        {
            ySpeed = -0.5f;
        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;
        }
    }

    void MoveHuman()
    {
        GroundCheck();
        DefineGravity();

        if (animator.GetLayerWeight(animator.GetLayerIndex(blockLayer)) == 0 &&
            animator.GetLayerWeight(animator.GetLayerIndex(attackLayer)) == 0 &&
            animator.GetLayerWeight(animator.GetLayerIndex(getHitLayer)) == 0)
        {
            if (isGrounded)
            {
                if (moveDirection != Vector3.zero)
                {
                    Run();
                }
                if (moveDirection != Vector3.zero && isWalk)
                {
                    Walk();
                }
                if (moveDirection == Vector3.zero)
                {
                    Idle();
                }
            }

            var velocity = moveDirection;
            velocity.y = ySpeed;
            MoveEntity(velocity, characterController);

            if (moveDirection != Vector3.zero)
            {
                targetRotation = Quaternion.LookRotation(moveDirection);
            }

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
            rotatingSpeed * Time.deltaTime);
        }
    }

    /*private void LockOnTarget()
    { 
         transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * rotatingSpeed);
    }*/

    public string GetHumanId()
    {
        return this.id;
    }
}
