using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour {

	public GameObject Fists;
	public float PunchDamage;
	public float PunchKnockback;
	public float PunchKnockup;
	public float PunchStun;
	public float PunchLength;
	public float PunchSpeed;

	public GameObject Feet;
	public float KickDamage;
	public float KickKnockback;
	public float KickKnockup;
	public float KickStun;
	public float KickSpeed;

	public float BlockLength;
	public float BlockStrength;
	public float BlockStun;

	public LayerMask TargetLayer;

	private PlayerControl PControl;

	void Start() {

		PControl = GetComponent<PlayerControl> ();

	}

	public void Punch() {
		StartCoroutine (PunchCoroutine ());
	}

	IEnumerator PunchCoroutine () {

		float count = 0.0f;

		while (count < PunchLength) {

			bool didHit = false;

			RaycastHit2D[] hits;
			hits = Physics2D.LinecastAll (Fists.transform.position, Fists.transform.position + (transform.right * 0.1f), TargetLayer);

			foreach (RaycastHit2D hit in hits) {

				if (hit.collider != null) {
					
					Vector2 knockback = new Vector2 (PunchKnockback * transform.right.x, PunchKnockup);
					hit.transform.gameObject.GetComponent<PlayerControl>().TakeHit (PunchStun, PunchDamage, knockback, gameObject);

					didHit = true;

				}

			}

			if (didHit) {
				break;
			}

			count += Time.deltaTime;

			yield return null;
		}

		PControl.ResetAnimator ();

		GetComponent<Rigidbody2D> ().velocity = new Vector2 (0.0f, 0.0f);

		PControl.TakeRecoil (0.1f);

	}
		
	public void Kick() {
		StartCoroutine (KickCoroutine ());
	}

	IEnumerator KickCoroutine () {

		while (!PControl.Grounded) {

			bool didHit = false;

			RaycastHit2D[] hits;
			hits = Physics2D.LinecastAll (Feet.transform.position, Feet.transform.position + (new Vector3(0.1f, 0.05f, 0.0f) * transform.right.x), TargetLayer);

			foreach (RaycastHit2D hit in hits) {

				if (hit.collider != null) {

					Vector2 knockback = new Vector2 (KickKnockback * transform.right.x, KickKnockup);
					hit.transform.gameObject.GetComponent<PlayerControl>().TakeHit (KickStun, KickDamage, knockback, gameObject);

					didHit = true;

				}

			}

			if (didHit) {
				break;
			}

			yield return null;
		}

		PControl.ResetAnimator ();

		GetComponent<Rigidbody2D> ().velocity = new Vector2 (0.0f, 0.0f);

		PControl.TakeRecoil (0.01f);

	}

	public void Block() {
		StartCoroutine (BlockCoroutine ());
	}

	IEnumerator BlockCoroutine() {

		float count = 0.0f;

		while (count < BlockLength) {

			count += Time.deltaTime;

			yield return null;
		}

		PControl.ResetAnimator ();

	}
		
}
