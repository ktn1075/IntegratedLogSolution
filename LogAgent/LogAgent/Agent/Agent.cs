using System;
using System.Collections.Generic;
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

        protected void Monitor()
        {

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

        public abstract void AgentAdd(string hMac);
        public abstract void HeartbitSend();
    }

    class AgentInfo
    {
        public String AgentId { get; set; }
        public String GroupId { get; set; }
    }

    
}
