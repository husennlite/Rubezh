"C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\xsd.exe" -c -l:c# -n:Firesec.CoreConfig "D:\Projects\Projects\FiresecClient\FiresecApi\XSD\GetCoreConfig.xsd"

"C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\xsd.exe" "D:\Metadata.xml" /out:"D:"
"C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\xsd.exe" -c -l:c# -n:Firesec.Metadata "D:\Metadata.xsd" /out:"D:" /fields

"C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\xsd.exe" "D:\GetCoreConfig.xml" /out:"D:"
"C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\xsd.exe" -c -l:c# -n:Firesec.CoreConfig "D:\GetCoreConfig.xsd" /out:"D:" /fields

"C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\xsd.exe" -c -l:c# -n:Firesec.CoreConfiguration   "D:/Projects/Projects/FiresecService/FiresecService/XSD/GetCoreConfig.xsd"          /out:"D:/Projects/Projects/FiresecService/FiresecService/XSD/" /fields
"C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\xsd.exe" -c -l:c# -n:Firesec.DeviceParameters    "D:/Projects/Projects/FiresecService/FiresecService/XSD/GetCoreDeviceParams.xsd"    /out:"D:/Projects/Projects/FiresecService/FiresecService/XSD/" /fields
"C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\xsd.exe" -c -l:c# -n:Firesec.CoreState           "D:/Projects/Projects/FiresecService/FiresecService/XSD/GetCoreState.xsd"           /out:"D:/Projects/Projects/FiresecService/FiresecService/XSD/" /fields
"C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\xsd.exe" -c -l:c# -n:Firesec.Metadata            "D:/Projects/Projects/FiresecService/FiresecService/XSD/GetMetaData.xsd"            /out:"D:/Projects/Projects/FiresecService/FiresecService/XSD/" /fields
"C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\xsd.exe" -c -l:c# -n:Firesec.Groups              "D:/Projects/Projects/FiresecService/FiresecService/XSD/Group.xsd"                  /out:"D:/Projects/Projects/FiresecService/FiresecService/XSD/" /fields
"C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\xsd.exe" -c -l:c# -n:Firesec.IndicatorsLogic     "D:/Projects/Projects/FiresecService/FiresecService/XSD/IndicatorLogic.xsd"         /out:"D:/Projects/Projects/FiresecService/FiresecService/XSD/" /fields
"C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\xsd.exe" -c -l:c# -n:Firesec.Plans               "D:/Projects/Projects/FiresecService/FiresecService/XSD/Plan.xsd"                   /out:"D:/Projects/Projects/FiresecService/FiresecService/XSD/" /fields
"C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\xsd.exe" -c -l:c# -n:Firesec.Journals            "D:/Projects/Projects/FiresecService/FiresecService/XSD/ReadEvents.xsd"             /out:"D:/Projects/Projects/FiresecService/FiresecService/XSD/" /fields
"C:\Program Files\Microsoft SDKs\Windows\v6.0A\bin\xsd.exe" -c -l:c# -n:Firesec.ZonesLogic          "D:/Projects/Projects/FiresecService/FiresecService/XSD/ZoneLogic.xsd"              /out:"D:/Projects/Projects/FiresecService/FiresecService/XSD/" /fields

В Plans заменить [][] на []
