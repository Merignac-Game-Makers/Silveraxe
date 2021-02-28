using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Explanation", menuName = "Custom/Explanation", order = -999)]
public class Explanation : ScriptableObject {
	public string title;
	[TextArea(1, 20)]
	public string explanation;
	public string bottom;
	public Sprite picture;
}
