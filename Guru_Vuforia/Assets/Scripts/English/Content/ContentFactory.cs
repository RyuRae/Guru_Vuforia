using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Content
{
    public class ContentFactory
    {
        private static Dictionary<string, IContent> cache = new Dictionary<string, IContent>();
        private delegate bool Handler(IListenerTracker v);
        private static Handler action = GetTrackerObj;
        private static string sentence = "";
        public static IContent CreateContent(List<IListenerTracker> trackers)
        {
            ContentType mode = GetTrackerType(trackers);
            if (mode == 0) return null;
            if (!cache.ContainsKey(mode.ToString()))
            {
                var nameSpace = typeof(ContentFactory).Namespace;
                string classFullName = string.Format("{0}Content", mode.ToString());

                if (!String.IsNullOrEmpty(nameSpace))
                    classFullName = nameSpace + "." + classFullName;

                Type type = Type.GetType(classFullName);
                cache.Add(mode.ToString(), Activator.CreateInstance(type) as IContent);
            }    
               
            return cache[mode.ToString()];
        }

        private static bool GetTrackerObj(IListenerTracker tracker)
        {
            var word = tracker.OnGetCurrentTransform().GetComponent<DefaultTrackableEventHandler>();
            if (word != null)
                return word.Type == TrackerType.WORD && (word.wordType == WordType.adj || word.wordType == WordType.n || word.wordType == WordType.v);
            return false;
        }
       
        public static ContentType GetTrackerType(List<IListenerTracker> trackers)//p.OnGetCurrentTransform().GetComponent<WordTrackableEventHandler>().Type == TrackerType.WORD &&  
        {
            string content = null;
            //var fruits = trackers.FindAll(p => p.OnGetCurrentTransform().GetComponent<TrackableEventHandler>());
            //if (fruits.Count > 1 && fruits.Find(p => p.Type == TrackerType.FRUIT || p.Type == TrackerType.TOOL) != null && (fruits.Find(p => p.Type == TrackerType.TOOL) != null || fruits.Find((p)=> action(p)) != null))
            //    return ContentType.Fruit;
            //字母组单词,获取所有的类型
            var letters = trackers.FindAll(p => p.Type == TrackerType.LETTER);
            if (letters.Count > 0 && letters.Count == trackers.Count)
                return ContentType.Getword;
            //var video = trackers.Find(p => p.OnGetCurrentTransform().GetComponent<VideoTrackableEventHandler>());
            //var wordwithvideo = trackers.FindAll(p => p.OnGetCurrentTransform().GetComponent<WordTrackableEventHandler>() && Global.FindChild(p.OnGetCurrentTransform(), "Video") != null);
            //if (video != null && wordwithvideo.Count == 1)
            //    return ContentType.Video;
            //var opers = trackers.FindAll(p => p.OnGetCurrentTransform().GetComponent<OperateTrackableEventHandler>());
            //var operation = opers.FindAll(p => p.Type == TrackerType.OPERATION);
            //if (opers.Count == 3 && operation.Count == 1)
            //{
            //    //opers.Find(p => p == operation);
            //    int index = opers.IndexOf(operation[0]);
            //    if (index >= 1 && opers[index - 1].Type == TrackerType.NUM && opers[index + 1].Type == TrackerType.NUM)
            //        return ContentType.Operation;
            //}


            var words = trackers.FindAll(p => p.Type == TrackerType.WORD || p.Type == TrackerType.FRUIT);
            for (int i = 0; i < words.Count; i++)
            {
                content += " " + trackers[i].OnGetCurrentTransform().name.ToLower();
            }

            sentence = content.TrimStart();
            //单词组句子
            if (words.Count == 4)
            {
                if (words.Find(p => p.OnGetCurrentTransform().GetComponent<DefaultTrackableEventHandler>().wordType == WordType.v) != null && sentence.Contains("the") && sentence.Contains("can") && words.Find(p => p.OnGetCurrentTransform().GetComponent<DefaultTrackableEventHandler>().wordType == WordType.n) != null)
                {
                    if ((words[0].OnGetCurrentTransform().name.ToLower().Equals("the") && words[2].OnGetCurrentTransform().name.ToLower().Equals("can") && words[3].OnGetCurrentTransform().GetComponent<DefaultTrackableEventHandler>().wordType == WordType.v) ||
                   words[3].OnGetCurrentTransform().name.ToLower().Equals("the") && words[1].OnGetCurrentTransform().name.ToLower().Equals("can") && words[0].OnGetCurrentTransform().GetComponent<DefaultTrackableEventHandler>().wordType == WordType.v)
                        return ContentType.Declarative;
                    else if ((words[0].OnGetCurrentTransform().name.ToLower().Equals("can") && words[1].OnGetCurrentTransform().name.ToLower().Equals("the") && words[3].OnGetCurrentTransform().GetComponent<DefaultTrackableEventHandler>().wordType == WordType.v) ||
                   (words[3].OnGetCurrentTransform().name.ToLower().Equals("can") && words[2].OnGetCurrentTransform().name.ToLower().Equals("the") && words[0].OnGetCurrentTransform().GetComponent<DefaultTrackableEventHandler>().wordType == WordType.v))
                        return ContentType.Query;
                    else
                    {
                        Debug.Log("这些单词可以拼成句子哦，交换一下卡牌顺序再试一试吧！");
                        UIManager.Instance.SetVisible(UIName.UISceneHint, true);
                        UISceneHint.Instance.ShowStatementHint("change");
                        return 0;
                    }
                }
                else if (words.Find(p => p.OnGetCurrentTransform().GetComponent<DefaultTrackableEventHandler>().wordType == WordType.adj) != null && sentence.Contains("the") && words.Find(p => p.OnGetCurrentTransform().name.ToLower().Equals("is")) != null && words.Find(p => p.OnGetCurrentTransform().GetComponent<DefaultTrackableEventHandler>().wordType == WordType.n) != null)
                {
                    if ((words[0].OnGetCurrentTransform().name.ToLower().Equals("the") && words[2].OnGetCurrentTransform().name.ToLower().Equals("is") && words[3].OnGetCurrentTransform().GetComponent<DefaultTrackableEventHandler>().wordType == WordType.adj) ||
                 (words[3].OnGetCurrentTransform().name.ToLower().Equals("the") && words[1].OnGetCurrentTransform().name.ToLower().Equals("is") && words[0].OnGetCurrentTransform().GetComponent<DefaultTrackableEventHandler>().wordType == WordType.adj))
                        return ContentType.State;
                    else if ((words[0].OnGetCurrentTransform().name.ToLower().Equals("is") && words[1].OnGetCurrentTransform().name.ToLower().Equals("the") && words[3].OnGetCurrentTransform().GetComponent<DefaultTrackableEventHandler>().wordType == WordType.adj) ||
                   words[3].OnGetCurrentTransform().name.ToLower().Equals("is") && words[2].OnGetCurrentTransform().name.ToLower().Equals("the") && words[0].OnGetCurrentTransform().GetComponent<DefaultTrackableEventHandler>().wordType == WordType.adj)
                        return ContentType.Question;
                    else
                    {
                        //Debug.Log("这些单词可以拼成句子哦，交换一下卡牌顺序再试一试吧！");
                        UIManager.Instance.SetVisible(UIName.UISceneHint, true);
                        UISceneHint.Instance.ShowStatementHint("change");
                        return 0;
                    }
                }
                else if (words.Find(p => p.OnGetCurrentTransform().GetComponent<DefaultTrackableEventHandler>().wordType == WordType.n) != null && words.Find(p => p.OnGetCurrentTransform().name.ToLower().Equals("is")) != null && (sentence.Contains("it") || sentence.Contains("this")) && (words.Find(p => p.OnGetCurrentTransform().name.ToLower().Equals("a")) != null || words.Find(p => p.OnGetCurrentTransform().name.ToLower().Equals("an")) != null))
                {
                    if (((words[0].OnGetCurrentTransform().name.ToLower().Equals("this") || words[0].OnGetCurrentTransform().name.ToLower().Equals("it")) && words[1].OnGetCurrentTransform().name.ToLower().Equals("is") && (words[2].OnGetCurrentTransform().name.ToLower().Equals("a") || words[2].OnGetCurrentTransform().name.ToLower().Equals("an"))
                   && words[3].OnGetCurrentTransform().GetComponent<DefaultTrackableEventHandler>().wordType == WordType.n) ||
                   ((words[3].OnGetCurrentTransform().name.ToLower().Equals("this") || words[3].OnGetCurrentTransform().name.ToLower().Equals("it")) && words[2].OnGetCurrentTransform().name.ToLower().Equals("is") && (words[1].OnGetCurrentTransform().name.ToLower().Equals("a") || words[1].OnGetCurrentTransform().name.ToLower().Equals("an"))
                   && words[0].OnGetCurrentTransform().GetComponent<DefaultTrackableEventHandler>().wordType == WordType.n) ||
                   (words[0].OnGetCurrentTransform().name.ToLower().Equals("is") && (words[1].OnGetCurrentTransform().name.ToLower().Equals("it") || words[1].OnGetCurrentTransform().name.ToLower().Equals("this")) && (words[2].OnGetCurrentTransform().name.ToLower().Equals("a") || words[2].OnGetCurrentTransform().name.ToLower().Equals("an"))
                   && words[3].OnGetCurrentTransform().GetComponent<DefaultTrackableEventHandler>().wordType == WordType.n) ||
                    (words[3].OnGetCurrentTransform().name.ToLower().Equals("is") && (words[2].OnGetCurrentTransform().name.ToLower().Equals("it") || words[2].OnGetCurrentTransform().name.ToLower().Equals("this")) && (words[1].OnGetCurrentTransform().name.ToLower().Equals("a") || words[1].OnGetCurrentTransform().name.ToLower().Equals("an"))
                   && words[0].OnGetCurrentTransform().GetComponent<DefaultTrackableEventHandler>().wordType == WordType.n))
                        return ContentType.Itis;
                    else
                    {
                        //Debug.Log("这些单词可以拼成句子哦，交换一下卡牌顺序再试一试吧！");
                        UIManager.Instance.SetVisible(UIName.UISceneHint, true);
                        UISceneHint.Instance.ShowStatementHint("change");
                        return 0;
                    }
                }
                else
                {
                    UIManager.Instance.SetVisible(UIName.UISceneHint, true);
                    UISceneHint.Instance.ShowStatementHint("none");
                    return 0;
                }

                //if ((words[0].OnGetCurrentTransform().name.ToLower().Equals("the") && words[2].OnGetCurrentTransform().name.ToLower().Equals("can") && words[3].OnGetCurrentTransform().GetComponent<TrackableEventHandler>().wordType == WordType.v) ||
                //    words[3].OnGetCurrentTransform().name.ToLower().Equals("the") && words[1].OnGetCurrentTransform().name.ToLower().Equals("can") && words[0].OnGetCurrentTransform().GetComponent<TrackableEventHandler>().wordType == WordType.v)
                //    return ContentType.Declarative;
                //if ((words[0].OnGetCurrentTransform().name.ToLower().Equals("the") && words[2].OnGetCurrentTransform().name.ToLower().Equals("is") && words[3].OnGetCurrentTransform().GetComponent<TrackableEventHandler>().wordType == WordType.adj) ||
                //    (words[3].OnGetCurrentTransform().name.ToLower().Equals("the") && words[1].OnGetCurrentTransform().name.ToLower().Equals("is") && words[0].OnGetCurrentTransform().GetComponent<TrackableEventHandler>().wordType == WordType.adj))
                //    return ContentType.State;
                //if ((words[0].OnGetCurrentTransform().name.ToLower().Equals("is") && words[1].OnGetCurrentTransform().name.ToLower().Equals("the") && words[3].OnGetCurrentTransform().GetComponent<TrackableEventHandler>().wordType == WordType.adj) ||
                //    words[3].OnGetCurrentTransform().name.ToLower().Equals("is") && words[2].OnGetCurrentTransform().name.ToLower().Equals("the") && words[0].OnGetCurrentTransform().GetComponent<TrackableEventHandler>().wordType == WordType.adj)
                //    return ContentType.Question;
                //if ((words[0].OnGetCurrentTransform().name.ToLower().Equals("can") && words[1].OnGetCurrentTransform().name.ToLower().Equals("the") && words[3].OnGetCurrentTransform().GetComponent<TrackableEventHandler>().wordType == WordType.v) ||
                //    (words[3].OnGetCurrentTransform().name.ToLower().Equals("can") && words[2].OnGetCurrentTransform().name.ToLower().Equals("the") && words[0].OnGetCurrentTransform().GetComponent<TrackableEventHandler>().wordType == WordType.v))
                //    return ContentType.Query;

                //if (((words[0].OnGetCurrentTransform().name.ToLower().Equals("this") || words[0].OnGetCurrentTransform().name.ToLower().Equals("it")) && words[1].OnGetCurrentTransform().name.ToLower().Equals("is") && (words[2].OnGetCurrentTransform().name.ToLower().Equals("a") || words[2].OnGetCurrentTransform().name.ToLower().Equals("an"))
                //    && words[3].OnGetCurrentTransform().GetComponent<TrackableEventHandler>().wordType == WordType.n) ||
                //    ((words[3].OnGetCurrentTransform().name.ToLower().Equals("this") || words[3].OnGetCurrentTransform().name.ToLower().Equals("it")) && words[2].OnGetCurrentTransform().name.ToLower().Equals("is") && (words[1].OnGetCurrentTransform().name.ToLower().Equals("a") || words[1].OnGetCurrentTransform().name.ToLower().Equals("an"))
                //    && words[0].OnGetCurrentTransform().GetComponent<TrackableEventHandler>().wordType == WordType.n))
                //    return ContentType.Itis;


                #region 其他句型
                /*if ((((words[0].OnGetCurrentTransform().name.ToLower().Equals("i") && words[1].OnGetCurrentTransform().name.ToLower().Equals("am")) || (words[1].OnGetCurrentTransform().name.ToLower().Equals("i") && words[0].OnGetCurrentTransform().name.ToLower().Equals("am"))) &&
                    (words[2].OnGetCurrentTransform().name.ToLower().Equals("a") || words[2].OnGetCurrentTransform().name.ToLower().Equals("an")) && words[3].OnGetCurrentTransform().GetComponent<TrackableEventHandler>().wordType == WordType.n) ||
                   (((words[3].OnGetCurrentTransform().name.ToLower().Equals("i") && words[2].OnGetCurrentTransform().name.ToLower().Equals("am")) || (words[2].OnGetCurrentTransform().name.ToLower().Equals("i") && words[3].OnGetCurrentTransform().name.ToLower().Equals("am"))) &&
                    (words[1].OnGetCurrentTransform().name.ToLower().Equals("a") || words[1].OnGetCurrentTransform().name.ToLower().Equals("an")) && words[0].OnGetCurrentTransform().GetComponent<TrackableEventHandler>().wordType == WordType.n))
                    return ContentType.Iam;
                if ((((words[0].OnGetCurrentTransform().name.ToLower().Equals("are") && words[1].OnGetCurrentTransform().name.ToLower().Equals("you")) || (words[1].OnGetCurrentTransform().name.ToLower().Equals("are") && words[0].OnGetCurrentTransform().name.ToLower().Equals("you"))) &&
                    (words[2].OnGetCurrentTransform().name.ToLower().Equals("a") || words[2].OnGetCurrentTransform().name.ToLower().Equals("an")) && words[3].OnGetCurrentTransform().GetComponent<TrackableEventHandler>().wordType == WordType.n) ||
                    (((words[3].OnGetCurrentTransform().name.ToLower().Equals("are") && words[2].OnGetCurrentTransform().name.ToLower().Equals("you")) || (words[2].OnGetCurrentTransform().name.ToLower().Equals("are") && words[3].OnGetCurrentTransform().name.ToLower().Equals("you"))) &&
                    (words[1].OnGetCurrentTransform().name.ToLower().Equals("a") || words[1].OnGetCurrentTransform().name.ToLower().Equals("an")) && words[0].OnGetCurrentTransform().GetComponent<TrackableEventHandler>().wordType == WordType.n))
                    return ContentType.Areyou;
                if ((((words[0].OnGetCurrentTransform().name.ToLower().Equals("i") && words[1].OnGetCurrentTransform().name.ToLower().Equals("have")) || (words[1].OnGetCurrentTransform().name.ToLower().Equals("i") && words[0].OnGetCurrentTransform().name.ToLower().Equals("have"))) &&
                   (words[2].OnGetCurrentTransform().name.ToLower().Equals("a") || words[2].OnGetCurrentTransform().name.ToLower().Equals("an")) && words[3].OnGetCurrentTransform().GetComponent<TrackableEventHandler>().wordType == WordType.n) ||
                   (((words[3].OnGetCurrentTransform().name.ToLower().Equals("i") && words[2].OnGetCurrentTransform().name.ToLower().Equals("have")) || (words[2].OnGetCurrentTransform().name.ToLower().Equals("i") && words[3].OnGetCurrentTransform().name.ToLower().Equals("have"))) &&
                   (words[1].OnGetCurrentTransform().name.ToLower().Equals("a") || words[1].OnGetCurrentTransform().name.ToLower().Equals("an")) && words[0].OnGetCurrentTransform().GetComponent<TrackableEventHandler>().wordType == WordType.n))
                    return ContentType.Ihave;
                if ((((words[0].OnGetCurrentTransform().name.ToLower().Equals("i") && words[1].OnGetCurrentTransform().name.ToLower().Equals("like")) || (words[1].OnGetCurrentTransform().name.ToLower().Equals("i") && words[0].OnGetCurrentTransform().name.ToLower().Equals("like")) ||
                    (words[0].OnGetCurrentTransform().name.ToLower().Equals("i") && words[1].OnGetCurrentTransform().name.ToLower().Equals("love")) || (words[1].OnGetCurrentTransform().name.ToLower().Equals("i") && words[0].OnGetCurrentTransform().name.ToLower().Equals("love"))) &&
                    words[2].OnGetCurrentTransform().name.ToLower().Equals("the") && words[3].OnGetCurrentTransform().GetComponent<TrackableEventHandler>().wordType == WordType.n) ||
                    (((words[3].OnGetCurrentTransform().name.ToLower().Equals("i") && words[2].OnGetCurrentTransform().name.ToLower().Equals("like")) || (words[2].OnGetCurrentTransform().name.ToLower().Equals("i") && words[3].OnGetCurrentTransform().name.ToLower().Equals("like")) ||
                    (words[3].OnGetCurrentTransform().name.ToLower().Equals("i") && words[2].OnGetCurrentTransform().name.ToLower().Equals("love")) || (words[2].OnGetCurrentTransform().name.ToLower().Equals("i") && words[3].OnGetCurrentTransform().name.ToLower().Equals("love"))) &&
                    words[1].OnGetCurrentTransform().name.ToLower().Equals("the") && words[0].OnGetCurrentTransform().GetComponent<TrackableEventHandler>().wordType == WordType.n))
                    return ContentType.Ilike;
                if ((((words[0].OnGetCurrentTransform().name.ToLower().Equals("i") && words[1].OnGetCurrentTransform().name.ToLower().Equals("want")) || (words[1].OnGetCurrentTransform().name.ToLower().Equals("i") && words[0].OnGetCurrentTransform().name.ToLower().Equals("want"))) &&
                  (words[2].OnGetCurrentTransform().name.ToLower().Equals("a") || words[2].OnGetCurrentTransform().name.ToLower().Equals("an")) && words[3].OnGetCurrentTransform().GetComponent<TrackableEventHandler>().wordType == WordType.n) ||
                  (((words[3].OnGetCurrentTransform().name.ToLower().Equals("i") && words[2].OnGetCurrentTransform().name.ToLower().Equals("want")) || (words[2].OnGetCurrentTransform().name.ToLower().Equals("i") && words[3].OnGetCurrentTransform().name.ToLower().Equals("want"))) &&
                  (words[1].OnGetCurrentTransform().name.ToLower().Equals("a") || words[1].OnGetCurrentTransform().name.ToLower().Equals("an")) && words[0].OnGetCurrentTransform().GetComponent<TrackableEventHandler>().wordType == WordType.n))
                    return ContentType.Iwant;*/
                #endregion
            }
            else if (words.Count == 5)
            {
                if ((words[1].OnGetCurrentTransform().name.ToLower().Equals("is") && words[2].OnGetCurrentTransform().name.ToLower().Equals("in") && words[3].OnGetCurrentTransform().name.ToLower().Equals("the")) ||
                    (words[3].OnGetCurrentTransform().name.ToLower().Equals("is") && words[2].OnGetCurrentTransform().name.ToLower().Equals("in") && words[1].OnGetCurrentTransform().name.ToLower().Equals("the")))
                    return ContentType.Inthe;
            }
            else if(words.Count > 5)
            {
                if (!ContentHelper.Instance.IsMatchWithSentencesConfig(sentence))
                {
                    //Debug.Log("哎呀，我只能显示出来最多5张卡牌，快找爸爸妈妈来帮忙吧！");
                    UIManager.Instance.SetVisible(UIName.UISceneHint, true);
                    UISceneHint.Instance.ShowStatementHint("limit");
                    return 0;
                }
            }
            return 0;
        }

    }
}
