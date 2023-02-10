using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;

namespace LogAgent.Agent
{
    class WindowsAgent :Agent
    {
        AgentInfo _defalutInfo = new AgentInfo();

        public override string RestServerHostName => "127.0.0.1";

        public override int RestServerPort => 50000;

        public WindowsAgent(string hMac)
        {
            AgentAdd(hMac);
        }

        // Server 등록 요청 시 등록여부 관계 없이 무조건 AgentInfo를 받아온다.
        // 처음 등록시에는 새로 생성된 AgentInfo
        // 등록 이후에는 hMac에 해당되는 정보를 받아온다.
        public override void AgentAdd(string hMac)
        {
            // 


        }

        public override bool ServerRequest(string url)
        {
            using (var client =new RestClient(RestServerHostName)
            {

            }

            return false;
        }
    }

    // Server 와 통신을 위한 JSON DATA 클래스 이다.
    class AgentInfo
    {
        public String AgentId  { get; set; }
        public String Hmac { get; set; }
        public String GroupId { get; set; }
    }
}
