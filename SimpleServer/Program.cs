using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
/*
 *  백엔드 서버가 개발 될때까지 API TEST를 위해 만든 서버이다.
 *  SetResHandlers 에 API 추가 하면된다.
 */

namespace SimpleServer
{
    public partial class Program
    {
        private static Server _server = null;

        static void Main(string[] args)
        {
            _server = new Server("127.0.0.1", 50000);

            SetRestHandlers();

            _server.Start();

            Console.WriteLine("Press ENTER to exit");
            Console.ReadLine();
        }

        private static void SetRestHandlers()
        {
            // agent 등록
            _server.Routes.Static.Add(HttpMethod.POST, "/agent/add", async (ctx) =>
            {
                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

                var t = JsonConvert.DeserializeObject<Dictionary<string, string>>(ctx.Request.DataAsString);

                keyValuePairs.Add("GroupId", "kwfkwefwkfnkafalwl230432jk3");
                keyValuePairs.Add("AgentId", "awerawerwerwrawr");

                await ctx.Response.Send(JsonConvert.SerializeObject(keyValuePairs));
            });


            // agent 동작여부 검사 
            _server.Routes.Static.Add(HttpMethod.POST, "/agent/healthcheck", async (ctx) =>
            {
                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

                await ctx.Response.Send("success");
            });


            _server.Routes.Static.Add(HttpMethod.POST, "/agent/updatepolicy", async (ctx) =>
            {
                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

                JObject json_data = JObject.Parse(ctx.Request.DataAsString);
                JToken arr_data = json_data["rules"];
                JArray json_array = (JArray)arr_data;

                Console.WriteLine(json_array.Count);

                foreach (var item in json_array)
                {
                    Console.WriteLine(item.ToString());
                }

                JArray jList = new JArray();

                JObject tempJobject = new JObject();
                tempJobject.Add("ruleId", "ktn122");
                tempJobject.Add("ruleVer", "1.0.0");
                jList.Add(tempJobject);

                JObject tempJobject2= new JObject();
                tempJobject2.Add("ruleId", "ktn1223");
                tempJobject2.Add("ruleVer", "1.0.0");
                jList.Add(tempJobject2);

                JObject policyData = new JObject();

                policyData.Add("rules", jList);

                // 반환값을 강제로 만들어 전송한다.

                await ctx.Response.Send(JsonConvert.SerializeObject(policyData));
            });

        }
    }
}
