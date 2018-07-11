using UnityEngine;
using System;
using System.IO;
using System.Text;

namespace FileTools
{
    public class FileHelper : Singleton<FileHelper>
    {
        public static string GetMainManifestName(object obj = null)
        {
            string mainManifestName = ("bundleindex_" + GetPlatformName(obj)).ToLower();
            return mainManifestName;
        }

        public static string GetPlatformName(object obj = null)
        {
#if UNITY_EDITOR
            UnityEditor.BuildTarget target = obj != null ?
                                             (UnityEditor.BuildTarget)obj :
                                             UnityEditor.EditorUserBuildSettings.activeBuildTarget;
            switch (target)
            {
                case UnityEditor.BuildTarget.Android:
                    return StrConst.ANDROID;

                case UnityEditor.BuildTarget.iOS:
                    return StrConst.IOS;

                case UnityEditor.BuildTarget.WebGL:
                    return StrConst.WEBGL;

                case UnityEditor.BuildTarget.StandaloneWindows:
                case UnityEditor.BuildTarget.StandaloneWindows64:
                    return StrConst.WINDOWS;

                case UnityEditor.BuildTarget.StandaloneOSXIntel:
                case UnityEditor.BuildTarget.StandaloneOSXIntel64:
                    return StrConst.OSX;

                default:
                    return null;
            }
#else
            RuntimePlatform platform = obj != null ? 
                                       (RuntimePlatform)obj :
                                       Application.platform;

		    switch (platform)
            {
                case RuntimePlatform.Android:
                    return StrConst.ANDROID;

                case RuntimePlatform.IPhonePlayer:
                    return StrConst.IOS;

                case RuntimePlatform.WebGLPlayer:
                    return StrConst.WEBGL;

                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    return StrConst.WINDOWS;

                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.OSXPlayer:
                //case RuntimePlatform.OSXWebPlayer:
                //case RuntimePlatform.OSXDashboardPlayer:
                    return StrConst.OSX;

                default:
                    return null;
            }
#endif
        }

        //-------------------------------------------------------------------------------------------
        #region 时间字符互转相关

        public static int ConvertDateTimeToInt(DateTime dataTime)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            return ((int)(dataTime - startTime).TotalSeconds);
        }

        public static long ConvertDateTimeToLong(DateTime dataTime)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)); // 当地时区
            return ((long)(dataTime - startTime).TotalMilliseconds);
        }

        //public static string ConvertDateTimeToStr(DateTime dt)
        //{
        //    return new StringBuilder().AppendFormat("{0:yyyy-MM-dd HH:mm:ss:ffff}", dt).ToString();
        //}

        //public static DateTime ConvertLongToDateTime(long l)
        //{
        //    DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
        //    long iTime = long.Parse(l + "0000");
        //    TimeSpan toNow = new TimeSpan(iTime);
        //    return startTime.Add(toNow);
        //}

        //public static string GetTimeStampStr()
        //{
        //    return ConvertDateTimeToStr(DateTime.UtcNow.ToLocalTime());
        //}

        #endregion
        //-------------------------------------------------------------------------------------------
        #region 文件大小相关
        public static string GetFileSize(double len)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            while (len >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                len = len / 1024;
            }

            return new StringBuilder().AppendFormat("{0:0.##} {1}", len, sizes[order]).ToString();
        }

        public static string GetFileSize(long len)
        {
            return GetFileSize((double)len);
        }
        #endregion
        //-------------------------------------------------------------------------------------------
        #region 文件扩展名与运行时类型相关
        public static string GetExtensionStr(byte extensionByte)
        {
            return GetExtensionStr((FileExtension)extensionByte);
        }

        public static string GetExtensionStr(FileExtension ExtensionEnum)
        {
            if (ExtensionEnum == FileExtension.EMPTY)
            {
                return "";
            }
            else
            {
                return new StringBuilder().AppendFormat("{0}{1}", ".", ExtensionEnum.ToString().ToLower()).ToString();
            }
        }

        public static byte ExtensionToByte(string extension)
        {
            byte i = 0;
            string str = extension.Replace(".", "").ToUpper();

            if (!string.IsNullOrEmpty(str))
            {
                if (Enum.IsDefined(typeof(FileExtension), str))
                {
                    i = (byte)((FileExtension)Enum.Parse(typeof(FileExtension), str));
                }
            }
            else
            {
                i = (byte)FileExtension.EMPTY;
            }
            return i;
        }

        public static RuntimeAssetType GetRuntimeAssetType(FileExtension extension)
        {
            RuntimeAssetType result = RuntimeAssetType.UNKNOW;
            switch (extension)
            {
                case FileExtension.EMPTY:
                    result = RuntimeAssetType.MANIFEST;
                    break;

                case FileExtension.PREFAB:
                    result = RuntimeAssetType.BUNDLE_PREFAB;
                    break;

                case FileExtension.UNITY:
                    result = RuntimeAssetType.BUNDLE_SCENE;
                    break;

                case FileExtension.CONF:
                case FileExtension.TXT:
                case FileExtension.CSV:
                    result = RuntimeAssetType.TEXT;
                    break;

                case FileExtension.JPEG:
                case FileExtension.PNG:
                    result = RuntimeAssetType.TEXTRUE;
                    break;

                case FileExtension.MP3:
                    result = RuntimeAssetType.AUDIO;
                    break;

                case FileExtension.FSET:
                case FileExtension.FSET3:
                case FileExtension.ISET:
                    result = RuntimeAssetType.ARMARK;
                    break;
            }
            return result;
        }

        public static string GetFilePath(AssetRecord record, FileAddressType addressType)
        {
            if (record == null || addressType == FileAddressType.NULL)
            {
                return null;
            }
            else
            {
                string address = "";
                switch (addressType)
                {
                    case FileAddressType.LOCAL: address = StrConst.Instance.LocalFileAddress; break;
                    case FileAddressType.SERVER: address = StrConst.Instance.ServerFileAddress; break;
                    case FileAddressType.DOWNLOAD_TEMP: address = StrConst.Instance.DownloadTempAddress; break;
                }
                string folder = GetFileFolderPath(record.Type);

                //断电续传下载到一半终止  服务器端已经发生变化之后再更新时 用于区别旧的临时文件
                string fileName = addressType == FileAddressType.DOWNLOAD_TEMP ? 
                                  record.ReleaseFileName + record.Version.ToString() : 
                                  record.ReleaseFileName ;

                return new StringBuilder().AppendFormat("{0}{1}{2}", address, folder, fileName).ToString();
            }
        }

        public static string GetFileFolderPath(RuntimeAssetType type)
        {
            string folder = "";
            switch (type)
            {
                case RuntimeAssetType.AUDIO:
                    folder = StrConst.PATH_AUDIO_FOLDER;
                    break;

                case RuntimeAssetType.BUNDLE_PREFAB:
                case RuntimeAssetType.BUNDLE_SCENE:
                case RuntimeAssetType.MANIFEST:
                    folder = StrConst.PATH_BUNDLE_FOLDER;
                    break;

                case RuntimeAssetType.TEXT:
                    folder = StrConst.PATH_CONFIG_FOLDER;
                    break;

                case RuntimeAssetType.TEXTRUE:
                    folder = StrConst.PATH_TEXTRUE_FOLDER;
                    break;

                case RuntimeAssetType.ARMARK:
                    folder = StrConst.PATH_ARMARK_FOLDER;
                    break;
            }
            return folder;
        }

        public static void ActionOnTraversePackingRecordType(Action<RuntimeAssetType> onGotType)
        {
            Array types = Enum.GetValues(typeof(RuntimeAssetType));
            var it_types = types.GetEnumerator();
            while (it_types.MoveNext())
            {
                RuntimeAssetType type = (RuntimeAssetType)it_types.Current;
                if (type != RuntimeAssetType.UNKNOW)
                {
                    onGotType(type);
                }
            }
        }
        
        #endregion
        //-------------------------------------------------------------------------------------------
        #region 二进制读写相关
        public static void ReadFromBinaryArray(byte[] bytes, Action<BinaryReader> onRecordRead)
        {
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    onRecordRead(br);
                }
            }
        }

        public static void ReadFromBinaryFile(string binaryFilePath, Action<BinaryReader> onRecordRead)
        {
            using (FileStream fs = new FileStream(binaryFilePath, FileMode.OpenOrCreate))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    onRecordRead(br);
                }
            }
        }

        public static void WriteToBinaryFile(string binaryFilePath, Action<BinaryWriter> onRecordWrite)
        {
            if (File.Exists(binaryFilePath))
                File.Delete(binaryFilePath);

            using (FileStream fs = new FileStream(binaryFilePath, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    onRecordWrite(bw);
                }
            }
        }

        public static void WriteBinaryString(BinaryWriter bw, string str)
        {
            bw.Write(Encoding.UTF8.GetByteCount(str));
            bw.Write(Encoding.UTF8.GetBytes(str));
        }

        public static string ReadBinaryString(BinaryReader br)
        {
            int length = br.ReadInt32();
            return Encoding.UTF8.GetString(br.ReadBytes(length));
        }
        #endregion
        //-------------------------------------------------------------------------------------------
        #region 文件保存相关
        public static void SaveToFile(LoadFile loadFile, Action<BinaryWriter> onWrite)
        {
            string saveFilePath = SetSaveFilePath(loadFile);
            if (onWrite != null && !string.IsNullOrEmpty(saveFilePath))
            {
                WriteToBinaryFile(saveFilePath, onWrite);
            }
        }

        public static void SaveToFile(LoadFile loadFile, byte[] bytes)
        {
            string saveFilePath = SetSaveFilePath(loadFile);
            if (bytes != null && bytes.Length > 0 && !string.IsNullOrEmpty(saveFilePath))
            {
                FileInfo fileInfo = new FileInfo(saveFilePath);
                if (fileInfo.Exists)
                {
                    fileInfo.Delete();
                }

                using (Stream fs = fileInfo.Create())
                {
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Close();
                }
            }
        }

        private static string SetSaveFilePath(LoadFile loadFile)
        {
            if (loadFile == null) return null;
            FileAddressType savePathType = loadFile.CorrelateRecord is AssetRecordsInfo ?
                                           FileAddressType.LOCAL :
                                           FileAddressType.DOWNLOAD_TEMP;
            string saveFilePath = null;
            if (savePathType == FileAddressType.LOCAL || savePathType == FileAddressType.DOWNLOAD_TEMP)
            {
                saveFilePath = GetFilePath(loadFile.CorrelateRecord, savePathType);
                DirectoryInfo folderPathInfo = new DirectoryInfo(Path.GetDirectoryName(saveFilePath));
                if (!folderPathInfo.Exists)
                {
                    folderPathInfo.Create();
                }
            }
            return saveFilePath;
        }

        #endregion
        //-------------------------------------------------------------------------------------------
    }
}

