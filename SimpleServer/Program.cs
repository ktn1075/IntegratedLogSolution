using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatsonWebserver;
using Newtonsoft.Json;

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

            // 아무 버튼이나 누를 경우 서버는 종료된다.
            Console.WriteLine("Press ENTER to exit");
            Console.ReadLine();
        }

        private static void SetRestHandlers()
        {
            _server.Routes.Static.Add(HttpMethod.GET, "/agent/add", async (ctx) =>
            {
                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

                keyValuePairs.Add("GroupId", "kwfkwefwkfnkafalwl230432jk3");
                keyValuePairs.Add("AgentId", "awerawerwerwrawr");

                await ctx.Response.Send(JsonConvert.SerializeObject(keyValuePairs));
            });
        }
    }
}
