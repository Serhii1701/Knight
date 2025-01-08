using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    public int DefineDamage(int strength)
    {
        return strength;
    }

    public int DefineDamage(int strength, int damageFromSword)
    {
        return strength + damageFromSword;
    }
}
