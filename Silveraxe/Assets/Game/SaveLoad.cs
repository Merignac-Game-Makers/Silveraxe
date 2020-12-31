using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
public static class SaveLoad
{

	public static void SaveHeaderFile(object data) {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/header.data");
		bf.Serialize(file, data);
		file.Close();
	}

	public static void SavePlayerFile(SPlayer data) {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/player.data");
		bf.Serialize(file, data);
		file.Close();
	}

	public static void SaveSceneFile(string fileName, List<object> data) {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/" + fileName + ".data");
		bf.Serialize(file, data);
		file.Close();
	}


	public static object LoadHeaderFile() {
		var fullPath = Application.persistentDataPath + "/header.data";
		if (File.Exists(fullPath)) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(fullPath, FileMode.Open);
			object data = bf.Deserialize(file);
			file.Close();
			return data;
			//Game.current.LoadHeader(data);
		}
		return null;
	}

	public static List<object> LoadSceneFile(string filename) {
		var fullPath = Application.persistentDataPath + "/" + filename + ".data";
		if (File.Exists(fullPath)) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(fullPath, FileMode.Open);
			List<object> data = (List<object>)bf.Deserialize(file);
			file.Close();
			return data;
			//Game.current.LoadScene(data);
		}
		return null;
		//SaveLoad.LoadPlayerFile();                                              // charger les données du joueur
	}	
	
	public static object LoadPlayerFile() {
		var fullPath = Application.persistentDataPath + "/player.data";
		if (File.Exists(fullPath)) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(fullPath, FileMode.Open);
			object data = bf.Deserialize(file);
			file.Close();
			return data;
			//Game.current.LoadPlayer(data);
		}
		return null;
	}


}
