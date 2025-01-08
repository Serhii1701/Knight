using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    Health healthScript;
    [SerializeField] Canvas healthBar;
    [SerializeField] Image healthBarImage;
    Camera mainCamera;
    
    void Start()
    {
        healthScript = gameObject.transform.GetComponentInParent<Health>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        healthBar.transform.rotation = Quaternion.LookRotation(healthBar.transform.position - mainCamera.transform.position);
    }

    public void UpdateHealthbarValue()
    {
        healthBarImage.fillAmount = (float)healthScript.CurrentHealth / healthScript.maxHealth;
    }
    
}
