using UnityEngine;
using System.Collections;

public class RecursiveBspTree : MonoBehaviour {

	// Use this for initialization
	void Start () {
        this.AddBSPTreeToChilds(this.transform);
	}

    void AddBSPTreeToChilds(Transform transform)
    {
        //Add BSPTree component if there is a mesh collider AND there is no BSPTree script allready attached
        if (transform.GetComponent<MeshCollider>() != null) {
            if (transform.GetComponent<BSPTree>() == null)
            {
                transform.gameObject.AddComponent<BSPTree>();
            }
        }

        //Loop all childs and call the same method
        foreach (Transform child in transform)
        {
            AddBSPTreeToChilds(child);
        }
    }

}
