using System;
using System.Threading;
using System.Threading.Tasks;

// TLS : 공용 자원에 계속 lock 하면 느림
// => TLS 로 땡겨옴
// 끝내면 WriteLock 걸고 넣음
// => 빠름

namespace TLS
{
    class Program
    {
        static ThreadLocal<string> ThreadName = new ThreadLocal<string>(() => $"My Name is {Thread.CurrentThread.ManagedThreadId}"); // 스레드 로컬 스토리지
        // 값이 세팅 안 되어있는데 접근하려하면 뒤의 람다를 실행시킴

        static void WhoAmI()
        {
            // ThreadName.Value = $"My Name is {Thread.CurrentThread.ManagedThreadId}";

            Thread.Sleep(1000);

            bool repeat = ThreadName.IsValueCreated;
            if(repeat)
            {
                System.Console.WriteLine(ThreadName.Value + "(repeated)");
            }
            else
            {
                System.Console.WriteLine(ThreadName.Value);
            }

            
        }


        static void Main(string[] args)
        {
            ThreadPool.SetMinThreads(1, 1);
            ThreadPool.SetMaxThreads(3, 3); // 풀에 최대 3개만
            Parallel.Invoke(WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI); // 병렬 실행이라는 의미
            // 스레드풀에서 가져옴
            // 저거 하나는 메인 스레드에서 돌릴 수도 있음
        }
    }
}