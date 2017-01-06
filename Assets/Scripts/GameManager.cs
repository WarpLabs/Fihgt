using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public PlayerControl[] Players;
	public AnimatorOverrideController[] Characters;

	private List<Vector2> StartingPositions;
	private List<PlayerControl> ActivePlayers;

	public Text TextMainCenter;
	public string StartKey;

	public static GameManager instance = null;

	public bool TriggerNext = false;

	void Awake () {

		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);

		DontDestroyOnLoad (gameObject);

	}

	void Start () {

		StartingPositions = new List<Vector2> ();

		ActivePlayers = new List<PlayerControl> ();

		for (int i = 0; i<Players.Length; i++) {

			StartingPositions.Add (new Vector2 (Players [i].transform.position.x, Players [i].transform.position.y));

			ActivePlayers.Add (Players[i]);

		}

		StartCoroutine (GameLoop ());

	}

	IEnumerator GameLoop () {

		yield return null;

		while (true){
			
			yield return StartCoroutine (Menu ());
			TriggerNext = false;

			yield return StartCoroutine (PreGame ());
			TriggerNext = false;

			yield return StartCoroutine (Game ());
			TriggerNext = false;

			yield return StartCoroutine (PostGame ());
			TriggerNext = false;

			Reset ();

		}

	}

	IEnumerator Menu () {

		Debug.Log ("GameManager -> Menu!");

		DisableAllPlayers ();

		TextMainCenter.text = " FIHGT";
		TextMainCenter.fontSize = 120;

		while (!TriggerNext) {

			if (Input.GetKeyDown (StartKey))
				TriggerNext = true;

			yield return null;

		}

	}

	IEnumerator PreGame () {

		Debug.Log ("GameManager -> PreGame!");

		TextMainCenter.text = "CHARACTER SELECT";
		TextMainCenter.fontSize = 50;
        

		while (!TriggerNext) {

			if (Input.GetKeyDown (StartKey))
				TriggerNext = true;

			foreach (PlayerControl player in Players) {

				if (Input.GetKeyDown (player.Left) || Input.GetKeyDown (player.Right)) {

					if (Input.GetKeyDown (player.Left))
						player.SpritesCharacter--;
					if (Input.GetKeyDown (player.Right))
						player.SpritesCharacter++;

					if (player.SpritesCharacter < 0)
						player.SpritesCharacter = Characters.Length - 1;
					else if (player.SpritesCharacter >= Characters.Length)
						player.SpritesCharacter = 0;


					player.SwitchAnimationSet (Characters [player.SpritesCharacter]);
				}

			}

			yield return null;

		}

	}

	IEnumerator Game () {

		Debug.Log ("GameManager -> Game!");

		TextMainCenter.fontSize = 150;
		TextMainCenter.text = "3";

		yield return new WaitForSeconds (1.0f);

		TextMainCenter.text = "2";

		yield return new WaitForSeconds (1.0f);

		TextMainCenter.text = "1";

		yield return new WaitForSeconds (1.0f);

        TextMainCenter.fontSize = 120;
		TextMainCenter.text = " FIHGT";
		
		EnableAllPlayers ();

		yield return new WaitForSeconds (1.0f);

		TextMainCenter.text = "";

		while (!TriggerNext) {


			yield return null;

		}

	}

	IEnumerator PostGame () {

		Debug.Log ("GameManager -> PostGame!");

		yield return new WaitForSeconds (5.0f);

		Reset ();

	}

	void Reset () {

		ActivePlayers.Clear ();

		for (int i = 0; i<Players.Length; i++) {

			ActivePlayers.Add (Players[i]);

			Players [i].Reset ();

			Players [i].Health.Reset ();
			Players [i].Mana.Reset ();
			Players [i].transform.position = StartingPositions [i];

		}

	}

	public void OnPlayerDeath (PlayerControl deadPlayer) {

		ActivePlayers.Remove (deadPlayer);

		deadPlayer.Death ();

		if (ActivePlayers.Count == 1) {

			GameOver ();

		}

	}

	void GameOver() {

		TriggerNext = true;

		DisableAllPlayers ();
		TextMainCenter.text = ActivePlayers [0].gameObject.name.ToUpper() + " WINS";
		TextMainCenter.fontSize = 50;

	}

	void EnableAllPlayers() {
		foreach (PlayerControl player in Players) {
			player.EnableControl ();
		}
	}

	void DisableAllPlayers() {
		foreach (PlayerControl player in Players) {
			player.DisableControl ();
		}
	}

}
