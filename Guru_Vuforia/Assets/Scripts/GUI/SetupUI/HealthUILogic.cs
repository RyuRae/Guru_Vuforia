using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUILogic  {

    private static HealthUILogic m_instance;
    public static HealthUILogic Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new HealthUILogic();
            }

            return HealthUILogic.m_instance;
        }
    }

    public void Initialize()
    {
        HealthUIView.Instance.FIRSTVBL += FirstVbl;
        HealthUIView.Instance.SECONDVBL += SecondVbl;
        HealthUIView.Instance.THIRDVBL += ThirdVbl;
        HealthUIView.Instance.FIRSTMIN += FirstMin;
        HealthUIView.Instance.SECONDMIN += SecondMin;
        HealthUIView.Instance.THIRDMIN += ThirdMin;
    }
    public void Release()
    {
        HealthUIView.Instance.FIRSTVBL -= FirstVbl;
        HealthUIView.Instance.SECONDVBL -= SecondVbl;
        HealthUIView.Instance.THIRDVBL -= ThirdVbl;
        HealthUIView.Instance.FIRSTMIN -= FirstMin;
        HealthUIView.Instance.SECONDMIN -= SecondMin;
        HealthUIView.Instance.THIRDMIN -= ThirdMin;
    }

    void FirstVbl()
    {

    }
    void SecondVbl()
    {

    }
    void ThirdVbl()
    {

    }
    void FirstMin()
    {

    }
    void SecondMin()
    {

    }
    void ThirdMin()
    {

    }
}
