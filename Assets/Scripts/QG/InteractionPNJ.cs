using UnityEngine;
using System.Collections;

public class InteractionPNJ : MonoBehaviour {
	void OnMouseDown () {
		QGManager.instance.ShowDetail (true);
		Debug.Log ("Clic PNJ");
	}
}
