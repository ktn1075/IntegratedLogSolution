using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Net.NetworkInformation;

namespace Utils
{
    // 자주 쓰는 객체가 아니기에 싱글톤으로 만들지 않는다.
    public static class RegistryManager
    {
        const string RegistryName = "HKEY_LOCAL_MACHINE";
        const string RegistrySubKey = @"Software\LogAgentKey";
        const string HMAC = "UniqeKey"; 

        public static void RegistryAdd()
        {
            RegistryKey regKey = Registry.LocalMachine.CreateSubKey(RegistrySubKey);

            regKey.SetValue(HMAC,"");

        }

        public static bool RegistryFind()
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(RegistrySubKey);

            if (registryKey == null)   
                return false;

            return true;
        }

        public static string RegistryGetValue()
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(RegistrySubKey);

            return registryKey.GetValue(HMAC).ToString();
        }
    }

    // 현재는 MAC주소를 받아오는 부분만 있지만 추가 소켓 등 추가 할 예정 
    // 자주 쓰는 객체가 아니기에 싱글톤으로 만들지 않는다.

    public static class NetworkManager
    {
       public static string getMac()
        {
            string macAddress = NetworkInterface.GetAllNetworkInterfaces()
            .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
            .Select(nic => nic.GetPhysicalAddress().ToString()).FirstOrDefault();

            return macAddress;
        }
    }

  
}

