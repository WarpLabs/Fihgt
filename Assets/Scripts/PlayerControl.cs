using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {

    public string Jump;
	public string Left;
	public string Right;

	public string AttackKey;
	public string DefenseKey;

	public LayerMask GroundLayer;

	public float BaseStaminaRecovery;

	public float JumpStaminaCost;
	public float PunchStaminaCost;
	public float KickStaminaCost;
	public float BlockStaminaCost;

	[HideInInspector] public bool Grounded;
	[HideInInspector] public bool Punching = false;
	[HideInInspector] public bool Kicking = false;
	[HideInInspector] public bool Blocking = false;

	[HideInInspector] public bool ControlEnabled = false;

	[HideInInspector] public int SpritesCharacter = 0;

	private bool Recoil = false;

	private PlayerMovement Movement;
	[HideInInspector] public PlayerHealth Health;
	[HideInInspector] public Mana Mana;
	private PlayerAttack Attack;
	private Rigidbody2D rb2d;
	private Animator Animator;
	private BoxCollider2D Bounds;
    private SpriteRenderer Renderer;

    private int Status = 0;

	private float JumpCheckGroundedDelay = 0f;

	void Start () {

		Movement = GetComponent<PlayerMovement> ();
		Health = GetComponent<PlayerHealth> ();
		Mana = GetComponent<Mana> ();
		Attack = GetComponent<PlayerAttack> ();
		rb2d = GetComponent<Rigidbody2D> ();
		Animator = GetComponent<Animator> ();
		Bounds = GetComponent<BoxCollider2D> ();
        Renderer = GetComponent<SpriteRenderer>();

	}

	public void EnableControl() {

		ControlEnabled = true;

	}

	public void DisableControl() {

		ControlEnabled = false;
		if (!Health.Dead) {
			Bounds.offset = new Vector2 (-0.05f, -0.15f);
			Bounds.size = new Vector2 (0.7f, 1.7f);
		}

	}

	void Update () {

		Status = 0;

		if (ControlEnabled)
			Controls ();
		else
			Status = -1;
		
		switch (Status) {

		case -1: // knockback

			break;

		case 0: // idle
			Movement.Move ();

			UseStamina (-BaseStaminaRecovery * Time.deltaTime);

			Animator.SetBool ("isMoving", false);
			Animator.SetBool ("isFalling", false);
			break;

		case 1: // moving
			Movement.Move ();

			UseStamina (-BaseStaminaRecovery * Time.deltaTime * 0.3f);

			if (Grounded)
				Animator.SetBool ("isMoving", true);
			break;

		case 2: // jump
			Movement.Move ();
			Movement.Jump ();

			JumpCheckGroundedDelay = 0.05f;

			UseStamina (JumpStaminaCost);

			Grounded = false;
			Animator.SetBool ("isMoving", false);
			Animator.SetTrigger ("jump");
			break;

		case 3: // punch
			Movement.Move (false, Attack.PunchSpeed, false);

			UseStamina (PunchStaminaCost);

			ResetAnimator ();

			Punching = true;	
			Animator.SetTrigger ("punch");

			Attack.Punch ();

			break;

		case 4: // punching
			Movement.Move (false, Attack.PunchSpeed, false);

			break;

		case 5: //kick
			rb2d.velocity = Vector2.zero;
			Movement.Move (false, Attack.KickSpeed, false);

			UseStamina (KickStaminaCost);

			ResetAnimator ();

			Kicking = true;	
			Animator.SetTrigger ("kick");

			Attack.Kick ();

			break;

		case 6: // kicking
			Movement.Move (false, Attack.KickSpeed, false);

			break;

		case 7: // block
			rb2d.velocity = Vector2.zero;

			UseStamina (BlockStaminaCost);

			ResetAnimator ();

			Blocking = true;
			Animator.SetTrigger ("block");

			Attack.Block ();

			break;

		case 8: // blocking
			rb2d.velocity = Vector2.zero;

			break;
	
		}
			
		CheckGrounded ();


	}

	void Controls () {

		if (Input.GetKey (Left) || Input.GetKey (Right))
			Status = 1;

		if (Input.GetKeyDown (Jump) && Grounded && Mana.CurrentMana >= JumpStaminaCost)
			Status = 2;

		if (Input.GetKeyDown (AttackKey) && Grounded && Mana.CurrentMana >= PunchStaminaCost)
			Status = 3;

		if (Punching)
			Status = 4;

		if (Input.GetKeyDown (AttackKey) && !Grounded && Mana.CurrentMana >= KickStaminaCost)
			Status = 5;

		if (Kicking)
			Status = 6;

		if (Input.GetKeyDown (DefenseKey) && Grounded && Mana.CurrentMana >= BlockStaminaCost)
			Status = 7;

		if (Blocking)
			Status = 8;

		if (Recoil) 
			Status = -1;

	}

	public void CheckGrounded () {

		if (JumpCheckGroundedDelay > 0) {
			JumpCheckGroundedDelay -= Time.deltaTime;
			return;
		}

		RaycastHit2D hit;

		Vector2 center = rb2d.position + Bounds.offset;

		hit = Physics2D.Linecast (new Vector2 (center.x - Bounds.size.x/2, center.y), new Vector2 (center.x - Bounds.size.x, center.y - Bounds.size.y/2 - 0.1f), GroundLayer);
		if (hit.transform == null)
			hit = Physics2D.Linecast (new Vector2 (center.x + Bounds.size.x/2, center.y), new Vector2 (center.x + Bounds.size.x, center.y - Bounds.size.y/2 - 0.1f), GroundLayer);


		if (hit.transform != null && !Grounded) { //land

			if (ControlEnabled) {
				Bounds.offset = new Vector2 (-0.05f, -0.15f);
				Bounds.size = new Vector2 (0.7f, 1.7f);
			}

			Grounded = true;
			Animator.SetTrigger ("land");
			Animator.SetBool ("isFalling", false);

		}
		else if (hit.transform == null) { //falling

			if (ControlEnabled) {
				Bounds.offset = new Vector2 (-0.05f, -0.05f);
				Bounds.size = new Vector2 (0.7f, 1.3f);
			}

			Grounded = false;
			Animator.SetBool ("isMoving", false);
			Animator.SetBool ("isFalling", true);

		}

	}

	public void TakeHit (float RecoilTime, float damage, Vector2 knockbackForce, GameObject attacker) {

		if (Blocking) {
			rb2d.velocity = knockbackForce / Attack.BlockStrength;
			attacker.GetComponent<PlayerControl> ().UseStamina (Mana.StartMana);
		} else {
			Health.TakeDamage (damage);
			rb2d.velocity = knockbackForce;
		}

		if (knockbackForce.x < 0 && transform.right.x < 0)
			transform.rotation = Quaternion.Euler (0f, 0f, 0f);
		else if (knockbackForce.x > 0 && transform.right.x > 0)
			transform.rotation = Quaternion.Euler (0f, 180f, 0f);

		TakeRecoil (RecoilTime);

	}

    public void TakeRecoil(float RecoilTime)
    {

        ResetAnimator();
        Recoil = true;

        if (!Blocking)
        {
            Renderer.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);

        }

        Invoke("ResetAnimator", RecoilTime);

    }

    public void UseStamina (float Usage) {

		Mana.UseMana (Usage);

	}

    public void ResetAnimator()
    {

        Recoil = false;
        Punching = false;
        Kicking = false;
        Blocking = false;

        Renderer.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        Animator.SetBool("isMoving", false);
        Animator.SetBool("isFalling", false);

        Animator.ResetTrigger("punch");
        Animator.ResetTrigger("jump");
        Animator.ResetTrigger("land");
        Animator.ResetTrigger("dance");

        Animator.SetTrigger("reset");

        Animator.SetTrigger("land");

    }

    public void Death () {

		DisableControl ();
		Animator.SetTrigger ("death");
		Bounds.offset = new Vector2 (-0.65f, -0.995f);
		Bounds.size = new Vector2 (1.7f, 0.01f);

	}

	public void Reset () {

		Animator.SetTrigger ("revive");
		Bounds.offset = new Vector2 (-0.05f, -0.15f);
		Bounds.size = new Vector2 (0.7f, 1.7f);

	}

	public void SwitchAnimationSet (AnimatorOverrideController AnimationSet) {

		Animator.runtimeAnimatorController = AnimationSet;

	}

}
