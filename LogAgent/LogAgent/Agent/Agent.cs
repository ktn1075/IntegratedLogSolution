using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace LogAgent.Agent
{
    public abstract class Agent
    {
        private Thread _t;
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        // 서버로 HEART_BIT를 전송하는 시간
        private const long HEARTBIT_INTERVAL = 30;
        private const long PROCESS_CHECK_INTERVAL = 2;
        private const long DENY_LIST_INTERVAL = 600;

        public abstract string RestServerHostName { get; }
        public abstract int RestServerPort { get; }
        
        // TODO:  리소스 클래스를 추가하여 관리한다.
        public abstract string ADD_URL { get; }

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
                        Monitor();
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

        private  void Monitor()
        {
            // 프로그램 동작 여부 전송 시간 
            long heartBitChecked = 0;

            // 프로세스 감시 시간 
            long lastProcessChecked = 0;

            // 차단 리스트 업데이트 시간 
            long lastDenyListChecked = 0;

            while (_t != null)
            {
                var now = DateTime.Now.Ticks / 10_000;

                if  (now > lastDenyListChecked + DENY_LIST_INTERVAL * 1000)
                {
                    DenyListRequest();

                    lastDenyListChecked = now;
                }

                if (now > heartBitChecked + HEARTBIT_INTERVAL * 1000)
                {
                    HeartbitSend();

                    heartBitChecked = now;
                }
                
                if (now > lastProcessChecked + PROCESS_CHECK_INTERVAL * 1000)
                {
                    ProcessCheck();

                    lastProcessChecked = now;
                }
            }
        }

        protected JObject ServerRequest(string URL, Dictionary<string, string> param)
        {
            try
            {
                string url = $"http://{RestServerHostName}:{RestServerPort}/{URL}";

                var client = new RestClient(url);

                var request = new RestRequest
                {
                    Method = Method.Get,
                    Timeout = 1000        // mesc 
                };

                foreach (KeyValuePair<string, string> pair in param)
                {
                    request.AddParameter(pair.Key, pair.Value, ParameterType.QueryString);
                }

                RestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return JObject.Parse(response.Content);
            }
            catch (Exception ex)
            {
                _logger.Error($"request 전송에러 : {0}",ex);
            }

            return null;
        }

        protected abstract void AgentAdd(string hMac);
        protected abstract void HeartbitSend();
        protected abstract bool ProcessCheck();
        protected abstract void DenyListRequest();
    }

    class AgentInfo
    {
        public String AgentId { get; set; }
        public String GroupId { get; set; }
    }

    class DenyListInfo
    {
        // rule id , 시간, 차단 리스트 , category

    }
}
