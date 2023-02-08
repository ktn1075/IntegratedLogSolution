using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace LogAgent
{
    public partial class LogService : ServiceBase
    {
        private static readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private Agent.Agent _agent;
        private Thread _s;
        private Utils.Cryptor _cryptor = Utils.Cryptor.Instance;

        public LogService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // Onstart 서비스의 시작 지점이니 hang 이 걸리게되면 서비스 등록 자체가 멈춘다.
            // 그렇기 때문에 쓰레드 형태로 START 함수를 뺀 상태로 처리한다.
            Start("windows");
        }

        protected override void OnStop()
        {

        }

        protected void Start(string target)
        {
            _s = new Thread(() =>
            {
                try
                {
                    if(Utils.RegistryManager.RegistryFind() == null)
                    {
                        // agent 에게 전달 한다.
                    }
                    else
                    {
                        string macAddress = Utils.NetworkManager.getMac();
                        string hmac = _cryptor.EncryptSHA512(macAddress, Encoding.Unicode);
                        Utils.RegistryManager.RegistryAdd(hmac);
                    }
                    _agent = Agent.Agent.New("windows");

                    _agent.Start();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            })
            {
                IsBackground = true
            };

            _s.Start();
        }
        
        internal void TestStartupAndStop(string[] args)
        {
            this.OnStart(args);
            Console.ReadLine();
            this.OnStop();
        }

    }
}
