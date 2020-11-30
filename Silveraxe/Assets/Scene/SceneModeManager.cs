using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static App;

public static class SceneModeManager
{
	public static SceneMode sceneMode => GetSceneMode();

	public const string Dialogue = "Dialogue";
	public const string Fight = "Fight";

	public static void SetSceneMode(SceneMode mode, bool on, Character other = null) {
		switch (mode) {
			case SceneMode.normal:
				cameraController.SetCamera(cameraController.vCamFollow);					// activer la caméra 'follow'
				interactableObjectsManager.SelectAll(true);									// resélectionner tous les objets intéractibles de la scène quand on commence un dialogue... 
				break;

			case SceneMode.dialogue:
				// scène
				if (on && sceneMode == SceneMode.normal) {
					playerManager.navAgent.SetDestination(playerManager.ActPosition(other), () => {
						cameraController.SetCamera(cameraController.vCamDialogue);          // activer la caméra 'dialogue'
						interactableObjectsManager.SelectAll(false);                        // désélectionner tous les objets intéractibles de la scène quand on commence un dialogue... 

						// player
						playerManager.animatorController?.anim.SetBool(Dialogue, true);		// activer l'animation 'dialogue' du joueur

						// PNJ
						other?.animatorController?.anim?.SetBool(Dialogue, true);			// activer l'animation 'dialogue' du pnj
						other.GetComponentInChildren<DialogueTrigger>().Run();              // démarrer le dialogue	
					});
				} else {
					// player
					playerManager.animatorController?.anim.SetBool(Dialogue, false);		// désactiver l'animation 'dialogue' du joueur

					// PNJ
					other?.animatorController?.anim?.SetBool(Dialogue, false);				// désactiver l'animation 'dialogue' du pnj

					SetSceneMode(SceneMode.normal, true, other);
				}
				// personnages
				break;

			case SceneMode.fight:
				if (on && sceneMode == SceneMode.normal) {
					playerManager.navAgent.SetDestination(playerManager.ActPosition(other), () => {
						interactableObjectsManager.SelectAll(false);                        // masquer tous les actionsSprites pendant le combat

						// player
						playerManager.fightController.SetOther(other);
						playerManager.animatorController?.anim?.SetBool(Fight, true);       // animation 'Attack'
						playerManager.FaceTo(on, other.gameObject);                         // orienter le joueur vers le PNJ

						// PNJ
						other.fightController.SetOther(playerManager);
						other.animatorController?.anim?.SetBool(Fight, true);				// animation 'fight idle'
						other.FaceTo(on, playerManager.gameObject);                         // orienter le PNJ vers le joueur 						
						other.Highlight(true);                                              // PNJ outlined

						statsUI.Show();														// afficher les statistiques
					});
				} else {
					statsUI.Hide();															// masquer les statistiques

					// PNJ
					other?.animatorController?.anim?.SetBool(Fight, false);					// animation 'idle'
					other?.FaceTo(false);                                                   // cesser d'orienter le PNJ vers le joueur

					// player
					playerManager.animatorController?.anim?.SetBool(Fight, false);			// animation 'idle'
					playerManager.FaceTo(false);                                            // cesser d'orienter le joueur vers le PNJ
					playerManager.fightController.SetOther(null);

					SetSceneMode(SceneMode.normal, true, other);
				}

				break;
		}
	}

	private static SceneMode GetSceneMode() {
		if (playerManager.animatorController.anim.GetBool(Dialogue))
			return SceneMode.dialogue;

		if (playerManager.animatorController.anim.GetBool(Fight))
			return SceneMode.fight;

		return SceneMode.normal;
	}
}
