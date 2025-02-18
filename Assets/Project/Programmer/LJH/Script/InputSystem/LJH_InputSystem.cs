using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LJH_InputSystem : MonoBehaviour
{
    float moveSpeed = 0.05f;
    public Vector3 direction { get; private set; }

    PlayerInput playerInput;
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();  
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        if(Input.GetKeyDown(KeyCode.M))
        playerInput.SwitchCurrentActionMap(InputType.UI);
    }

    public void Move()
    {
        gameObject.transform.Translate(direction * moveSpeed);
    }

    public void OnMove1(InputAction.CallbackContext context)
    {
        Debug.Log(context);
        Vector2 input = context.ReadValue<Vector2>();
        direction = new Vector3(input.x, 0f, input.y);
        
    }
}
