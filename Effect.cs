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
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
