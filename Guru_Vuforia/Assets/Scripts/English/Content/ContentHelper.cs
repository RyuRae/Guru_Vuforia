using LocalAsset;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Video;
using System.Text;
using FileTools;
using DevelopEngine;
using System.IO;

namespace Content
{
    public enum Tags
    {
        /// <summary>
        /// 百搭
        /// </summary>
        JOKER,
        /// <summary>
        /// 数字
        /// </summary>
        NUM,
        /// <summary>
        /// 动物
        /// </summary>
        ZOOLOGY,
        /// <summary>
        /// 颜色
        /// </summary>
        COLOR,
        /// <summary>
        /// 人物关系
        /// </summary>
        CHARACTERRELATION,
        /// <summary>
        /// 水果
        /// </summary>
        FRUIT,
        /// <summary>
        /// 动作
        /// </summary>
        ACTION,
        /// <summary>
        /// 交通工具
        /// </summary>
        VEHICLE,
        /// <summary>
        /// 衣物
        /// </summary>
        CLOTHING,
        /// <summary>
        /// 地点
        /// </summary>
        SITE,
        /// <summary>
        /// 生活
        /// </summary>
        LIFE,
        /// <summary>
        /// 身体部位
        /// </summary>
        BODYPARTS,
        /// <summary>
        /// 景物
        /// </summary>
        SCENERY,
        /// <summary>
        /// 形状
        /// </summary>
        FORM,
        /// <summary>
        /// 天气季节
        /// </summary>
        WEATHERANDSEASONS,
        /// <summary>
        /// 食物
        /// </summary>
        FOOD,
        /// <summary>
        /// 音体美
        /// </summary>
        ARTANDGYM,
        /// <summary>
        /// 植物
        /// </summary>
        BOTANY,
        /// <summary>
        /// 蔬菜
        /// </summary>
        VEGETABLES,
        /// <summary>
        /// 文具
        /// </summary>
        STATIONERY,
        /// <summary>
        /// 国家区域
        /// </summary>
        NATION,
        /// <summary>
        /// 月份星期节日
        /// </summary>
        DATE,
        /// <summary>
        /// 职业
        /// </summary>
        PROFESSION,
        /// <summary>
        /// 宇宙
        /// </summary>
        UNIVERSE,
        /// <summary>
        /// 形容词
        /// </summary>
        ADJECTIVE,
        /// <summary>
        /// 学科
        /// </summary>
        SUBJECT,
        /// <summary>
        /// 故事
        /// </summary>
        STORY
    }

    public enum WordType
    {
        /// <summary>代词</summary>
        pron,
        /// <summary>介词</summary>
        prep,
        /// <summary>副词</summary>
        adv,
        /// <summary>助词</summary>
        aux,
        /// <summary>连词</summary>
        conj,
        /// <summary>感叹词</summary>
        inter,
        /// <summary>be助词</summary>
        be_v,
        /// <summary>冠词</summary>
        art,
        /// <summary>数量词</summary>
        num,
        /// <summary>名词</summary>
        n,
        /// <summary>动词</summary>
        v,
        /// <summary>形容词</summary>
        adj,
        /// <summary>缩写词</summary>
        abbr
    }

    public class WordUnit
    {
        
        private string id;
        private int kindNum;
        private string attribute;
        private int order;
        private string word;
        private string paraphrase;
        private WordType type;
        private bool isHaveModel;

        public WordUnit()
        {

        }

        public WordUnit(string unit)
        {     
            string[] content = unit.Trim().Split(',');
            kindNum = int.Parse(content[0]);
            attribute = content[1];
            order = int.Parse(content[2]);
            word = content[3];
            paraphrase = content[4];
            type = (WordType)Enum.Parse(typeof(WordType), content[5]);
            if (content[6].ToLower().Equals("t"))
                isHaveModel = true;
            else
                isHaveModel = false;
            this.id = kindNum.ToString() + order.ToString();
        }

        public WordUnit(int kindNum, string attribute, int order, string word, string paraphrase, WordType type, bool isHaveModel)
        {
            this.kindNum = kindNum;
            this.attribute = attribute;
            this.order = order;
            this.word = word;
            this.paraphrase = paraphrase;
            this.type = type;
            this.isHaveModel = isHaveModel;
            this.id = kindNum.ToString() + order.ToString();
        }

        /// <summary>
        /// 单词的唯一标识
        /// </summary>
        public string ID { get { return id; } }

        /// <summary>
        /// 单词种类ID
        /// </summary>
        public int KindNum { get { return kindNum; }  }

        /// <summary>
        /// 单词属性分类
        /// </summary>
        public string Attribute { get { return attribute; } }

        /// <summary>
        /// 单词在所属种类中的序号
        /// </summary>
        public int Order { get { return order; } }

        /// <summary>
        /// 单词
        /// </summary>
        public string Word { get { return word; } }

        /// <summary>
        /// 单词解释
        /// </summary>
        public string Parephrase { get { return paraphrase; } }

        /// <summary>
        /// 单词词性
        /// </summary>
        public WordType Type { get { return type; } }

        /// <summary>
        /// 是否有模型
        /// </summary>
        public bool IsHaveModel { get { return isHaveModel; } }

    }

    public class DictUnit
    {
        public class Explain
        {
            private string paraphrase;

            private WordType type;

            public Explain()
            {

            }

            public Explain(string unit)
            {
                if (unit.Contains("."))
                {
                    string[] content = unit.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    if (content[0].Equals("int"))
                        content[0] = "inter";
                    type = (WordType)Enum.Parse(typeof(WordType), content[0]);
                    paraphrase = content[1];
                }
                else
                {
                    paraphrase = unit;
                }
            }

            /// <summary>
            /// 单词词性
            /// </summary>
            public WordType Type
            {
                get { return type; }
            }

            /// <summary>
            /// 单词解释
            /// </summary>
            public string Parephrase
            {
                get { return paraphrase; }
            }
        }

        private string word;
        List<Explain> exps = new List<Explain>();

        public DictUnit()
        {
        }

        public DictUnit(string unit)
        {
            string[] content = unit.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            word = content[0];

            for (int i = 1; i < content.Length; i++)
            {
                var exp = new Explain(content[i]);
                exps.Add(exp);
            }
        }

        /// <summary>
        /// 单词
        /// </summary>
        public string Word { get { return word; } }

        public List<Explain> Exps { get { return exps; } }
    }

    public class ContentHelper : MonoSingleton<ContentHelper>
    {
        public Dictionary<string, WordUnit> units;
        private Dictionary<string, WordUnit> lib;
        private float dot;//两个相邻物体之间的弧度值
        private float angle;//两个相邻物体之间的夹角
        private float width = 0.5f;

        public float limitX = 1f;

        public float limitY = 0.5f;

        public float limitAngle = 10f;

        Dictionary<int, int> angles;

        private static Dictionary<string, GameObject> cache = new Dictionary<string, GameObject>();

        private static Dictionary<string, VideoPlayer> dic = new Dictionary<string, VideoPlayer>();

       

        private List<GameObject> listModels = new List<GameObject>();
        //存储点击的特效
        private List<GameObject> list = new List<GameObject>();
        private Dictionary<Tags, string> tags = new Dictionary<Tags, string>(); 
        private GameObject clickEffect;
        private Dictionary<string, DictUnit> dict;//英汉词典 
        private List<string> sentences;
        /// <summary>
        /// 初始化加载词库
        /// </summary>
        public void Init()
        {
            sentences = new List<string>();
            dict = new Dictionary<string, DictUnit>();
            lib = new Dictionary<string, WordUnit>();
            units = new Dictionary<string, WordUnit>();
            StartCoroutine(LoadWordLib(Tips.WORD_LIB));
            StartCoroutine(LoadConfig(Tips.configPath));
            StartCoroutine(LoadDict(Tips.DictPath)); 
            StartCoroutine(LoadSentenceConfig(Tips.SentencePath));
        }

        public Dictionary<string, GameObject> GetNums()
        {
            Dictionary<string, GameObject> nums = new Dictionary<string, GameObject>();
            var _parent = GameObject.Find("Nums").transform;
            for (int i = 0; i < _parent.childCount; i++)
            {
                if (!nums.ContainsKey(_parent.GetChild(i).name))
                {
                    GameObject clone = Instantiate(_parent.GetChild(i).gameObject);
                    clone.name = _parent.GetChild(i).name;
                    nums.Add(clone.name, clone);
                    clone.SetActive(false);
                }
            }
            return nums;
        }


        public bool FindWordInDictByName(string name)
        {
            if (dict.ContainsKey(name))
                return true;
            else
                return false;
        }

        public string GetPare(string name)
        {
            DictUnit unit = GetWordInDictByName(name);
            var list = unit.Exps;
            string paraphrase = null;
            for (int i = 0; i < list.Count; i++)
            {
                paraphrase += ("," + list[i].Parephrase);
            }
            return paraphrase;
        }

        public DictUnit GetWordInDictByName(string name)
        {
            if (dict.ContainsKey(name))
                return dict[name];
            return null;
        }

        public bool GetWordTypeByName(string name)
        {
            DictUnit.Explain exp = null;
            if (dict.ContainsKey(name))
            {
                var list = dict[name].Exps;
                exp = list.Find(p => p.Type == WordType.n);
            }

            return exp != null ? true : false; 
        }

        /// <summary>根据标签获取需要加载物体</summary>
        /// <param name="tag">标签</param>
        /// <returns>要加载的物体</returns>
        public List<string> GetListByTag(string tag)
        {
            string content = Configuration.GetContent(Tips.BUTTONS, tag);
            string[] tags = content.Split('&');
            var list = new List<string>();
            List<WordUnit> words = new List<WordUnit>(lib.Values);
            var arr = new List<WordUnit>();
            for (int i = 0; i < tags.Length; i++)
            {
                string currTag = Configuration.GetContent(Tips.TAGS, tags[i]);
                //Debug.Log("当前标签:" + currTag);
                arr = words.FindAll(p => p.Attribute.Equals(currTag));
                arr.ForEach(p =>
                {
                    if (!currTag.Equals("字母"))
                        list.Add(p.Word + "_card");
                    else
                        list.Add(p.Word + "_letter_card");
                });
            }
            return list;
        }


        public bool OnVisibleShow(List<IListenerTracker> trackers)
        {
            angles = new Dictionary<int, int>();
            for (int i = 1; i < trackers.Count; i++)
            {
                //判断两个物体之间的夹角
                dot = Vector3.Dot(trackers[i].OnGetCurrentTransform().forward, trackers[i - 1].OnGetCurrentTransform().forward);
                angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
                if (!angles.ContainsKey(i))
                    angles.Add(i, (int)angle);
                else
                    angles[i] = (int)angle;
                //判断两个物体之间的水平距离
                float distanceX = Mathf.Abs(trackers[i].OnGetCurrentTransform().position.x - trackers[i - 1].OnGetCurrentTransform().position.x);
                if (Mathf.Abs(distanceX - width) > limitX)
                    return false;
                //判断两个物体之间的垂直距离
                float distanceY = Mathf.Abs(trackers[i].OnGetCurrentTransform().position.y - trackers[i - 1].OnGetCurrentTransform().position.y);
                if (distanceY > limitY)
                    return false;
            }

            foreach (var item in angles.Values)
            {
                if (item > limitAngle)
                    return false;
            }
            return true;
        }

        public void GetModel(string name, Action<GameObject> action)
        {
            GameObject tempObj = null;
            if (!cache.ContainsKey(name))
            {
                AssetsManager.Instance.Load(RuntimeAssetType.BUNDLE_PREFAB, name, false, (obj) => {
                    tempObj = GameObject.Instantiate(obj as GameObject);
                    cache.Add(name, tempObj);
                    if (action != null)
                        action(cache[name]);
                });
            }
            else
            {
                if (action != null)
                    action(cache[name]);
            }
        }

        public void SetModel(string name, GameObject obj)
        {
            if (!cache.ContainsKey(name))
                cache.Add(name, obj);
        }

        public GameObject GetModelByName(string name)
        {
            if (!cache.ContainsKey(name))
            {
                //加载bundle，从bundle加载模型

                //UISceneHome.Instance.SetText(Tips.BUNDLEPATH + name);
                GameObject obj = null;
                //StartCoroutine(LoadBundle(Tips.BUNDLEPATH + name, name, obj));
                AssetBundle bundle = LocalAssetManager.Instace.getBundle(Tips.BUNDLEPATH + name);

                if (bundle != null && bundle.Contains(name))
                {
                    obj = GameObject.Instantiate(bundle.LoadAsset(name)) as GameObject;
                    obj.SetActive(false);
                    bundle.Unload(false);//卸载bundle
                }
                else
                    Debug.LogError("没有此模型!!!");
                cache.Add(name, obj);
            }
            return cache[name];
        }

        public bool GetModelVisible(string name)
        {
            var list = new List<WordUnit>(lib.Values);
            var word = list.Find(p => p.Word.Equals(name) && p.Type == WordType.n);
            return word != null ? true : false;
        }

        //加载句子的配置文件
        IEnumerator LoadSentenceConfig(string url)
        {
            WWW www = new WWW(url);
            yield return www;
            if (www.isDone)
            {
                string content = www.text;
                string[] map = content.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < map.Length; i++)
                {
                    //Debug.Log(map[i].ToLower());
                    string curr = map[i].ToLower();
                    if (!sentences.Contains(curr))
                        sentences.Add(curr);
                }
            }
        }

        /// <summary>
        /// 匹配当前句子是否存在于配置文件
        /// </summary>
        /// <returns>返回匹配结果</returns>
        public bool IsMatchWithSentencesConfig(string content)
        {
            string result = sentences.Find(p => p.Equals(content));
            return result != null ? true : false;
        }

        //加载英汉词典
        IEnumerator LoadDict(string url)
        {
            WWW www = new WWW(url);
            yield return www;
            if (www.isDone)
            {
                string content = www.text;
                string[] map = content.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < map.Length; i++)
                {                  
                    var unit = new DictUnit(map[i]);
                    if (!dict.ContainsKey(unit.Word))
                        dict.Add(unit.Word, unit);
                    //Debug.Log(unit.Word);
                }           
            }
        }

        //加载找定的单词词库
        IEnumerator LoadWordLib(string url)
        {
            WWW www = new WWW(url);
            yield return www;
            if (www.isDone)
            {
                string content = www.text;
                string[] localFileMap = content.Split(new char[] {'\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < localFileMap.Length; i++)
                {
                    var unit = new WordUnit(localFileMap[i]);
                    if (!units.ContainsKey(unit.Word))
                        units.Add(unit.Word, unit);
                    lib.Add(unit.ID, unit);
                }
            }
        }

        //加载配置文件
        IEnumerator LoadConfig(string url)
        {
            //读取配置文件
            Configuration.LoadConfig(url);
            while (!Configuration.IsDone)
                yield return null;
        }


        private bool IsPlay = false;
        public void PrepareVideo(Transform trans)
        {
            if (!dic.ContainsKey(trans.name))
            {
                VideoPlayer player = Global.FindChild<VideoPlayer>(trans, "Video");
                if (player != null)
                    dic.Add(trans.name, player);
            }
            if (dic.ContainsKey(trans.name))
            {
                currVideo = trans.name;
                if (!IsPlay)
                {
                    dic[trans.name].gameObject.SetActive(true);
                    dic[trans.name].Play();
                    dic[trans.name].started += PlayVideo;
                  
                    IsPlay = true;
                }
            }
        }

        private void PlayVideo(VideoPlayer source)
        {
            //source.isLooping = true;
            source.Play();
        }

        string currVideo = null;
        public void StopVideo()
        {          
            if (!String.IsNullOrEmpty(currVideo) && dic.ContainsKey(currVideo))
            {
                dic[currVideo].Stop();
                dic[currVideo].gameObject.SetActive(false);
            }
            IsPlay = false;
        }

        private Vector3 worldPos;
        public void OnShowEffect(Vector3 screenPos, string name, float z)
        {
            worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, z));
            var ef = list.Find(p => !p.activeSelf);
            if (ef == null)
            {
                if (clickEffect == null)
                {
                    clickEffect = GameObject.Find("clickEffect");
                    clickEffect.SetActive(false);
                }
                ef = Instantiate(clickEffect);
                list.Add(ef);
            }

            ef.transform.position = worldPos;
            ChangeTexture(ef, name);
            for (int i = 0; i < ef.transform.childCount; i++)
            {
                ef.SetActive(true);
                var particle = ef.transform.GetChild(i).GetComponent<ParticleSystem>();
                if (particle != null)
                    particle.Play();
            }

            StartCoroutine(OnComplete(0.45f, ef));
        }

        Dictionary<string, Texture> blueNums = new Dictionary<string, Texture>();
        Dictionary<string, Texture> redNums = new Dictionary<string, Texture>();
        Dictionary<string, Texture> yellowNums = new Dictionary<string, Texture>();
        private void ChangeTexture(GameObject ef, string name)
        {
            if (!redNums.ContainsKey(name))
            {
                var redTexture = Resources.Load<Texture>("Nums/Red/" + name);
                redNums.Add(name, redTexture);
            }
            var red = Global.FindChild<Renderer>(ef.transform, "red");
            red.material.mainTexture = redNums[name];

            if (!blueNums.ContainsKey(name))
            {
                var blueTexture = Resources.Load<Texture>("Nums/Blue/" + name);
                blueNums.Add(name, blueTexture);
            }
            var blue = Global.FindChild<Renderer>(ef.transform, "blue");
            blue.material.mainTexture = blueNums[name];

            if (!yellowNums.ContainsKey(name))
            {
                var yellowTexture = Resources.Load<Texture>("Nums/Yellow/" + name);
                yellowNums.Add(name, yellowTexture);
            }
            var yellow = Global.FindChild<Renderer>(ef.transform, "yellow");
            yellow.material.mainTexture = yellowNums[name];

        }

        IEnumerator OnComplete(float time, GameObject obj)
        {
            yield return new WaitForSeconds(time);
            obj.SetActive(false);
        }

        public void OnClear()
        {
            cache.Clear();
            dic.Clear();
        }

        void OnApplicationQuit()
        {
            //Debug.Log(1111111);
            cache.Clear();
            dic.Clear();
            CancelInvoke();
            blueNums.Clear();
            redNums.Clear();
            yellowNums.Clear();
            sentences.Clear();
            dict.Clear();
            lib.Clear();
            units.Clear();
        }
    }
}
