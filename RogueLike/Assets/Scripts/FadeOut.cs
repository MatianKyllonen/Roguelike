using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    private void Start()
    {
        Bust();
    }

    public void Bust()
    {
        GetComponent<Animator>().SetTrigger("FadeOut");
    }
}
