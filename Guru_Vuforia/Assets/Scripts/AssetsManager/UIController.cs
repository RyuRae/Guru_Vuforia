using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace FileTools
{
    public class UIController : MonoSingleton<UIController>
    {
        public GameObject progressBar;
        public Slider progressBar_Slider;
        public Text progressBar_Descrip;
        public Text progressBar_Percent;
        public Image testImage;
        public Text deviceInfo_Text;
        public Text serverConnection_Text;
        private Coroutine hideProgressBar = null;

        private void Awake()
        {
            SetProgressBar(false);
        }

        private void OnEnable()
        {
            ManagerEvent.Register(ManagerEvent.MSG_DiviceInfo, SetDiviceInfo);
            ManagerEvent.Register(ManagerEvent.MSG_ProgressBar, SetProgressBar);
            ManagerEvent.Register(ManagerEvent.MSG_ServerConnection, SetServerConnection);
        }

        private void OnDisable()
        {
            ManagerEvent.Unregister(ManagerEvent.MSG_DiviceInfo, SetDiviceInfo);
            ManagerEvent.Unregister(ManagerEvent.MSG_ProgressBar, SetProgressBar);
            ManagerEvent.Unregister(ManagerEvent.MSG_ServerConnection, SetServerConnection);
        }

        private void SetDiviceInfo(params object[] args)
        {


        }

        private void SetServerConnection(params object[] args)
        {
            if (args == null) return;

            string errorString = "";
            if (args[0] is string)
            {
                string temp = (string)args[0];
                if (temp.StartsWith("re"))
                {
                    errorString = "连接服务器失败";
                }
                else if (temp.StartsWith("error is 404") || 
                         temp.StartsWith("error is 504") || 
                         temp.StartsWith("error is 502"))
                {
                    errorString = "未找到指定文件";
                }
                else
                {
                    errorString = "网络连接错误 (>_<)!";
                }

            }
            else if(args[0] is int)
            {
                switch ((int)args[0])
                {
                    case 504: errorString = "(504)未在服务器上找到指定文件 (⊙_⊙)？"; break;
                    default: errorString = "网络连接错误 (>_<)!"; break;
                }
            }
            serverConnection_Text.text = errorString;
        }

        private void SetProgressBar(params object[] args)
        {
            if (args == null) return;
            bool isShow = false;
            float slider = 0;
            string descrip = "";
            for (int i=0; i<args.Length; i++)
            {
                object obj = args[i];
                switch (i)
                {
                    case 0: isShow = (bool)obj; break;
                    case 1: slider = (float)obj; break;
                    case 2: descrip = (string)obj; break;
                }
            }

            progressBar.SetActive(isShow);
            progressBar_Slider.value = slider;
            progressBar_Descrip.text = descrip;
            progressBar_Percent.text = slider.ToString("0%");
            if (slider == 1f)
            {
                if (hideProgressBar != null)
                {
                    StopCoroutine(hideProgressBar);

                }
                hideProgressBar = StartCoroutine(delayClose(progressBar));
            }
        }

        private IEnumerator delayClose(GameObject go)
        {
            yield return new WaitForSeconds(0.33f);
            go.SetActive(false);
            hideProgressBar = null;
        }
    }
}
