﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS2Api;
using System.Threading;

namespace ServerFS2.Service
{
	public static class CallbackManager
	{
		static object locker = new object();
		static List<FSAgentCallbacCash> FSAgentCallbacCashes = new List<FSAgentCallbacCash>();
		static int LastIndex { get; set; }

		public static void Add(FS2Callbac fsAgentCallbac)
		{
			lock (FSAgentCallbacCashes)
			{
				FSAgentCallbacCashes.RemoveAll(x => (DateTime.Now - x.DateTime) > TimeSpan.FromMinutes(1));

				LastIndex++;
				var callbackResultSaver = new FSAgentCallbacCash()
				{
					FS2Callbac = fsAgentCallbac,
					Index = LastIndex,
					DateTime = DateTime.Now
				};
				FSAgentCallbacCashes.Add(callbackResultSaver);
			}
			ClientsManager.ClientInfos.ForEach(x => x.PollWaitEvent.Set());
		}

		public static List<FS2Callbac> Get(ClientInfo clientInfo)
		{
			if (IsConnectionLost)
			{
				Thread.Sleep(TimeSpan.FromSeconds(1));
				var result = new List<FS2Callbac>();
				var fsAgentCallbac = new FS2Callbac()
				{
					IsConnectionLost = IsConnectionLost
				};
				result.Add(fsAgentCallbac);
				return result;
			}

			lock (FSAgentCallbacCashes)
			{
				var result = new List<FS2Callbac>();
				var safeCopy = FSAgentCallbacCashes.ToList();
				foreach (var callbackResultSaver in safeCopy)
				{
					if (callbackResultSaver.Index > clientInfo.CallbackIndex)
					{
						result.Add(callbackResultSaver.FS2Callbac);
					}
				}
				if (safeCopy.Count > 0)
				{
					clientInfo.CallbackIndex = safeCopy.Max(x => x.Index);
				}
				return result;
			}
		}

		static bool IsConnectionLost = false;
		public static void SetConnectionLost(bool value)
		{
			IsConnectionLost = value;
			ClientsManager.ClientInfos.ForEach(x => x.PollWaitEvent.Set());
		}
	}

	public class FSAgentCallbacCash
	{
		public FS2Callbac FS2Callbac { get; set; }
		public int Index { get; set; }
		public DateTime DateTime { get; set; }
	}
}