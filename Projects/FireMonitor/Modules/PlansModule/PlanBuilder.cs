﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using System.Text;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace PlansModule
{
    public static class PlanLoader
    {
        //DataContractSerializer dcs
        public static Plan Load()
        {
            try
            {
                /*
                XmlSerializer deserializer = new XmlSerializer(typeof(Plan));
                StreamReader reader = new StreamReader(PathHelper.Plans);
                Plan plan = (Plan)deserializer.Deserialize(reader);
                reader.Close();

                DataContractSerializer dcs = new DataContractSerializer(typeof(Plan));
                FileStream fs = new FileStream(@"D:/del/Plans_new.xml", FileMode.Create);
                XmlDictionaryWriter xdw = XmlDictionaryWriter.CreateTextWriter(fs);
                dcs.WriteObject(xdw, plan);
                xdw.Close();

                */
                DataContractSerializer dcs = new DataContractSerializer(typeof(Plan));
                FileStream fs = new FileStream(PathHelper.Plans, FileMode.Open);
                XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
                Plan plan = (Plan)dcs.ReadObject(reader);
                reader.Close();
                return plan;
            }
            catch 
            {
                return null;
            }
        }

        static void CreateDemoData()
        {
            Plan plan = new Plan();
            plan.Name = "rootPlan";
            plan.BackgroundSource = "D:/picture6.jpg";
            plan.Caption = "cap";
            plan.Height = 500;
            plan.Width = 500;
            plan.ElementSubPlans = new List<ElementSubPlan>();
            plan.ElementSubPlans.Add(new ElementSubPlan());
            plan.ElementSubPlans[0].Name = "subPlan1";
            plan.ElementSubPlans[0].PolygonPoints = new List<PolygonPoint>();
            plan.ElementSubPlans[0].BackgroundSource = "D:/picture1.jpg";
            plan.ElementSubPlans[0].PolygonPoints.Add(new PolygonPoint() { X = 100, Y = 100 });
            plan.ElementSubPlans[0].PolygonPoints.Add(new PolygonPoint() { X = 100, Y = 200 });
            plan.ElementSubPlans[0].PolygonPoints.Add(new PolygonPoint() { X = 200, Y = 200 });
            plan.ElementSubPlans[0].PolygonPoints.Add(new PolygonPoint() { X = 200, Y = 100 });
            plan.ElementZones = new List<ElementZone>();
            plan.ElementZones.Add(new ElementZone());
            plan.ElementZones[0].ZoneNo = "0";
            plan.ElementZones[0].PolygonPoints = new List<PolygonPoint>();
            plan.ElementZones[0].PolygonPoints.Add(new PolygonPoint() { X = 300, Y = 300 });
            plan.ElementZones[0].PolygonPoints.Add(new PolygonPoint() { X = 300, Y = 400 });
            plan.ElementZones[0].PolygonPoints.Add(new PolygonPoint() { X = 400, Y = 400 });
            plan.ElementZones[0].PolygonPoints.Add(new PolygonPoint() { X = 400, Y = 300 });
            plan.ElementDevices = new List<ElementDevice>();
            plan.ElementDevices.Add(new ElementDevice());
            plan.ElementDevices[0].Left = 100;
            plan.ElementDevices[0].Top = 200;
            plan.Children = new List<Plan>();
            plan.Children.Add(new Plan());
            plan.Children[0].Name = "subPlan1";
            plan.Children[0].Caption = "Plan 2";
/*
                 XmlSerializer deserializer = new XmlSerializer(typeof(Plan));
                StreamReader reader = new StreamReader(PathHelper.Plans);
                Plan plan = (Plan)deserializer.Deserialize(reader);
                reader.Close();

                DataContractSerializer dcs = new DataContractSerializer(typeof(Plan));
                FileStream fs = new FileStream(@"D:/del/Plans_new.xml", FileMode.Create);
                XmlDictionaryWriter xdw = XmlDictionaryWriter.CreateTextWriter(fs);
                dcs.WriteObject(xdw, plan);
                xdw.Close();
                 
                
                
                DataContractSerializer dcs = new DataContractSerializer(typeof(Plan));
                FileStream fs = new FileStream(@"D:/del/Plans_new.xml", FileMode.Open);
                XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
                Plan plan = (Plan)dcs.ReadObject(reader);
                reader.Close();

 * */

        }
    }
}