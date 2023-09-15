using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TurnBackToMenu : MonoBehaviour
{

    public Sprite lobbyyfull;
    

    public void TurnBack(int delay, int sceneIndex, NetworkRunner runner = null, int imageInex = 0)
    {
        StartCoroutine(LoadScene(delay, sceneIndex, runner, imageInex));
    }

    IEnumerator LoadScene(int delay, int index, NetworkRunner runner = null, int imageIndex = 0)
    {
        if(imageIndex == 1) GetComponent<Image>().sprite = lobbyyfull;

        yield return new WaitForSecondsRealtime(delay);
        SceneManager.LoadScene(index);
        if (runner != null)
        {
            runner.Shutdown();
        }

    }
}
