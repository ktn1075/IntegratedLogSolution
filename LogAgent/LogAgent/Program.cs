using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace LogAgent
{
    internal static class Program
    {

        static void Main(string[] args)
        {
            // debuging 용 Conosle 실행
            // 프로젝트 설정 프로그램 속성 변경도 필요하다.
            if (Environment.UserInteractive)
            {
                LogService logService = new LogService();
                logService.TestStartupAndStop(args);
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new LogService()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
