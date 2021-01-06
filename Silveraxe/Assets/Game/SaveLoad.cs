using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Runtime.Serialization;

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
		}
		return null;
	}

	public static List<object> LoadSceneFile(string filename) {
		var fullPath = Application.persistentDataPath + "/" + filename + ".data";
		if (File.Exists(fullPath)) {
			try {
				BinaryFormatter bf = new BinaryFormatter();
				FileStream file = File.Open(fullPath, FileMode.Open);
				List<object> data = (List<object>)bf.Deserialize(file);
				file.Close();
				return data;
			}
			catch (SerializationException e) {
				Debug.Log(e.Message);
				return null;
			}
		}
		return null;
	}

	public static void DeleteSceneFile(string scene) {
		var fullPath = Application.persistentDataPath + "/" + scene + ".data";
		if (File.Exists(fullPath)) {
			File.Delete(fullPath);
		}
	}
}