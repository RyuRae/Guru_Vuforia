using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;

namespace FileTools
{
    public class StrConst : Singleton<StrConst>
    {
        //-------------------------------------------------------------------------------------------
        #region 路径
        public const string PATH_BUNDLE_FOLDER = "assetbundles/";
        public const string PATH_CONFIG_FOLDER = "configfiles/";
        public const string PATH_AUDIO_FOLDER = "audiofiles/";
        public const string PATH_TEXTRUE_FOLDER = "textruefiles/";
        public const string PATH_ARMARK_FOLDER = "armarkfiles/";

        private string _LocalFileAddress;
        public string LocalFileAddress
        {
            get
            {
                if (_LocalFileAddress == null)
                {
                    _LocalFileAddress =
                    #if UNITY_EDITOR || UNITY_STANDALONE
                        Application.persistentDataPath + "/release/";
                    #elif UNITY_ANDROID
                        Application.persistentDataPath + "/release/";
                    #else
                        Application.persistentDataPath + "/release/";
                    #endif
                }

                return _LocalFileAddress;
            }
        }

        private string _ServerFileAddress;
        public string ServerFileAddress
        {
            get
            {
                if (_ServerFileAddress == null)
                {
                    //_ServerFileAddress = "http://service.airguru.cn/file/downloadBundles/"; //http://service.airguru.cn:8080/file/downloadBundles/ ---http://10.0.1.52:8080/Air_ARToolkit/release/
                    _ServerFileAddress = "http://10.0.1.52:8080/Air_ARToolkit/release/";//"http://10.0.1.52:8080/Air_ARToolkit/release/";http://service.airguru.cn/file/downloadAssets/
                }

                return _ServerFileAddress;
            }
        }

        private string _DownloadTempAddress;
        public string DownloadTempAddress
        {
            get
            {
                if (_DownloadTempAddress == null)
                {
                    _DownloadTempAddress =
                    #if UNITY_EDITOR || UNITY_STANDALONE
                        Application.persistentDataPath + "/temprelease/";
                    #elif UNITY_ANDROID
                        Application.persistentDataPath + "/temprelease/";
                    #else
                        Application.persistentDataPath + "/temprelease/";
                    #endif
                }

                return _DownloadTempAddress;
            }
        }

        #endregion
        //-------------------------------------------------------------------------------------------
        #region 字符串
        public const string ANDROID = "Android";
        public const string IOS = "IOS";
        public const string WEBGL = "WebGL";
        public const string WINDOWS = "Windows";
        public const string OSX = "OSX";

        public const string DOT_PREFAB = ".prefab";
        public const string DOT_UNITY = ".unity";
        public const string DOT_AB = ".ab";
        public const string DOT_CONF = ".conf";
        public const string DOT_TXT = ".txt";
        public const string DOT_CSV = ".csv";
        public const string DOT_JPEG = ".jpeg";
        public const string DOT_PNG = ".png";
        public const string DOT_MP3 = ".mp3";
        public const string DOT_WAV = ".wav";

        public const string DOT_FSET = ".fset";
        public const string DOT_FSET3 = ".fset3";
        public const string DOT_ISET = ".iset";

        public static string FILE_PREFIX =
        #if UNITY_EDITOR || UNITY_STANDALONE
        "file:///";
        #elif UNITY_ANDROID
        "file:///";
        #else
        "file:///";
        #endif

        #endregion
        //-------------------------------------------------------------------------------------------
        #region 配置文件名

        public const string CONF_ASSET_RECORD_INFO = "assetrecordsinfo";
        public const string CONF_WORD_LIB = "word_lib";

        #endregion
        //-------------------------------------------------------------------------------------------
    }

    //-----------------------------------------------------------------------------------------------

    public enum LoadMethod { WWW, BUNDLE_FILE, HTTP_WEB_REQUEST }

    public enum FileExtension
    {
        NULL = 0,
        EMPTY = 1,
        PREFAB = 2,
        UNITY = 3,
        AB = 4,
        CONF = 5,
        TXT = 6,
        CSV = 7,
        JPEG = 8,
        PNG = 9,
        MP3 = 10,
        FSET = 11,
        FSET3 = 12,
        ISET = 13
    }

    public enum RuntimeAssetType
    {
        UNKNOW = 0,
        MANIFEST = 1,
        BUNDLE_SCENE = 2,
        BUNDLE_PREFAB = 3,
        TEXT = 4,
        TEXTRUE = 5,
        AUDIO = 6,
        ARMARK = 7
    }

    public enum FileAddressType { NULL, SERVER, LOCAL, DOWNLOAD_TEMP }

    public enum LoadBehaviour
    {
        Null = 0,
        DownloadFile_WWW = 2,
        ContentLoadFromServer_WWW = 4,
        ContentLoadFromLoacal_WWW = 8,
        ContentLoadFromLoacal_LoadBundleFile = 16,
        DownloadFile_ResumeBrokenTransfer_HttpWebRequest = 32,
    }

}
