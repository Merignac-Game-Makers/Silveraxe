using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Share2Skeleton : MonoBehaviour
{
	public GameObject[] targets; 


    void Start() {
        SkinnedMeshRenderer sourceRenderer = GetComponent<SkinnedMeshRenderer>();
        Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();
        foreach (Transform bone in sourceRenderer.bones) {
            boneMap[bone.name] = bone;
        }

        foreach (GameObject target in targets) {
            SkinnedMeshRenderer renderer = target.GetComponent<SkinnedMeshRenderer>();
            Transform[] boneArray = renderer.bones;

           for (int idx = 0; idx < boneArray.Length; ++idx) {
                string boneName = boneArray[idx].name;
                if (false == boneMap.TryGetValue(boneName, out boneArray[idx])) {
                    Debug.LogError("failed to get bone: " + boneName);
                    Debug.Break();
                }
            }
            renderer.bones = boneArray; //take effect
		}

     }
}
