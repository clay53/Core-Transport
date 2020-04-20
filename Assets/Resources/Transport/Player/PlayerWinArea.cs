using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerWinArea : MonoBehaviour
{
    public string sceneName;

    public void OnTriggerEnter(Collider col)
    {
        Debug.Log(col.gameObject.name + " - " + col.tag);
        if (col.tag == "Part" && col.transform.parent.tag == "Player")
        {
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(sceneName);
        }
    }
}
