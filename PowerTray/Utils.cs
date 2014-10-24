using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PowerTray
{
    static class Utils
    {
        public enum PowerDataAccessor : uint {
            ACPowerSettingIndex = 0x00,
            DCPowerSettingIndex = 0x01,
            Scheme              = 0x10,
            Subgroup            = 0x11,
            IndividualSetting   = 0x12,
            ActiveScheme        = 0x13,
            CreateScheme        = 0x14,
        }

        [DllImport("powrprof.dll")]
        public static extern UInt32 PowerGetActiveScheme
        (
            IntPtr UserRootPowerKey, 
            ref IntPtr ActivePolicyGuid
        );

        [DllImport("powrprof.dll")]
        public static extern UInt32 PowerSetActiveScheme
        (
            IntPtr UserRootPowerKey, 
            IntPtr SchemeGuid
        );

        [DllImport("powrprof.dll")]
        public static extern UInt32 PowerEnumerate
        (
            IntPtr RootPowerKey, 
            IntPtr SchemeGuid, 
            IntPtr SubGroupOfPowerSettingsGuid, 
            PowerDataAccessor AccessFlags,
            UInt32 Index, 
            Byte[] Buffer, 
            ref UInt32 BufferSize
        );

        [DllImport("powrprof.dll")]
        public static extern UInt32 PowerReadFriendlyName
        (
            IntPtr RootPowerKey, 
            IntPtr SchemeGuid, 
            IntPtr SubGroupOfPowerSettingsGuid, 
            IntPtr PowerSettingGuid, 
            Byte[] Buffer, 
            ref UInt32 BufferSize
        );
    }
}
