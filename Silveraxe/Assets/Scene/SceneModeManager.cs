using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static App;

public static class SceneModeManager
{
	public static SceneMode sceneMode => GetSceneMode();

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
						cameraController.SetCamera(cameraController.vCamLateral);			// activer la caméra latérale
						interactableObjectsManager.SelectAll(false);                        // désélectionner tous les objets intéractibles de la scène quand on commence un dialogue... 

						// player
						playerManager.FaceTo(on, other.gameObject);                         // orienter le joueur vers le PNJ
						//playerManager.animatorController.SendAnims((Animator anim) => { anim.SetBool(Dialogue, true); });
						playerManager.animatorController?.anim.SetBool(DIALOGUE, true);		// activer l'animation 'dialogue' du joueur

						// PNJ
						other.FaceTo(on, playerManager.gameObject);                         // orienter le PNJ vers le joueur 						
						other?.animatorController?.anim?.SetBool(DIALOGUE, true);			// activer l'animation 'dialogue' du pnj
						other.GetComponentInChildren<DialogueTrigger>().Run();              // démarrer le dialogue	
					});
				} else {
					// player
					playerManager.FaceTo(false);                                            // cesser d'orienter le joueur vers le PNJ
					//playerManager.animatorController.SendAnims((Animator anim) => { anim.SetBool(Dialogue, false); });
					playerManager.animatorController?.anim.SetBool(DIALOGUE, false);		// désactiver l'animation 'dialogue' du joueur

					// PNJ
					other?.FaceTo(false);                                                   // cesser d'orienter le PNJ vers le joueur
					other?.animatorController?.anim?.SetBool(DIALOGUE, false);				// désactiver l'animation 'dialogue' du pnj

					SetSceneMode(SceneMode.normal, true, other);
				}
				// personnages
				break;

			case SceneMode.fight:
				if (on && sceneMode == SceneMode.normal) {
					playerManager.navAgent.SetDestination(playerManager.ActPosition(other), () => {
						cameraController.SetCamera(cameraController.vCamLateral);           // activer la caméra latérale
						interactableObjectsManager.SelectAll(false);                        // masquer tous les actionsSprites pendant le combat

						// player
						playerManager.StopAgent();
						playerManager.fightController.SetOther(other);
						playerManager.animatorController?.anim?.SetBool(FIGHT, true);       // animation 'Attack'
						playerManager.FaceTo(on, other.gameObject);                         // orienter le joueur vers le PNJ

						// PNJ
						other.navAgent.ResetPath();                    // annulation de la navigation en cours
						other.navAgent.velocity = Vector3.zero;        // vitesse nulle
						other.fightController.SetOther(playerManager);
						other.animatorController?.anim?.SetBool(FIGHT, true);				// animation 'fight idle'
						other.FaceTo(on, playerManager.gameObject);                         // orienter le PNJ vers le joueur 						
						other.Highlight(true);                                              // PNJ outlined

						statsUI.Show();														// afficher les statistiques
					});
				} else {
					statsUI.Hide();															// masquer les statistiques

					// PNJ
					other?.animatorController?.anim?.SetBool(FIGHT, false);					// animation 'idle'
					other?.FaceTo(false);                                                   // cesser d'orienter le PNJ vers le joueur

					// player
					//playerManager.animatorController.SendAnims((Animator anim) => { anim.SetBool(Fight, false); });
					playerManager.animatorController?.anim?.SetBool(FIGHT, false);			// animation 'idle'
					playerManager.FaceTo(false);                                            // cesser d'orienter le joueur vers le PNJ
					//playerManager.SendFightController((FightController fc) => { fc.SetOther(null); });
					playerManager.fightController.SetOther(null);

					SetSceneMode(SceneMode.normal, true, other);
				}

				break;
		}
	}

	private static SceneMode GetSceneMode() {
		if (playerManager.animatorController.anim.GetBool(DIALOGUE))
			return SceneMode.dialogue;

		if (playerManager.animatorController.anim.GetBool(FIGHT))
			return SceneMode.fight;

		return SceneMode.normal;
	}
}

public enum SceneMode
{
	normal,
	dialogue,
	fight
}
