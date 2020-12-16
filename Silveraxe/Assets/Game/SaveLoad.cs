using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
public static class SaveLoad
{
	//public static List<Game> savedGames = new List<Game>();

	public static void SaveData(string fileName, List<object> data) {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/" + fileName + ".data");
		bf.Serialize(file, data);
		file.Close();
	}

	public static void LoadPlayerData(string filename = "player") {
		var fullPath = Application.persistentDataPath + "/" + filename + ".data";
		if (File.Exists(fullPath)) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(fullPath, FileMode.Open);
			List<object> data = (List<object>)bf.Deserialize(file);
			file.Close();
			Game.current.LoadPlayer(data);
		} 
	}

	public static void LoadSceneData(string filename) {
		var fullPath = Application.persistentDataPath + "/" + filename + ".data";
		if (File.Exists(fullPath)) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(fullPath, FileMode.Open);
			List<object> data = (List<object>)bf.Deserialize(file);
			file.Close();
			Game.current.LoadScene(data);
		}
	}
}
