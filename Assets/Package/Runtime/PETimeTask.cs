using System;

namespace FinTOKMAK.PETimeTask
{

    public class PETimeTask
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public int tid;
        /// <summary>
        /// 下次执行时间(毫秒)
        /// </summary>
        public float destTime;
        /// <summary>
        /// 回调方法（任务ID，下次执行时间(毫秒)，延迟间隔(毫秒)，执行次数(为0时则无限执行)
        /// </summary>
        public Action<int,float,float,int> callback;
        /// <summary>
        /// 每次执行的延迟间隔（毫秒）
        /// </summary>
        public float delay;
        public int count;
        /// <summary>
        /// PE定时回调任务
        /// </summary>
        /// <param name="tid">任务id</param>
        /// <param name="callback">回调方法（任务ID，下次执行时间(毫秒)，延迟间隔(毫秒)，执行次数(为0时则无限执行)</param>
        /// <param name="destTime">下次执行时间</param>
        /// <param name="delay">延迟间隔</param>
        /// <param name="count">次数</param>
        public PETimeTask(int tid, Action<int, float, float, int> callback, float destTime,float delay,int count)
        {
            this.tid = tid;
            this.callback = callback;
            this.destTime = destTime;
            this.delay = delay;
            this.count = count;
        }
    }
    public enum PETimeUnit
    {
        /// <summary>
        /// 动画时间单位，60个动画单位=1000ms
        /// </summary>
        Millisecond,
        Second,
        Minute,
        Hour,
        Day

    }
}