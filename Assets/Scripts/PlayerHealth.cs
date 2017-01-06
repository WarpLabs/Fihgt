using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerHealth : Health {

	public override void OnDeath()
	{
		base.OnDeath ();

		GameManager.instance.OnPlayerDeath (gameObject.GetComponent<PlayerControl>());
	}

}
