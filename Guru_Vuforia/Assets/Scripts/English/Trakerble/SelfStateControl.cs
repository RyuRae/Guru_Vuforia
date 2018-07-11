using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfStateControl : MonoBehaviour {

    // Use this for initialization
    bool state = false;
    Animation anim;
    Animation clickanim;
    //IListenerTracker nowListener;
    //SingleTrackableEventHandler single;
    private void Start()
    {
        //nowListener = transform.transform.parent.parent.parent.GetComponent<IListenerTracker>();
        //single = nowListener.OnGetCurrentTransform().GetComponent<SingleTrackableEventHandler>();
        anim = GetComponent<Animation>();
    }
    public void StateChange()
    {
        //CombineControl.Instance.PlaySound(nowListener);
        
        if (anim != null)
        {
            //single.PlaySound();
            anim.CrossFade("clickanim");
            StartCoroutine(WateAnimationLength(anim.GetClip("clickanim").length));

        }
        else
        {
            state = !state;
            if (state)
            {
                //single.PlaySound();
                gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x * 1.5f, gameObject.transform.localScale.y * 1.5f, gameObject.transform.localScale.z * 1.5f);
            }
            else
            {
                //single.Stop();
                gameObject.transform.localScale = new Vector3(gameObject.transform.localScale.x / 1.5f, gameObject.transform.localScale.y / 1.5f, gameObject.transform.localScale.z / 1.5f);
            }
        }
        
    }
    IEnumerator WateAnimationLength(float a)
    {
        yield return new WaitForSeconds(a);
        gameObject.gameObject.GetComponent<Animation>().CrossFade("idle");
    }
     
}
