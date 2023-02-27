using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Threading;
using Newtonsoft.Json;
using System.Security.Principal;
using System.Diagnostics;

namespace LogAgent.Agent
{
    class WindowsAgent : Agent
    {
        AgentInfo _agentInfo = new AgentInfo();
        DenyListInfo _denyInfo = new DenyListInfo();
        Dictionary<int, string> preProcess;

        public override string RestServerHostName => "127.0.0.1";

        public override int RestServerPort => 50000;

        public override string ADD_URL => "agent/add";

        public WindowsAgent(string hMac)
        {
            AgentAdd(hMac);
        }

        // Server 등록 요청 시 등록여부 관계 없이 무조건 AgentInfo를 받아온다.
        // 처음 등록시에는 새로 생성된 정보 등록 이후에는 hMac에 해당되는 정보를 받아온다.
        // 서버로 부터 해당 값을 받을때 까지 시간 지연을 하면서 요청한다.

        protected override void AgentAdd(string hMac)
        {
            _logger.Info("---------- agent 등록 프로세스 시작---------");

            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

            keyValuePairs.Add("agentId", hMac);
            keyValuePairs.Add("alias", Environment.UserName);

            JObject jobj = null;

            int delayCount = 0;

            while (jobj == null)
            {
                jobj = ServerRequest(ADD_URL, new JObject(JsonConvert.SerializeObject(keyValuePairs))) as JObject;

                // mesc * 1000 = 1초
                // 요청회수 30번 이전까지는 1초에 한번, 90번이전까지는 3초에 한번 이후 부터는 1분에 한번씩 요청한다.
                Thread.Sleep(Utils.delayTime(delayCount) * 1000);

                delayCount++;
            }

            //TODO : 추후 JSON에서 키 존재 여부를 확인해야 한다.
            _agentInfo = JsonConvert.DeserializeObject<AgentInfo>(jobj.ToString());
            _agentInfo.hMac = hMac;
            _agentInfo.alias = Environment.UserName;

            _logger.Info($"---------- {_agentInfo.alias} 등록 프로세스 완료---------");

        }

        /*
             Request : 실행중인 APP 정보, 현재 세션 사용자, AppInfo 정보를 보낸다. 
             Response : x
             
             30초 단위로 계속해서 서버로 정보를 전송하고, 프로그램 실행과 밀접한 연관이 없기때문에
             계속해서 전송하지는 않는다.
         */

        protected override void HeartbitSend()
        {

            // TODO : 추가 필요
            // Heartbit 에는 현재 로그인한 사용자, Agent에 대한 정보, 버전 리스트가 들어간다.

            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();


        }

        protected override bool ProcessCheck()
        {
            bool IsUpdate = false;

            //TODO : 전체 리스트를 보내면 너무 많다. 이부분에 대한 논의 필요
            // 동작 
            // 1. 현재 동작하는 프로세스 리스트를 가지고온다.
            // 2. 차단 리스트와 현재 프로세스 리스트를 비교하여 차단 리스트에 등록된 프로세스 존재시 DenyList에 추가한다.
            // 3. 이전 프로세스 리스트와 현재 프로세스 리스트를 비교한다. 변동 사항이 있는 경우 
            // 4. 해당 리스트를 서버에 전송한다.
            try
            {
                Process[] allProc = Process.GetProcesses();

                Dictionary<int, string> tempProcess = new Dictionary<int, string>();

                foreach (Process p in allProc)
                {
                    tempProcess.Add(p.Id, p.ProcessName);
                }

                if (preProcess == null)
                {
                    preProcess = tempProcess;
                    IsUpdate = true;
                }
                else
                {
                    if (tempProcess.Count != preProcess.Count)
                    {
                        preProcess = tempProcess;
                        IsUpdate = true;
                    }
                    else
                    {
                        foreach (var item in tempProcess)
                        {
                            if (!preProcess.ContainsKey(item.Key))
                            {
                                preProcess = tempProcess;
                                IsUpdate = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            if (IsUpdate)
            {
                foreach (var item in preProcess)
                {
                    // TODO 
                    //  Console.WriteLine(item.Value.ToString());
                }
            }
            return false;
        }

        protected override void DenyListRequest()
        {
            // hamc 
        }
    }
}
