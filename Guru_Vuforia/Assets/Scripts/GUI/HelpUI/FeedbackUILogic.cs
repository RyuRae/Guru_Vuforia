using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackUILogic
{
    private static FeedbackUILogic m_instance;
    public static FeedbackUILogic Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new FeedbackUILogic();
            }

            return FeedbackUILogic.m_instance;
        }
    }

    public void Initialize()
    {

    }
    public void Release()
    {

    }

}
