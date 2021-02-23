using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorlogeSaver : Savable
{
	public Horloge horloge;
	public Light sunLight;


	/// <summary>
	/// Sérialiser les infos à sauvegarder 
	/// </summary>
	public override SSavable Serialize()
	{
		var result = new SHorloge().Copy(base.Serialize());
		//result.daySpeed = horloge.daySpeed;                 // vitesse d'horloge
		result.sunTime = horloge.GetSunTime();              // heure
		result.sunIntensity = sunLight.intensity;           // intensité de la lumière solaire
		result.sunColor = sunLight.color.ToArray();         // couleur de la lumière solaire
		return result;
	}

	/// <summary>
	/// Restaurer les valeurs précédement  sérialisées 
	/// </summary>
	/// <param name="serialized"></param>
	public override void Deserialize(object serialized)
	{
		if (serialized is SHorloge)
		{
			SHorloge s = serialized as SHorloge;
			//horloge.SetDaySpeed(s.daySpeed);
			horloge.SetDateTime(s.sunTime);
			sunLight.intensity = s.sunIntensity;
			sunLight.color = s.sunColor.ToColor();
		}
	}
}

/// <summary>
/// Classe pour la sauvegarde
/// </summary>
[System.Serializable]
public class SHorloge : SSavable
{
	//public float daySpeed;
	public float sunTime;
	public float sunIntensity;
	public float[] sunColor;
}