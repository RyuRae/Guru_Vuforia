using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using System.Text;

namespace FileTools
{
    public class AssetRecord
    {
        // ---------------------------------------------------------------------------------
        # region data

        protected string _IndexName = "";
        public string IndexName
        {
            get { return _IndexName; }
        }

        protected byte _OrigExt = 0;
        public byte OrigExt
        {
            get { return _OrigExt; }
        }

        protected bool _IsBundle = false;
        public bool IsBundle
        {
            get { return _IsBundle; }
        }

        protected RuntimeAssetType _Type = RuntimeAssetType.UNKNOW;
        public RuntimeAssetType Type
        {
            get { return _Type; }
        }

        protected string _ReleaseFileName = null;
        public string ReleaseFileName
        {
            get { return _ReleaseFileName; }
        }

        protected long _Size = 0L;
        public long Size
        {
            get { return _Size; }
        }

        protected long _LastModifyTime = 0L;
        public long LastModifyTime
        {
            get { return _LastModifyTime; }
        }

        protected int _Version = 0;
        public int Version
        {
            get { return _Version; }
        }

        public object cached = null;

        #endregion
        // ---------------------------------------------------------------------------------
        #region constructor
        public AssetRecord() { }

        public AssetRecord(BinaryReader br) : this()
        {
            ReadByBinaryReader(br);
            _Type = FileHelper.GetRuntimeAssetType((FileExtension)OrigExt);
        }

        public AssetRecord(FileInfo fileInfo, string mainManifestName = null) : this()
        {
            if (!string.IsNullOrEmpty(mainManifestName) && 
                fileInfo.Name.Equals(mainManifestName))
            {
                _IndexName = mainManifestName;
                _OrigExt = (byte)FileExtension.EMPTY;
            }
            else
            {
                SetByIsBundle(fileInfo);
            }
            _Type = FileHelper.GetRuntimeAssetType((FileExtension)OrigExt);
            _ReleaseFileName = fileInfo.Name;
            _Size = fileInfo.Length;
            _LastModifyTime = FileHelper.ConvertDateTimeToLong(fileInfo.LastWriteTimeUtc);
        }

        #endregion
        // ---------------------------------------------------------------------------------
        #region binary read and write
        public virtual void ReadByBinaryReader(BinaryReader br)
        {
            _IndexName = FileHelper.ReadBinaryString(br);
            _OrigExt = br.ReadByte();
            _IsBundle = br.ReadBoolean();
            _ReleaseFileName = FileHelper.ReadBinaryString(br);
            _Size = br.ReadInt64();
            _LastModifyTime = br.ReadInt64();
            _Version = br.ReadInt32();
        }

        public virtual void WriteByBinaryWriter(BinaryWriter bw)
        {
            FileHelper.WriteBinaryString(bw, IndexName);
            bw.Write(OrigExt);
            bw.Write(IsBundle);
            FileHelper.WriteBinaryString(bw, ReleaseFileName);
            bw.Write(Size);
            _LastModifyTime = _LastModifyTime == 0L ? FileHelper.ConvertDateTimeToLong(DateTime.UtcNow) : _LastModifyTime;
            bw.Write(LastModifyTime);
            _Version = _Version == 0 ? FileHelper.ConvertDateTimeToInt(DateTime.UtcNow) : _Version;
            bw.Write(Version);
        }

        #endregion
        // ---------------------------------------------------------------------------------
        #region bundleName的明明规则和解析规则，如果要改，两个函数应该一起改
        private void SetByIsBundle(FileInfo fileInfo)
        {
            _IsBundle = fileInfo.Extension.ToLower().EndsWith(StrConst.DOT_AB);
            if (_IsBundle)
            {
                string[] strArray = fileInfo.Name.Split('@');
                _IndexName = strArray[0];
                _OrigExt = FileHelper.ExtensionToByte(strArray[1].ToLower().Replace(StrConst.DOT_AB, ""));
            }
            else
            {
                //_IndexName = fileInfo.Name.Replace(fileInfo.Extension, "");
                _IndexName = fileInfo.Name; //应ARTOOLKIT需求将扩展名也作为索引名的一部份
                _OrigExt = FileHelper.ExtensionToByte(fileInfo.Extension);
            }
        }

        public static string GetBundleName(FileInfo fi)
        {
            return fi.Name.Replace(".", "@").ToLower() + StrConst.DOT_AB;
        }

        #endregion
        // ---------------------------------------------------------------------------------

        public virtual void PrintInfo()
        {
            RuntimeAssetType rat = FileHelper.GetRuntimeAssetType((FileExtension)OrigExt);
            Debug.Log("IndexName : " + IndexName + " , " +
                      "OrigExt : " + OrigExt + " , " +
                      "IsBundle : " + IsBundle + " , " +
                      "RuntimeAssetType : " + rat + " , " +
                      "ReleaseFileName : " + ReleaseFileName + " , " +
                      "Size : " + FileHelper.GetFileSize(Size) + " , " +
                      "LastModifyTime : " + LastModifyTime + " , " +
                      "Version : " + Version);
        }

        public virtual LoadFile GetLoadFile(LoadBehaviour behaviour,
                                            bool isCacheToRecord,
                                            Action<object> onloadingEnd = null,
                                            Action<LoadFile> onFileLoaded = null)
        {
            bool isCache = isCacheToRecord;
            if (behaviour == LoadBehaviour.Null ||
                behaviour == LoadBehaviour.DownloadFile_ResumeBrokenTransfer_HttpWebRequest)
            {
                isCache = false;
            }
            return new LoadFile(this, behaviour, isCache, onloadingEnd, onFileLoaded);
        }

        // ---------------------------------------------------------------------------------
    }
}
