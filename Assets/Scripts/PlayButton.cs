using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public GameObject persistantObj;

    public string gameMode;

    public void OnClicked()
    {
        persistantObj.GetComponent<PersistanceScript>().gameMode = gameMode;
        SceneManager.LoadScene("GameScreen");
    }
}
