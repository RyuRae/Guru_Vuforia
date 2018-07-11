using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AboutUILogic
{
    private static AboutUILogic m_instance;
    public static AboutUILogic Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new AboutUILogic();
            }

            return AboutUILogic.m_instance;
        }
    }

    public void Initialize()
    {

    }
    public void Release()
    {

    }

}
