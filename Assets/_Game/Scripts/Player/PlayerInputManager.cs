using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
	PlayerInput playerInput;

	[Header("Character Input Values")]
	public Vector2 move;
	public bool jump;
	public bool attack;
	public bool shoot;

	void Awake()
	{
		playerInput = new PlayerInput();
	}

    private void OnEnable()
    {
		playerInput.Enable();
    }

    private void OnDisable()
    {
		playerInput.Disable();
    }

    private void Update()
    {
		move = playerInput.Movement.Move.ReadValue<Vector2>();
		jump = playerInput.Movement.Jump.triggered;
		attack = playerInput.Movement.Attack.triggered;
		shoot = playerInput.Movement.Shoot.triggered;
	}
}

