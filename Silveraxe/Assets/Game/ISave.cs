using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISave 
{
	void Serialize(List<object> sav);
	void Deserialize(object serialized);
}
