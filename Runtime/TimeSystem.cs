using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FinTOKMAK.PETimeTask
{
    public class TimeSystem : MonoBehaviour
    {

        public static TimeSystem Instance;
        private static readonly string obj = "lock";
        private int tid;
        private List<int> tidList = new List<int>();

        private List<PETimeTask> timeTaskList = new List<PETimeTask>();
        private List<PETimeTask> tmpTaskList = new List<PETimeTask>();

        // Update is called once per frame
        public void Awake()
        {
            if (TimeSystem.Instance != null)
            {
                Debug.LogError("TimeSystem脚本重复挂载");
            }
            else
            {
                Instance = this;
            }
        }


        void Update()
        {
        // 加入缓存区的定时任务(作用是等到下一帧循环时才执行，而不是这一帧)
            for (int i = 0; i < tmpTaskList.Count; i++)
            {
                timeTaskList.Add(tmpTaskList[i]);
            }
            tmpTaskList.Clear();
            for (int i = 0;i < timeTaskList.Count; i++)
            {
                PETimeTask task = timeTaskList[i];
            // Debug.Log("IF DT:" + task.destTime + "TT:" + Time.realtimeSinceStartup*1000);
                if (Time.realtimeSinceStartup*1000f < task.destTime)
                {
                    continue;
                }
                else
                {
                    //Debug.Log("Do D:" +task.delay + "| DT:" + task.destTime + "| TT:" + Time.realtimeSinceStartup * 1000);
                    task.callback?.Invoke(task.tid,task.destTime,task.delay,task.count);
                    if (task.count == 1)
                    {
                        timeTaskList.RemoveAt(i);
                        i--;//循环内自减后要对索引进行偏移
                    }
                    else
                    {
                        if (task.count != 0)
                        {
                            task.count -= 1;
                        }
                        task.destTime += task.delay; 
                    }
                }
            }
        }
        /// <summary>
        /// 新增定时回调任务(callback:执行任务ID,下次执行时间,执行间隔,剩余执行次数,)
        /// </summary>
        /// <param name="callback">回调方法(执行任务ID,下次执行时间,执行间隔,剩余执行次数)</param>
        /// <param name="delay">定时时间</param>
        /// <param name="timeUnit">时间格式</param>
        /// <param name="count">循环次数，0为无限循环</param>
        /// <returns>返回定时回调任务的id</returns>
        public int AddTimeTask(Action<int, float, float, int> callback, float delay,PETimeUnit timeUnit=PETimeUnit.Second,int count=1)
        {
            if (timeUnit != PETimeUnit.Millisecond)
            {
                switch (timeUnit)
                {
                    case PETimeUnit.Second:
                        delay = delay * 1000f;
                        break;
                    case PETimeUnit.Minute:
                        delay = delay * 1000f*60f;
                        break;
                    case PETimeUnit.Hour:
                        delay = delay * 1000f * 60f*60f;
                        break;
                    case PETimeUnit.Day:
                        delay = delay * 1000f * 60f*60f * 24f;
                        break;
                    default:
                        Debug.Log("添加时间类型错误");
                        break;
                }
            }
            int tid=GetTid();
        
            float destTime = Time.realtimeSinceStartup*1000 + delay;
            //Debug.Log("Add D:" + delay + "| DT:" + destTime + "| TT:" + Time.realtimeSinceStartup * 1000);
            tmpTaskList.Add(new PETimeTask(tid,callback, destTime, delay, count));
            tidList.Add(tid);
            return tid;

        }

        /// <summary>
        /// 删除定时回调任务
        /// </summary>
        /// <param name="tid"></param>
        /// <returns>是否删除成功</returns>
        public bool DeleteTimeTask(int tid)
        {
            bool exist = false;
            for (int i = 0; i < timeTaskList.Count; i++)
            {
                PETimeTask task = timeTaskList[i];
                if (task.tid == tid)
                {
                    timeTaskList.RemoveAt(i);
                    for (int i2 = 0; i2 < tidList.Count; i2++)
                    {
                        if (tidList[i2] == tid)
                        {
                            tidList.RemoveAt(i2);
                            break;
                        }
                    }
                    exist = true;
                    break;
                }
            }
            return exist;
        }
        /// <summary>
        /// 获取定时ID
        /// </summary>
        /// <returns></returns>
        private int GetTid()
        {
            lock (obj)
            {
                tid +=1;
                //安全代码防止tid溢出或者重复
                while (true)
                {
                    if (tid == int.MaxValue)//tid达到最大时
                    {
                        tid = 0;
                    }
                    bool used = false;
                    for (int i = 0; i < tidList.Count; i++)//遍历已使用的tid列表，查看是否有使用过
                    {
                        if(tid==tidList[i])
                        {
                            used = true;
                            break;//有使用过，跳出for
                        }
                    }
                    if (!used)//判断是否有使用过
                    {
                        break;//没有使用过，该tid有效，直接退出死循环
                    }
                    else//使用过，在该tid的基础上+1,然后继续循环
                    {
                        tid += 1;
                    }
                }
            }
            return tid;
        }
    }

}