using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;
using Newtonsoft.Json;

namespace LogAgent.Agent
{
    public abstract class Agent
    {
        private Thread _t;
        protected static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        // 서버로 HEART_BIT를 전송하는 시간
        private const long HEARTBIT_INTERVAL = 30;
        private const long PROCESS_CHECK_INTERVAL = 2;
        private const long POLICY_UPDATE_INTERVAL = 600;

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

        private void Monitor()
        {
            // 프로그램 동작 여부 전송 시간 
            long heartBitChecked = 0;

            // 프로세스 감시 시간 
            long lastProcessChecked = 0;

            long lastPolicyUpdated = 0;

            while (_t != null)
            {
                var now = DateTime.Now.Ticks / 10_000;

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

                // 서버에서 먼저 에이전트에게 명령을 내릴수 없기때문에 인터벌로 처리한다.
                // 나중에 Heartbit 의 응답으로 해당 아이디의 정책 업데이트 여부를 받는식으로 처리한다.
                if (now > lastPolicyUpdated + POLICY_UPDATE_INTERVAL * 1000)
                {
                    PolicyUpdate();

                    lastPolicyUpdated = now;
                }

            }
        }


        protected JObject ServerRequest<T>(string URL, T param)
        {
            try
            {
                string url = $"http://{RestServerHostName}:{RestServerPort}/{URL}";

                var client = new RestClient(url);

                var request = new RestRequest
                {
                    Method = Method.Post,
                    Timeout = 1000        // mesc 
                };

                request.AddJsonBody(JsonConvert.SerializeObject(param));

                RestResponse response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    return JObject.Parse(response.Content);
                else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    // 불필요한 코드인데 이걸 어떻게 해결할까
                    _logger.Info("204");
                // 해당 agent 차단된 정보이므로 프로그램 삭제 시킨다.
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {    // TODO MSI 패키지 삭제 코드 추가
                    _logger.Debug("페이지 또는 리소스에 대한 권한이 없습니다. ");
                }
                else
                {
                    _logger.Debug($"Request Error : {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"request 전송에러 : {0}",ex);
                return null;
            }

            return null;
        }

        protected abstract void AgentAdd(string hMac);
        protected abstract void HeartbitSend();
        protected abstract void ProcessCheck();
        protected abstract void PolicyUpdate();
    }


    class AgentInfo
    {
        // 해당 데이터는 서버에서 만들어서 받아오는 데이터 
        public string agentId { get; set; }
        public string groupId { get; set; }
        
        // 해당 데이터는 agent에서 만드는 데이터 
        public string hMac { get; set; } 
        public string alias { get; set; }

        public static implicit operator JObject(AgentInfo v)
        {
            throw new NotImplementedException();
        }
    }


    // 다수의 룰을 받아 에이전트에서 처리하는게 아니라 
    // 서버에서 다수의 룰을 통합하여 보내주는 식으로 해야 하지 않을까?
    // 아니면 어떻게 하지 ?
    class RuleData
    {
        public string ruleId { get; set; }
        public string ruleVer { get; set; }
        public string ruleNm { get; set; }
        public string content { get; set; }
    }
}
