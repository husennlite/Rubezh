﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;

namespace ServiceHosting
{
    internal class NativeComServer
    {
        static FS_Types.IFSC_Connection connectoin;
        static FS_Types.IFSC_Connection Connectoin
        {
            get
            {
                if (connectoin == null)
                    connectoin = GetConnection();
                return connectoin;
            }
        }

        internal static string GetCoreConfig()
        {
            mscoree.IStream stream = Connectoin.GetCoreConfig();

            return ReadFromStream(stream);
        }

        internal static string GetCoreState()
        {
            mscoree.IStream stream = Connectoin.GetCoreState();

            return ReadFromStream(stream);
        }

        internal static string GetMetaData()
        {
            mscoree.IStream stream = Connectoin.GetMetaData();

            return ReadFromStream(stream);
        }

        internal static void SetNewConfig(string coreConfig)
        {
            //Connectoin.SetNewConfig();
        }

        internal static void ResetStates(string states)
        {
            Connectoin.ResetStates(states);
        }

        internal static string GetCoreDeviceParams()
        {
            return Connectoin.GetCoreDeviceParams();
        }

        static FS_Types.IFSC_Connection GetConnection()
        {
            ObjectHandle objectHandle = Activator.CreateComInstanceFrom("Interop.FS_Types.dll", "FS_Types.FSC_LIBRARY_CLASSClass");
            FS_Types.FSC_LIBRARY_CLASSClass library = (FS_Types.FSC_LIBRARY_CLASSClass)objectHandle.Unwrap();
            FS_Types.TFSC_ServerInfo serverInfo = new FS_Types.TFSC_ServerInfo();
            serverInfo.Port = 211;
            serverInfo.ServerName = "localhost";

            NotificationCallBack notificationCallBack = new NotificationCallBack();

            FS_Types.IFSC_Connection connectoin = library.Connect2("adm", "", serverInfo, notificationCallBack);

            return connectoin;
        }

        static string ReadFromStream(mscoree.IStream stream)
        {
            string message = "";

            unsafe
            {
                while (true)
                {
                    byte* unsafeBytes = stackalloc byte[1024];
                    IntPtr _intPtr = new IntPtr(unsafeBytes);
                    uint bytesRead = 0;
                    stream.Read(_intPtr, 1024, out bytesRead);
                    if (bytesRead == 0)
                        break;

                    byte[] bytes = new byte[bytesRead];
                    for (int i = 0; i < bytesRead; i++)
                    {
                        bytes[i] = unsafeBytes[i];
                    }
                    message += Encoding.Default.GetString(bytes);
                }
            }
            return message;
        }
    }

    public class NotificationCallBack : FS_Types.IFS_CallBack
    {
        public void NewEventsAvailable(int EventMask)
        {
            //string coreStateString = ComServer.GetCoreStateString();
            //CoreState.config coreState = ComServer.GetCoreState();
            //ComEventAggregator.OnComEvent(coreStateString, coreState);
        }
    }
}