using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLoseArea : MonoBehaviour
{
    public void OnTriggerEnter(Collider col)
    {
        Debug.Log(col.gameObject.name + " - " + col.tag);
        if (col.tag == "Part" && col.transform.parent.tag == "Player")
        {
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
