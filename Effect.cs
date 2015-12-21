using UnityEngine;
using System.Collections;

public class Effect : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Invoke("AutoDestroy", 3);
	}

    void AutoDestroy()
    {
        Destroy(this.gameObject);
        //gameObject.SetActive(false);
    }

    void OnDisalbe()
    {
        CancelInvoke();
    }
}
