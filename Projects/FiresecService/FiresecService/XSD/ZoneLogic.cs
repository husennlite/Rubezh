﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:2.0.50727.3053
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=2.0.50727.3038.
// 
namespace Firesec.ZonesLogic {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
    public partial class expr {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("clause", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public clauseType[] clause;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class clauseType {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("zone", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string[] zone;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("device", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public deviceType[] device;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string joinOperator;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string operation;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string state;
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class deviceType {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string UID;
    }
}
