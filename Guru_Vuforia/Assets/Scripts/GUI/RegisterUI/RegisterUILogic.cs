using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#region 模块信息
/*----------------------------------------------------------------
 * 模块名：RegisterUILogic
 * 创建者：郑双喜
 * 修改者列表：
 * 创建日期：2017.12.18
 * 模块描述：注册UI界面逻辑
 *----------------------------------------------------------------*/
#endregion
public class RegisterUILogic
{
    private static RegisterUILogic m_instance;
    public static RegisterUILogic Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new RegisterUILogic();
            }

            return RegisterUILogic.m_instance;
        }
    }

    public void Initialize()
    {
        RegisterUIView.Instance.SUBMIT += InfoSubmit;
    }
    public void Release()
    {
        RegisterUIView.Instance.SUBMIT -= InfoSubmit;
    }
    void InfoSubmit()
    {

    }
   

}
