using System;
using System.Collections.Generic;
using UnityEngine;

namespace FinTOKMAK.PETimeTask
{
    public class TimeSystem : MonoBehaviour
    {
        public static TimeSystem Instance;
        private static readonly string obj = "lock";
        private int _tid;
        /// <summary>
        /// The dictionary that stores all the tid-PETimeTask pair
        /// </summary>
        private readonly Dictionary<int, PETimeTask> _tidTaskTable = new Dictionary<int, PETimeTask>();
        /// <summary>
        /// All the running task
        /// </summary>
        private readonly List<PETimeTask> _timeTaskList = new List<PETimeTask>();
        /// <summary>
        /// The task buffer storing all the tasks which will be added to the _timeTaskList next frame
        /// </summary>
        private readonly List<PETimeTask> _addTaskBuffer = new List<PETimeTask>();

        // Update is called once per frame
        public void Awake()
        {
            if (Instance != null)
                Debug.LogError("TimeSystem脚本重复挂载");
            else
                Instance = this;
        }


        private void Update()
        {
            // 加入缓存区的定时任务(作用是等到下一帧循环时才执行，而不是这一帧)
            for (var i = 0; i < _addTaskBuffer.Count; i++) _timeTaskList.Add(_addTaskBuffer[i]);
            _addTaskBuffer.Clear();
            for (var i = 0; i < _timeTaskList.Count; i++)
            {
                var task = _timeTaskList[i];
                // Debug.Log("IF DT:" + task.destTime + "TT:" + Time.realtimeSinceStartup*1000);
                if (Time.realtimeSinceStartup * 1000f >= task.destTime)
                {
                    // Debug.Log("Do D:" +task.delay + "| DT:" + task.destTime + "| TT:" + Time.realtimeSinceStartup * 1000);
                    task.callback?.Invoke(task.tid, task.destTime, task.delay, task.count);
                    if (task.count == 1)
                    {
                        // Task finished
                        _timeTaskList.RemoveAt(i);
                        i--; // 循环内自减后要对索引进行偏移
                    }
                    else
                    {
                        // Perform the task multiple times
                        if (task.count != 0) task.count -= 1;
                        task.destTime += task.delay;
                    }
                }
            }
        }

        /// <summary>
        ///     新增定时回调任务(callback:执行任务ID,下次执行时间,执行间隔,剩余执行次数,)
        /// </summary>
        /// <param name="callback">回调方法(执行任务ID,下次执行时间,执行间隔,剩余执行次数)</param>
        /// <param name="delay">定时时间</param>
        /// <param name="timeUnit">时间格式</param>
        /// <param name="count">循环次数，0为无限循环</param>
        /// <returns>返回定时回调任务的id</returns>
        public int AddTimeTask(Action<int, float, float, int> callback, float delay,
            PETimeUnit timeUnit = PETimeUnit.Second, int count = 1)
        {
            if (timeUnit != PETimeUnit.Millisecond)
                switch (timeUnit)
                {
                    case PETimeUnit.Second:
                        delay = delay * 1000f;
                        break;
                    case PETimeUnit.Minute:
                        delay = delay * 1000f * 60f;
                        break;
                    case PETimeUnit.Hour:
                        delay = delay * 1000f * 60f * 60f;
                        break;
                    case PETimeUnit.Day:
                        delay = delay * 1000f * 60f * 60f * 24f;
                        break;
                    default:
                        Debug.Log("添加时间类型错误");
                        break;
                }

            var tid = GetTid();

            var destTime = Time.realtimeSinceStartup * 1000 + delay;
            // Debug.Log("Add D:" + delay + "| DT:" + destTime + "| TT:" + Time.realtimeSinceStartup * 1000);
            PETimeTask task = new PETimeTask(tid, callback, destTime, delay, count);
            _addTaskBuffer.Add(task);
            // Add the task to the tid-Task table
            _tidTaskTable.Add(tid, task);
            return tid;
        }

        /// <summary>
        ///     删除定时回调任务
        /// </summary>
        /// <param name="tid"></param>
        /// <returns>是否删除成功</returns>
        public bool DeleteTimeTask(int tid)
        {
            // If the task with certain tid do not exist
            if (!_tidTaskTable.ContainsKey(tid))
            {
                return false;
            }

            // remove the task
            _timeTaskList.Remove(_tidTaskTable[tid]);
            
            // remove the tid-Task pair in the table
            _tidTaskTable.Remove(tid);

            return true;
        }

        /// <summary>
        ///     获取定时ID
        /// </summary>
        /// <returns></returns>
        private int GetTid()
        {
            lock (obj)
            {
                _tid += 1;
                // TODO: 如果_tidList中包含所有的int值，此处会进入死循环。需要解决
                // 安全代码防止tid溢出或者重复
                while (true)
                {
                    // tid达到最大时
                    if (_tid == int.MaxValue)
                        _tid = 0;
                    
                    // If the tid is not used, break the while look
                    if (!_tidTaskTable.ContainsKey(_tid))
                        break;
                    
                    _tid += 1;
                }
            }

            return _tid;
        }
    }
}