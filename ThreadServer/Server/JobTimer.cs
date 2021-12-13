using ServerCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Server
{
    struct JobTimerElement : IComparable<JobTimerElement>
    {
        public int execTick; //실행시간
        public Action action; //수행할 액션
        public int CompareTo(JobTimerElement other)
        {
            return other.execTick - execTick;
        }
    }

    public class JobTimer
    {
        PriorityQueue<JobTimerElement> _pq = new PriorityQueue<JobTimerElement>();

        object _lock = new object();

        public static JobTimer Instance { get; } = new JobTimer();

        public void Push(Action action, int tickAfter = 0)
        {
            //tickAfter이후에 실행할 작업을 넣는 함수
            JobTimerElement job;
            job.execTick = System.Environment.TickCount + tickAfter;
            job.action = action;

            lock(_lock)
            {
                _pq.Push(job);
            }
        }
        //node.js
        public void Flush()
        {
            while(true)
            {
                int now = System.Environment.TickCount;

                JobTimerElement job;

                lock(_lock)
                {
                    if (_pq.Count == 0) break;

                    job = _pq.Peek();
                    if (job.execTick > now) break;

                    _pq.Pop();
                    job.action();
                }
            }
        }
    }
}
