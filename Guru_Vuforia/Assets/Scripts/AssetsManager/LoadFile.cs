using System;
using UnityEngine;

namespace FileTools
{
    public class LoadFile
    {
        #region data

        public AssetRecord CorrelateRecord = null;

        private LoadBehaviour _Behaviour = LoadBehaviour.Null;
        public LoadBehaviour Behaviour
        {
            get { return _Behaviour; }
        }

        private FileAddressType _LoadPathType = FileAddressType.NULL;
        public FileAddressType LoadPathType
        {
            get { return _LoadPathType; }
        }

        private bool _IsCacheToRecord = false;
        public bool IsCacheToRecord
        {
            get { return _IsCacheToRecord; }
        }

        public Action<LoadFile> onFileLoaded = null;

        public Action<object> onloadingEnd = null;

        private bool _IsLoadSuccess = false;
        public bool IsLoadSuccess
        {
            set { _IsLoadSuccess = value; }
            get { return _IsLoadSuccess; }
        }

        #endregion

        // --------------------------------------------------------------------------------------------------------
        public LoadFile(AssetRecord correlateRecord,
                        LoadBehaviour behaviour,
                        bool isCacheToRecord = false,
                        Action<object> onloadingEnd = null,
                        Action<LoadFile> onFileLoaded = null)
        {
            CorrelateRecord = correlateRecord;
            _Behaviour = behaviour;
            _IsCacheToRecord = isCacheToRecord;
            this.onloadingEnd = onloadingEnd;
            this.onFileLoaded = onFileLoaded;

            switch (behaviour)
            {
                case LoadBehaviour.ContentLoadFromLoacal_LoadBundleFile:
                case LoadBehaviour.ContentLoadFromLoacal_WWW:
                    _LoadPathType = FileAddressType.LOCAL;
                    break;

                case LoadBehaviour.ContentLoadFromServer_WWW:
                    _LoadPathType = FileAddressType.SERVER;
                    break;
                case LoadBehaviour.DownloadFile_ResumeBrokenTransfer_HttpWebRequest:
                case LoadBehaviour.DownloadFile_WWW:
                    _LoadPathType = FileAddressType.SERVER;
                    break;
            }
        }

        // --------------------------------------------------------------------------------------------------------
        public void PrintInfo()
        {
            Debug.Log(" FileName : " + CorrelateRecord.ReleaseFileName +
                      " Behaviour : " + Behaviour.ToString() +
                      " IsCacheToRecord : " + IsCacheToRecord +
                      " IsFileLoadedSuccess : " + IsLoadSuccess +
                      " OnFileLoaded is Null : " + (onFileLoaded == null).ToString());
        }

        public void ExecuteOnFileLoaded()
        {
            if (onFileLoaded != null)
                onFileLoaded(this);
        }

        public string GetLoadFilePath()
        {
            return CorrelateRecord != null ? FileHelper.GetFilePath(CorrelateRecord, LoadPathType) : null;
        }

    }
}
