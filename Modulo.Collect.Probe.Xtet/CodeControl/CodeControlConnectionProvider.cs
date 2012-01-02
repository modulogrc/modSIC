using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Modulo.Collect.Probe.Common;
using System.Management;
using Modulo.Collect.Probe.Common.Exceptions;
using System.Runtime.InteropServices;
using Modulo.Collect.Probe.Xtet.CodeControlWS;
using System.ServiceModel.Channels;
using System.ServiceModel;

namespace Modulo.Collect.Probe.CodeControl
{
    public class CodeControlConnectionProvider : IConnectionProvider
    {
        public ScanWSClient connectionProvider;        
        
        public CodeControlConnectionProvider()
        {
          //  this.connectionProvider = new ScanWSClient();//new BasicHttpBinding(), new System.ServiceModel.EndpointAddress(""));
        }

        public void Connect(TargetInfo target)
        {
            var binding = new BasicHttpBinding();
            binding.MaxReceivedMessageSize = 1000000;
            binding.SendTimeout = TimeSpan.FromMinutes(10);
            this.connectionProvider = new ScanWSClient(
                binding,
                new System.ServiceModel.EndpointAddress(target.GetAddress()));
            connectionProvider.Open();
            
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

    
    }
        
}
