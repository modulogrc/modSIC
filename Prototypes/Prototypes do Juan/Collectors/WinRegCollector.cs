using System;
using System.Collections;
using FrameworkNG;

namespace FrameworkNG
{
    /// <summary>
    /// === WinRegCollector ===
    /// Author: jcastro
    /// Creation Date: 21/05/2009
    /// Description: Makes data collections on the Windows Registry of remote machines. This uses a class library by Neal Bailey, used in accordance to license: http://www.codeproject.com/KB/system/everythingInWMI01.aspx
    /// How to Use: N/A
    /// Exceptions: N/A
    /// Hypoteses: Should be used with subclasses of WiRegControlSpec.
    /// Example: N/A
    /// </summary>
    public class WinRegCollector : Collector
    {
        protected WMI.Registry.RegistryObject sysRegistry;

        public override CollectResult Collect(ControlSpec spec)
        {
            CollectResult retval = new CollectResult();
            WinRegControlSpec myspec = (WinRegControlSpec)spec;

            retval.Data = sysRegistry.GetValue(myspec.Hive, myspec.RegKey, myspec.RegValue, myspec.ValueType);
            return retval;
        }

        public override void Connect(string hostname)
        {
            sysRegistry = new WMI.Registry.RegistryRemote(Auth.Username, Auth.Password, Auth.Domain, hostname);
        }

        public WinRegCollector(CollectorAuth authinfo)
            : base(authinfo)
        {
        }
        public WinRegCollector()
        {
        }

        public ArrayList SearchKeys(WMI.Registry.baseKey RootKey, string keypattern)
        {
            return sysRegistry.SearchKeys(RootKey, keypattern);
        }

        public ArrayList SearchKeys(WMI.Registry.baseKey RootKey, string keypattern, string vnamepattern)
        {
            return sysRegistry.SearchKeys(RootKey, keypattern, vnamepattern);
        }
    }
}
