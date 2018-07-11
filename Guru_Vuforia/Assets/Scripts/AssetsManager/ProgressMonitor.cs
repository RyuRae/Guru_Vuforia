using System;
using System.Text;
using UnityEngine;

namespace FileTools
{
    public enum ProgressState { NULL, DOWNLOAD, CHECK, LOAD }

    public class ProgressMonitor
    {
        // -----------------------------------------------------------------
        #region Describe
        const string STR_Downloading = "已下载 ";
        const string STR_Checking = "完成检查 ";
        const string STR_Load = "已加载 ";

        public ProgressState _State = ProgressState.NULL;
        public ProgressState State
        {
            get { return _State; }
            set
            {
                _State = value;
                SetDescribe();
            }
        }

        private string _TargetStr = "";
        public string TargetStr
        {
            get { return _TargetStr; }
            set
            {
                _TargetStr = value;
                SetDescribe();
                if (onChange != null) onChange(this);
            }
        }

        private void SetDescribe()
        {
            string actionStr = "";
            switch (_State)
            {
                case ProgressState.DOWNLOAD: actionStr = STR_Downloading; break;
                case ProgressState.CHECK: actionStr = STR_Checking; break;
                case ProgressState.LOAD: actionStr = STR_Load; break;
                default: actionStr = ""; break;
            }
            _Describe = new StringBuilder(actionStr).Append(_TargetStr).Append(" ...").ToString();
        }

        private string _Describe = "";

        public string Describe
        {
            get { return _Describe; }
        }

        #endregion
        // -----------------------------------------------------------------
        #region progress
        private float _TotalNum = 1f;
        public float TotalNum
        {
            get { return _TotalNum; }
            set
            {
                _TotalNum = value;
                SetProgress();
            }
        }

        private float _CurrentNum = 1f;
        public float CurrentNum
        {
            get { return _CurrentNum; }
            set
            {
                _CurrentNum = value;
                SetProgress();
                if (onChange != null) onChange(this);
            }
        }

        private void SetProgress()
        {
            if (_TotalNum != 0)
            {
                _Progress = _CurrentNum / _TotalNum;
            }
            else
            {
                _Progress = 0f;
            }
        }

        public float _Progress = 0f;
        public float Progress
        {
            get
            {
                return _Progress;
            }
        }
        #endregion
        // --------------------------------------------------

        public ProgressMonitor() { }

        public ProgressMonitor(int total, ProgressState state) : this()
        {
            _State = state;
            CaculateInit(total);
        }

        public Action<ProgressMonitor> onChange = null;

        public void CaculateInit(int total)
        {
            _TotalNum = total;
            _CurrentNum = 0f;
        }

        public void Refresh(LoadFile loadedFile)
        {
            Refresh(loadedFile.IsLoadSuccess, loadedFile.CorrelateRecord.ReleaseFileName, 1);
        }

        public void Refresh(bool isSuccess, string target, int increment)
        {
            if (target.Contains("@") && target.Contains("_card"))
                target = target.Substring(0, target.IndexOf('_'));
            TargetStr = target == null ? "" : target;
            CurrentNum += increment;
            string result = isSuccess ? "成功" : "失败";
            //UIController.Instance.SetProgressBar(true, Progress, Describe + result);
            UIManager.Instance.SetVisible(UIName.UISceneProgress, true);
            ManagerEvent.Send(ManagerEvent.MSG_ProgressBar, true, Progress, Describe + result);
        }
    }
}

