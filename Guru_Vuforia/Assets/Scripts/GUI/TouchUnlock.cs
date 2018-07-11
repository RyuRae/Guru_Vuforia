using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#region 模块信息
/*----------------------------------------------------------------
 * 模块名：TouchUnlock
 * 创建者：郑双喜
 * 修改者列表：
 * 创建日期：2017.12.26
 * 模块描述：双指滑动解锁家长锁
 *----------------------------------------------------------------*/
#endregion
public class TouchUnlock : MonoBehaviour
{

    Transform close;
    Transform panel;
    NativeCall native;
    void Awake()
    {
        close = transform.Find("UnlockBG/Close");
        panel= transform.parent.Find("Panel");
        Button button = close.GetComponent<Button>();
        if (button != null)
        {
            //button.onClick.RemoveListener(OnClick);
            button.onClick.AddListener(OnClick);
        }
        native = GameObject.Find("NativeCall").GetComponent<NativeCall>();
    }

    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.touchCount ==2 && Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved)
            {
                Vector2 touchDeltaPositiona= Input.GetTouch(0).deltaPosition;
                Vector2 touchDeltaPositionb = Input.GetTouch(1).deltaPosition;
                //Debug.LogError("flyvr方向" + touchDeltaPositiona.x);
                if (touchDeltaPositiona.x>50&& touchDeltaPositionb.x > 50)
                {
                   if(gameObject.gameObject.activeSelf)
                    {
                        native.StartSetting();
                        //panel.gameObject.SetActive(true);
                        this.gameObject.SetActive(false);
                    }                       
                }         
            }
        }

        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    gameObject.SetActive(false);
        //}
    }

    void OnClick()
    {
        //Debug.LogError("flyvr111");
        //native.StartSetting();
        this.gameObject.SetActive(false);
    }
}
