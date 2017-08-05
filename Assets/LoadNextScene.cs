using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URSA;
using URSA.SceneManagement;

public class LoadNextScene : MonoBehaviour {

    public SceneData scenedata;

	public void LoadScene() {
        ECSController.LoadLevel(scenedata);
    }
}
