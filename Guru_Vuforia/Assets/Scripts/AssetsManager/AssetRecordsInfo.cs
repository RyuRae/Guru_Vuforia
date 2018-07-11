using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using UnityEngine;

namespace FileTools
{
    public class AssetRecordsInfo : AssetRecord
    {
        // ---------------------------------------------------------------------------------
        #region data
        public string mainManifestFileName = "";
        Dictionary<RuntimeAssetType, int> countDic;
        Dictionary<RuntimeAssetType, Dictionary<string, AssetRecord>> typeToRecordsDic;

        #endregion
        // ---------------------------------------------------------------------------------
        #region constructor

        private void InitDic()
        {
            countDic = new Dictionary<RuntimeAssetType, int>();
            typeToRecordsDic = new Dictionary<RuntimeAssetType, Dictionary<string, AssetRecord>>();
            FileHelper.ActionOnTraversePackingRecordType((type) =>
            {
                countDic.Add(type, 0);
                typeToRecordsDic.Add(type, new Dictionary<string, AssetRecord>());
            });
        }

        public AssetRecordsInfo() : base()
        {
            InitDic();
            _IndexName = StrConst.CONF_ASSET_RECORD_INFO;
            _OrigExt = (byte)FileExtension.CONF;
            _ReleaseFileName = new StringBuilder().AppendFormat("{0}{1}", 
                                                                _IndexName,
                                                                FileHelper.GetExtensionStr(_OrigExt)).ToString();
        }

        public AssetRecordsInfo(FileInfo fileInfo) : base(fileInfo, "")
        {
            InitDic();
        }

        public AssetRecordsInfo(byte[] bytes) : this()
        {
            FileHelper.ReadFromBinaryArray(bytes, ReadByBinaryReader);
        }

        #endregion
        // ---------------------------------------------------------------------------------
        #region binary read and write

        public void ReadByBinaryReader_Summary(BinaryReader br)
        {
            try
            {
                base.ReadByBinaryReader(br);
                mainManifestFileName = FileHelper.ReadBinaryString(br);
            }
            catch (Exception)
            {
                // done !
            }
        }

        public override void ReadByBinaryReader(BinaryReader br)
        {
            try
            {
                ReadByBinaryReader_Summary(br);
                while (true)
                {
                    string byteStr = FileHelper.ReadBinaryString(br);
                    RuntimeAssetType recordType = (RuntimeAssetType)(Enum.Parse(typeof(RuntimeAssetType), byteStr.ToUpper()));
                    countDic[recordType] = br.ReadInt32();
                    for (int i = 0; i < countDic[recordType]; i++)
                    {
                        AssetRecord ar = new AssetRecord(br);
                        if (!typeToRecordsDic[recordType].ContainsKey(ar.IndexName))
                        {
                            typeToRecordsDic[recordType].Add(ar.IndexName, ar);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // done !
            }
        }

        public override void WriteByBinaryWriter(BinaryWriter bw)
        {
            base.WriteByBinaryWriter(bw);
            FileHelper.WriteBinaryString(bw, mainManifestFileName);
            FileHelper.ActionOnTraversePackingRecordType((type) =>
            {
                FileHelper.WriteBinaryString(bw, type.ToString());
                bw.Write(countDic[type]);
                var it_record = typeToRecordsDic[type].Values.GetEnumerator();
                while (it_record.MoveNext())
                {
                    it_record.Current.WriteByBinaryWriter(bw);
                }
            });
        }

        #endregion
        // ---------------------------------------------------------------------------------
        public override void PrintInfo()
        {
            Debug.Log(" ################# " + mainManifestFileName + " ################# ");
            base.PrintInfo();
            var it_dictionary = typeToRecordsDic.GetEnumerator();
            while (it_dictionary.MoveNext())
            {
                Debug.Log("<<<<<<<<<< " + it_dictionary.Current.Key.ToString() + " >>>>>>>>>");
                var it_record = it_dictionary.Current.Value.GetEnumerator();
                while (it_record.MoveNext())
                {
                    it_record.Current.Value.PrintInfo();
                }
                Debug.Log("----------------------------------------------------------------");
            }
            Debug.Log("################################################################");
        }

        public AssetRecord GetAssetRecord(RuntimeAssetType type, string indexName)
        {
            Dictionary<string, AssetRecord> recordDic = GetRecordsDic(type);
            if (recordDic != null && recordDic.ContainsKey(indexName))
            {
                return recordDic[indexName];
            }
            else
            {
                return null;
            }
        }
 
        public Dictionary<string, AssetRecord> GetRecordsDic(RuntimeAssetType type)
        {
            if (typeToRecordsDic.ContainsKey(type))
            {
                return typeToRecordsDic[type];
            }
            else return null;
        }

        public void ActionOnTraverseRecordsDic(Action<KeyValuePair<string, AssetRecord>> OnGotRecordEnumerator)
        {
            FileHelper.ActionOnTraversePackingRecordType((type) =>
            {
                var it = GetRecordsDic(type).GetEnumerator();
                while (it.MoveNext())
                {
                    OnGotRecordEnumerator(it.Current);
                }
            });
        }

        public void UpdateInfoAndCorrectionData(string mainManifestFileName, long totalSize)
        {
            FileHelper.ActionOnTraversePackingRecordType((type) =>
            {
                if (typeToRecordsDic.ContainsKey(type) && countDic.ContainsKey(type))
                {
                    countDic[type] = typeToRecordsDic[type].Count;
                }
            });

            if (!string.IsNullOrEmpty(mainManifestFileName))
                this.mainManifestFileName = mainManifestFileName;
            _Size = totalSize;
        }

        // ---------------------------------------------------------------------------------

        //public LoadFile GetLoadFile(FileAddressType loadPathType, Action<LoadFile> onFileLoaded)
        //{
        //    return base.GetLoadFile(loadPathType, false, LoadMethod.WWW, FileAddressType.NULL, (obj)=> 
        //    {
        //        WWW www = obj as WWW;
        //        FileHelper.ReadFromBinaryArray(www.bytes, ReadByBinaryReader);

        //    }, onFileLoaded);
        //}

        public LoadFile GetLoadFile(LoadBehaviour behaviour, Action<LoadFile> onFileLoaded)
        {
            return base.GetLoadFile(behaviour, false, (obj) =>
            {
                WWW www = obj as WWW;
                FileHelper.ReadFromBinaryArray(www.bytes, ReadByBinaryReader);

            }, onFileLoaded);
        }
    }
}