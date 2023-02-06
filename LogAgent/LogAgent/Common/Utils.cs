using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Utils
{
    static class RegistryManager
    {
        const string RegistryName = "HKEY_LOCAL_MACHINE";
        const string RegistrySubKey = "LogAgentKey";

        public static bool RegistryAdd()
        {
            // 키 조회
            // 키 만들기 
            // 하위 키 만들기 
            RegistryKey rkey = Registry.LocalMachine.OpenSubKey(RegistrySubKey);

            return false;
        }

        public static bool RegistryFind()
        {
            return false;
        }

        public static string RegistryGetValue()
        {
            return "test";
        }
    }

    // Json 데이터 포맷팅



}

