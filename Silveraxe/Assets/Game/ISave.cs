using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISave 
{
	SSavable Serialize();
	void Deserialize(object serialized);
}
