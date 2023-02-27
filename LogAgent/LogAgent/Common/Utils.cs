using Microsoft.Win32;
using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Principal;
/*
*  윈도우 서비스는 영역 0에서 돌아가기 때문에 레지스트리에 접근하는게 문제가 없지만
*  디버깅 시에는 관리자권한으로 ide를 실행해야 한다.
*/

public static class RegistryManager
{
    // TODO :  현재는 관련 설정 값을 들고 있지만 나중에는 CONFIG 파일로 만들어서 처리한다.
    const string RegistryName = "HKEY_LOCAL_MACHINE";
    const string RegistrySubKey = @"SOFTWARE\LogAgentKey";
    const string HMAC = "UniqeKey";

    public static void RegistryAdd(string hamc)
    {
        RegistryKey regKey = Registry.LocalMachine.CreateSubKey(RegistrySubKey, RegistryKeyPermissionCheck.ReadWriteSubTree);

        regKey.SetValue(HMAC, hamc);

    }

    public static string RegistryFind()
    {
        RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(RegistrySubKey);

        if (registryKey == null)
            return null;

        return registryKey.GetValue(HMAC).ToString();
    }
}


public class SystemInfoManager
{

    /* 현재 0번째 어뎁터의 값을 가지고 오는 방법도 있지만. 
       가상환경으로 실행시 실제 MAC주소를 못받아 올 수 있다.
       NIC 접근해서 직접 가지고 온다.
    */
    public static string getMac()
    {
        string macAddress = NetworkInterface.GetAllNetworkInterfaces()
        .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
        .Select(nic => nic.GetPhysicalAddress().ToString()).FirstOrDefault();

        return macAddress;
    }

    public static List<string> getRunAppList()
    {

        //TODO 실행중인 Process 받아온다 (상태가 running)

        return new List<string>();
    }
}

public class Cryptor
{
    // 해당 클래스는 자주 사용하기에 싱글톤 방식으로 만들어 놓고 사용한다.

    private static readonly Lazy<Cryptor> _instance = new Lazy<Cryptor>(() => new Cryptor());

    public static Cryptor Instance => _instance.Value;

    public string EncryptSHA512(string text, Encoding encoding)
    {
        var sha = new System.Security.Cryptography.SHA512Managed();
        byte[] data = sha.ComputeHash(encoding.GetBytes(text));

        var sb = new StringBuilder();

        foreach (byte b in data)
        {
            sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }
}


public class Utils
{
    static public int delayTime(int delayCount)
    {
        if (delayCount <= 30) return 1;
        else if (delayCount <= 90) return 3;
        else return 60;
    }
}


