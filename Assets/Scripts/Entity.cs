using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    protected float speed;
    protected virtual void MoveEntity(Vector3 moveDirection, CharacterController characterController)
    {
        if (characterController != null)
        {
            characterController.Move(moveDirection * speed * Time.deltaTime);
        }
    }

    protected abstract IEnumerator Attack(string attackLayer);

    protected abstract void Block();
    
}
