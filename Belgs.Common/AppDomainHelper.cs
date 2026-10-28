using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Belgs.Common
{
    //Thank you LMS for your help in writing this class.
    [Serializable]
    public class AppDomainHelper
    {

        public delegate void CrossAppDomainCallDelegate(params object[] payload);
        public delegate object CrossAppDomainCallRetValueDelegate(params object[] payload);

        public static AppDomain GetAppDomainByName(string name)
        {
            return GetAppDomains().FirstOrDefault(appDomain => appDomain.FriendlyName == name);
        }

        public static void InvokeOnAppDomain(AppDomain appDomain, CrossAppDomainCallDelegate targetFunc, params object[] payload)
        {
            appDomain.SetData(appDomain.FriendlyName + "_payload", payload);
            appDomain.SetData(appDomain.FriendlyName + "_func", targetFunc);
            appDomain.DoCallBack(InvokedOnAppDomain);
        }

        public static T InvokeOnAppDomain<T>(AppDomain appDomain, CrossAppDomainCallRetValueDelegate targetFunc, params object[] payload)
        {
            appDomain.SetData(appDomain.FriendlyName + "_payload", payload);
            appDomain.SetData(appDomain.FriendlyName + "_func", targetFunc);
            appDomain.DoCallBack(InvokedOnAppDomainRet);

            return (T)Convert.ChangeType(appDomain.GetData("result"), typeof(T));
        }

        private static void InvokedOnAppDomain()
        {
            AppDomain currentAppDomain = AppDomain.CurrentDomain;
            string name = currentAppDomain.FriendlyName;

            // Grab payload.
            object[] payload = (object[])currentAppDomain.GetData(name + "_payload");
            CrossAppDomainCallDelegate func = (CrossAppDomainCallDelegate)currentAppDomain.GetData(name + "_func");

            func.Invoke(payload);
        }

        private static void InvokedOnAppDomainRet()
        {
            AppDomain currentAppDomain = AppDomain.CurrentDomain;
            string name = currentAppDomain.FriendlyName;

            // Grab payload.
            object[] payload = (object[])currentAppDomain.GetData(name + "_payload");
            CrossAppDomainCallRetValueDelegate func = (CrossAppDomainCallRetValueDelegate)currentAppDomain.GetData(name + "_func");

            object result = func.Invoke(payload);
            currentAppDomain.SetData("result", result);
        }

        public static IList<AppDomain> GetAppDomains()
        {
            IList<AppDomain> domains = new List<AppDomain>();
            IntPtr enumHandle = IntPtr.Zero;
            ICorRuntimeHost host = (ICorRuntimeHost)new CorRuntimeHost();

            try
            {
                host.EnumDomains(out enumHandle);
                while (true)
                {
                    host.NextDomain(enumHandle, out object domain);
                    if (domain == null) break;
                    domains.Add((AppDomain)domain);
                }

                return domains;
            }
            finally
            {
                host.CloseEnum(enumHandle);
                Marshal.ReleaseComObject(host);
            }
        }

        // Replaces the mscoree COM reference, which only builds with Visual Studio's MSBuild.
        [ComImport, Guid("CB2F6723-AB3A-11D2-9C40-00C04FA30A3E")]
        private class CorRuntimeHost { }

        // Slots must stay in vtable order up to CloseEnum; the ones after it are never called.
        [ComImport, Guid("CB2F6722-AB3A-11D2-9C40-00C04FA30A3E"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface ICorRuntimeHost
        {
            void CreateLogicalThreadState();
            void DeleteLogicalThreadState();
            void SwitchInLogicalThreadState(IntPtr pFiberCookie);
            void SwitchOutLogicalThreadState(out IntPtr pFiberCookie);
            void LocksHeldByLogicalThread(out uint pCount);
            void MapFile(IntPtr hFile, out IntPtr hMapAddress);
            void GetConfiguration(out IntPtr pConfiguration);
            void Start();
            void Stop();
            void CreateDomain([MarshalAs(UnmanagedType.LPWStr)] string pwzFriendlyName, [MarshalAs(UnmanagedType.IUnknown)] object pIdentityArray, [MarshalAs(UnmanagedType.IUnknown)] out object pAppDomain);
            void GetDefaultDomain([MarshalAs(UnmanagedType.IUnknown)] out object pAppDomain);
            void EnumDomains(out IntPtr hEnum);
            void NextDomain(IntPtr hEnum, [MarshalAs(UnmanagedType.IUnknown)] out object pAppDomain);
            void CloseEnum(IntPtr hEnum);
        }
    }
}
