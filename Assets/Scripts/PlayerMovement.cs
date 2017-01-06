using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	public float MoveSpeed;
	public float JumpVelocity;

	private Rigidbody2D rb2d;

	private PlayerControl PControl;

	void Start () {

		rb2d = GetComponent<Rigidbody2D> ();
		PControl = GetComponent<PlayerControl> ();

	}

	public void Move (bool Control = true, float horMove = 0.0f, bool UseDefaultSpeed = false, bool DirectionBased = true) {

		if (UseDefaultSpeed) {
			horMove = MoveSpeed;
		}

		if (DirectionBased) {
			float direction = transform.right.x > 0 ? 1f : -1f;
			horMove *= direction;
		}

		if (Control) {

			if (Input.GetKey (PControl.Left)) {
				horMove = -1 * MoveSpeed;
				transform.rotation = Quaternion.Euler (new Vector3 (0, 180, 0));
			} else if (Input.GetKey (PControl.Right)) {
				horMove = MoveSpeed;
				transform.rotation = Quaternion.Euler (new Vector3 (0, 0, 0));
			}

		}

		rb2d.velocity = new Vector2 (horMove, rb2d.velocity.y);

	}

	public void Jump () {

		rb2d.velocity = new Vector2 (rb2d.velocity.x, 0.0f);
		rb2d.AddForce (new Vector2 (0.0f, JumpVelocity));
	
	}
}
