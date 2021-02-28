using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VIDE_Data;

public static class SceneModeManager {
	public static SceneMode sceneMode => GetSceneMode();

	public static void SetSceneMode(SceneMode mode, bool on, Character other = null) {
		switch (mode) {
			case SceneMode.normal:									// passage en mode Normal
				App.cameraController.SetFollowCamera();											// activer la caméra 'follow'
				App.itemsManager.SelectAll(true);												// resélectionner tous les objets intéractibles de la scène quand on commence un dialogue... 
				App.playerManager.StopAgent(false);												// redémarrer les déplacements
				break;

			case SceneMode.dialogue:
				// scène
				if (on && sceneMode == SceneMode.normal) {			// passage en mode Dialogue depuis le mode Normal
					App.cameraController.SetLateralCamera(other.transform);						// activer la caméra latérale
					App.itemsManager.SelectAll(false);											// désélectionner tous les objets intéractibles de la scène quand on commence un dialogue... 

					// player
					App.playerManager.FaceTo(on, other.gameObject);								// orienter le joueur vers le PNJ
					App.playerManager.animatorController?.anim.SetBool(App.DIALOGUE, true);     // activer l'animation 'dialogue' du joueur

					// PNJ
					other.FaceTo(on, App.playerManager.gameObject);								// orienter le PNJ vers le joueur 						
					other?.animatorController?.anim?.SetBool(App.DIALOGUE, true);				// activer l'animation 'dialogue' du pnj
					other.GetComponentInChildren<DialogueManager>().Run();						// démarrer le dialogue	
				} else {											// sortie du mode Dialogue et retour au mode Normal
					// player
					App.playerManager.FaceTo(false);                                            // cesser d'orienter le joueur vers le PNJ
					App.playerManager.animatorController?.anim.SetBool(App.DIALOGUE, false);    // désactiver l'animation 'dialogue' du joueur

					// PNJ
					other?.FaceTo(false);														// cesser d'orienter le PNJ vers le joueur
					other?.animatorController?.anim?.SetBool(App.DIALOGUE, false);              // désactiver l'animation 'dialogue' du pnj

					VD.isActive = false;
					SetSceneMode(SceneMode.normal, true, other);

					App.playerManager.StartCoroutine(PNJTimer(other));							// empêcher la reprise immédiate d'un dialogue
					IEnumerator PNJTimer(Character pnj) {										// par un clic trop rapide
						pnj.enabled = false;													//
						yield return new WaitForSeconds(1f);									//
						pnj.enabled = true;														//
					}
				}
				// personnages
				break;

			case SceneMode.fight:									// passage en mode Combat depuis le mode Normal
				if (on && sceneMode == SceneMode.normal) {
					App.cameraController.SetLateralCamera(other.transform);						// activer la caméra latérale
					App.itemsManager.SelectAll(false);											// masquer tous les actionsSprites pendant le combat

					// player
					App.playerManager.StopAgent(true);
					App.playerManager.fightController.SetOther(other);
					App.playerManager.animatorController?.anim?.SetBool(App.FIGHT, true);       // animation 'Attack'
					App.playerManager.FaceTo(on, other.gameObject);								// orienter le joueur vers le PNJ

					// PNJ
					other.navAgent.ResetPath();													// annulation de la navigation en cours
					other.navAgent.velocity = Vector3.zero;										// vitesse nulle
					other.fightController.SetOther(App.playerManager);
					other.animatorController?.anim?.SetBool(App.FIGHT, true);					// animation 'fight idle'
					other.FaceTo(on, App.playerManager.gameObject);								// orienter le PNJ vers le joueur 						
					other.Highlight(true);														// PNJ outlined

					App.statsUI.Show();															// afficher les statistiques
				} else {                                            // sortie du mode Combat et retour au mode Normal
					App.statsUI.Hide();                                                         // masquer les statistiques

					// PNJ
					other?.patrol?.RestoreInitialMode();										// si c'est un patrouilleur, restaurer son mode initial
					other?.animatorController?.anim?.SetBool(App.FIGHT, false);                 // animation 'idle'
					other?.FaceTo(false);														// cesser d'orienter le PNJ vers le joueur

					// player
					App.playerManager.animatorController?.anim?.SetBool(App.FIGHT, false);      // animation 'idle'
					App.playerManager.FaceTo(false);                                            // cesser d'orienter le joueur vers le PNJ
					App.playerManager.fightController.SetOther(null);

					SetSceneMode(SceneMode.normal, true, other);
				}

				break;
		}
	}

	private static SceneMode GetSceneMode() {
		if (App.playerManager.animatorController.anim.GetBool(App.DIALOGUE))
			return SceneMode.dialogue;

		if (App.playerManager.animatorController.anim.GetBool(App.FIGHT))
			return SceneMode.fight;

		return SceneMode.normal;
	}
}

public enum SceneMode {
	normal,
	dialogue,
	fight
}
