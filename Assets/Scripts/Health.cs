using System.Collections;
using System.Collections.Generic;
//using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour, IDataPersistence
{
    public int CurrentHealth { get; private set; } = 10;
    public int maxHealth;

    public bool isDie;

    Animator animator;
    Damage damage;
    Strength strengthScript;
    HealthBar healthBar;
    Human human;

    AudioSource audioSource;
    [SerializeField] AudioClip takeDamageSound;
    [SerializeField] AudioClip metalSound;
    [SerializeField] Slider sliderEffects;
 
    Quaternion fallDirection;
    readonly float rotationSpeed = 20f;

    readonly string deathLayer = "Death Layer";
    readonly string getHitLayer = "GetHit Layer";
    readonly string attackAreaTag = "Attack Area";
    readonly string getHitTrigger = "GetHit";
    readonly string isDieBoolAnimator = "isDie";
    readonly string npc_HumanEnemyTag = "NPC_Human_Enemy";

    public int EnemiesKilled { get; private set; } = 0;

    private void Awake()
    {
        human = GetComponent<Human>();
        audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        animator = GetComponent<Animator>();
        damage = GetComponent<Damage>();
        healthBar = GetComponent<HealthBar>();
        fallDirection = Quaternion.Euler(-80, 0, 0);
        if (CurrentHealth <= 0)
        {
            Die();
        }
        healthBar.UpdateHealthbarValue();
        audioSource.volume = sliderEffects.value;
    }

    public void LoadData(GameData data)
    {
        if (gameObject.CompareTag(npc_HumanEnemyTag))
        {
            if (data.enemiesKilledDictionary.TryGetValue(human.GetHumanId(), out bool isKilled))
            {
                isDie = isKilled;
            }
        }

        if (data.playersHealth.TryGetValue(human.GetHumanId(), out int characterHealth))
        {
            CurrentHealth = characterHealth;
            Debug.Log(CurrentHealth);
        }
        else if (data == null)
        {
            CurrentHealth = maxHealth;
        }
    }

    public void SaveData(GameData data)
    {
        if (gameObject.CompareTag(npc_HumanEnemyTag))
        {
            if (data.enemiesKilledDictionary.ContainsKey(human.GetHumanId()))
            {
                data.enemiesKilledDictionary.Remove(human.GetHumanId());
            }
            data.enemiesKilledDictionary.Add(human.GetHumanId(), isDie);
        }

        if (data.playersHealth.ContainsKey(human.GetHumanId()))
        {
            data.playersHealth.Remove(human.GetHumanId());
        }
        data.playersHealth.Add(human.GetHumanId(), CurrentHealth);
    }

    void TakeDamage(int amount)
    {
        if (CurrentHealth - amount > 0)
        {
            CurrentHealth -= amount;
        } else
        {
            CurrentHealth = 0;
            Die();
        }
    }

    void Heal(int amount)
    {
        if (CurrentHealth + amount < maxHealth)
        {
            CurrentHealth += amount;
        } else
        {
            CurrentHealth = maxHealth;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(attackAreaTag) && IsNotChildOf(other.gameObject, gameObject))
        {
            if (!animator.GetBool("Block"))
            {
                strengthScript = other.GetComponentInParent<Strength>();
                TakeDamage(damage.DefineDamage(strengthScript.strength));
                audioSource.volume = sliderEffects.value;
                audioSource.PlayOneShot(takeDamageSound);
                StartCoroutine(GetHit());
                healthBar.UpdateHealthbarValue();
            }
            else if (animator.GetBool("Block"))
            {
                audioSource.volume = sliderEffects.value;
                audioSource.PlayOneShot(metalSound);
            }
        }
    }

    void Die()
    {
        isDie = true;
        animator.SetLayerWeight(animator.GetLayerIndex(deathLayer), 1);
        animator.SetBool(isDieBoolAnimator, isDie);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, fallDirection, rotationSpeed);
        CountEnemiesKilled();
    }

    private void CountEnemiesKilled()
    {
        if (isDie && gameObject.CompareTag("NPC_Human_Enemy"))
        {
            EnemiesKilled++;
        }
    }

    IEnumerator GetHit()
    {
        if (animator.GetLayerWeight(animator.GetLayerIndex(getHitLayer)) == 0)
        {
            animator.SetLayerWeight(animator.GetLayerIndex(getHitLayer), 1);
            animator.SetTrigger(getHitTrigger);

            yield return new WaitForSeconds(0.7f);

            animator.SetLayerWeight(animator.GetLayerIndex(getHitLayer), 0);
        }
    }

    bool IsNotChildOf(GameObject child, GameObject parent)
    {
        Transform currentGameObject = child.transform;

        while (currentGameObject != null)
        {
            if (currentGameObject == parent.transform)
            {
                return false;
            }
            currentGameObject = currentGameObject.parent;
        }
        return true; ;
    }
}
