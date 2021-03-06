﻿using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class LibraryXFrame
	{
		public LibraryXFrame()
		{
			Duration = 500;
			Image = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\n<Canvas Width=\"500\" Height=\"500\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">\n</Canvas>";
		}

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public int Duration { get; set; }

		[DataMember]
		public string Image { get; set; }
	}
}