using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject fadeIn;

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(GameObject.Find("PlayButton"));
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (Input.GetAxis("Horizontal1") != 0 || Input.GetAxis("Vertical1") != 0)
                EventSystem.current.SetSelectedGameObject(GameObject.Find("PlayButton"));
        }
    }

    public void StartGame()
    {
        StartCoroutine(StartGameDelay());
    }

    private IEnumerator StartGameDelay()
    {
        fadeIn.GetComponent<Animator>().SetTrigger("FadeIn");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        StartCoroutine(QuitGameDelay());
    }

    private IEnumerator QuitGameDelay()
    {
        fadeIn.GetComponent<Animator>().SetTrigger("FadeIn");
        yield return new WaitForSeconds(1);
        Application.Quit();
    }
}
