using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISave 
{
	//void Serialize(List<object> sav);
	SInteractable Serialize();
	void Deserialize(object serialized);
}
