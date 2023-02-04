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

        /*
        * 서버, 프로그램, 패키지 다양한 환경에서 처리를 위해 추상 클래스로 작성 
        */

        public static Agent New(string mode)
        {
            switch (mode)
            {
                case "windows":
                    return new WindowsAgent();

                default:
                    throw new Exception($"Invalid mode: {mode}");
            }
        }

        private void test2()
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
                        test2();
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
    }
}
