using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QGManager : MonoBehaviour {

	public List<GameObject> upgradesAvailable;
	public List<GameObject> upgradesUnvailable;

	public RectTransform panelQG;

	private static QGManager _instance;
	public static QGManager instance
	{
		get
		{
			if (_instance == null)
				_instance = GameObject.FindObjectOfType<QGManager>();
			return _instance;
		}
	}

	void BuildUpgrade ()
	{

	}

	public void ShowDetail (bool value)
	{
		panelQG.gameObject.SetActive (value);
		if (value)
		{
			foreach (GameObject upgrade in upgradesAvailable) {
				GameObject tmpRCT = Instantiate(Resources.Load("QG/Prefabs/Button-Upgrade")) as GameObject;
				tmpRCT.transform.parent = panelQG.transform;
				tmpRCT.transform.localScale = new Vector3(1,1,1);
				tmpRCT.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,0);
				
				UpgradeQG tmpUp = upgrade.GetComponent<UpgradeQG>();
			}
		}
	}

}
