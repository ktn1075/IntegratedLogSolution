using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogAgent.Agent
{
    public abstract class Agent
    {
        private Thread _t;
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public abstract string RestServerHostName { get; }
        public abstract int RestServerPort { get; }

        /*
        * 서버, 프로그램, 패키지 다양한 환경에서 처리를 위해 추상 클래스로 작성 
        */
        // args 순서 :  mode, hamc, 
        public static Agent New(string[] args)
        {
            switch (args[0])
            {
                case "windows":
                    return new WindowsAgent(args[1]);

                default:
                    throw new Exception($"Invalid mode: {args[0]}");
            }
        }

        private void monitor()
        {
            while(true)
            {
                Console.WriteLine("test");
                Thread.Sleep(1000);
            }
        }

        public void Start()
        {
            if (_t!= null)
                throw new Exception("이미 실행중");

            _t = new Thread(() =>
            {
                do
                {
                    try
                    {
                        monitor();
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Monitorring thread Error: {ex}");
                    }
                    for (int i = 0; i < 60 && _t != null; i++)
                        Thread.Sleep(1000);
                }
                while (_t != null);

            })
            {
                IsBackground = true
            };

            _t.Start();
        }

        // 현재는 REST/API 방식이지만 agent 마다 서버 요청 방식이 다를수도 있다.
        public abstract bool ServerRequest(string url);

        public abstract void AgentAdd(string hMac);
    }
}
