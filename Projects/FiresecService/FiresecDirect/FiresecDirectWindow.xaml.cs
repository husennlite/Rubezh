﻿using System.Collections.Generic;
using System.IO;
using System.Windows;

namespace FiresecDirect
{
	public partial class FiresecDirectWindow : Window
	{
		public FiresecDirectWindow()
		{
			InitializeComponent();
			NativeFiresecClient = new Firesec.NativeFiresecClient();
		}

		Firesec.NativeFiresecClient NativeFiresecClient;

		void Window_Loaded(object sender, RoutedEventArgs e)
		{
			NativeFiresecClient.Connect("adm", "");
		}

		void OnSetNewConfig(object sender, RoutedEventArgs e)
		{
			//using (var reader = new StreamReader("SetNewConfig.xml"))
			//{
			//    textBox1.Text = reader.ReadToEnd();
			//}

			//byte[] bytes = Encoding.UTF8.GetBytes(message);
			//var memoryStream = new MemoryStream(bytes);
			//var serializer = new XmlSerializer(typeof(Firesec.CoreConfig.config));
			//Firesec.CoreConfig.config config = (Firesec.CoreConfig.config)serializer.Deserialize(memoryStream);
			//memoryStream.Close();

			//FiresecClient.SetNewConfig(config);
			//FiresecClient.DeviceWriteConfig(config, "0");
		}

		void OnGetCoreConfig(object sender, RoutedEventArgs e)
		{
			textBox1.Text = NativeFiresecClient.GetCoreConfig().Result;

			using (var fileStream = new FileStream("D:/CoreConfig.xml", FileMode.Create))
			using (var streamWriter = new StreamWriter(fileStream))
			{
				streamWriter.Write(textBox1.Text);
			}
		}

		void OnGetPlans(object sender, RoutedEventArgs e)
		{
			string plans = NativeFiresecClient.GetPlans().Result;

			using (var fileStream = new FileStream("D:/Plan.xml", FileMode.Create))
			using (var streamWriter = new StreamWriter(fileStream))
			{
				streamWriter.Write(plans);
			}

			//textBox1.Text = plans;
		}

		void OnGetCoreState(object sender, RoutedEventArgs e)
		{
			textBox1.Text = NativeFiresecClient.GetCoreState().Result;
		}

		void OnGetMetaData(object sender, RoutedEventArgs e)
		{
			textBox1.Text = NativeFiresecClient.GetMetadata().Result;

			using (var fileStream = new FileStream("D:/Metadata.xml", FileMode.Create))
			using (var streamWriter = new StreamWriter(fileStream))
			{
				streamWriter.Write(textBox1.Text);
			}
		}

		void OnGetCoreDeviceParams(object sender, RoutedEventArgs e)
		{
			textBox1.Text = NativeFiresecClient.GetCoreDeviceParams().Result;
		}

		void OnReadEvents(object sender, RoutedEventArgs e)
		{
			textBox1.Text = NativeFiresecClient.ReadEvents(0, 100).Result;
		}

		void Button_Click_6(object sender, RoutedEventArgs e)
		{
		}

		void OnBoltOpen(object sender, RoutedEventArgs e)
		{
			NativeFiresecClient.ExecuteCommand("0\\0\\6\\13", "BoltOpen");
		}

		void OnBoltClose(object sender, RoutedEventArgs e)
		{
		}

		void OnBoltStop(object sender, RoutedEventArgs e)
		{
		}

		void OnBoltAutoOn(object sender, RoutedEventArgs e)
		{
		}

		void OnBoltAutoOff(object sender, RoutedEventArgs e)
		{
		}

		void OnAddToIgnoreList(object sender, RoutedEventArgs e)
		{
			NativeFiresecClient.AddToIgnoreList(new List<string>() { "0\\0\\0\\0" });
		}

		void OnRemoveFromIgnoreList(object sender, RoutedEventArgs e)
		{
		}

		void OnAddCustomMessage(object sender, RoutedEventArgs e)
		{
			NativeFiresecClient.AddUserMessage("message");
		}
	}
}