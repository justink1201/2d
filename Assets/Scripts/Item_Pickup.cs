using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Pickup : MonoBehaviour
{
    Animator _myAnim;
    AudioSource _myAudio;
    public bool _collected;

    public void Pickup()
    {
        _myAnim = GetComponent<Animator>();
        _myAudio = GetComponent<AudioSource>();
        _myAnim.SetBool("_pickup", true);
        _myAudio.Play();
        _collected = true;
    }

    IEnumerator WaitToDestroy()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }
}
