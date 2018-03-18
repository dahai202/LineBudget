using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using System.Configuration;
using System.Web;
using System.Xml;
using System.Web.Configuration;
using System.Collections.Specialized;

namespace X2Lib
{
    ///// <summary>
    ///// for configuration info
    ///// </summary>
    //public class X2Provider {
        
    //    private NameValueCollection _ProviderAttributes;
    //    private string _ProviderName;
    //    private string _ProviderType;

    //    public NameValueCollection Attributes {
    //        get { return _ProviderAttributes; }
    //    }
    //    public string Name
    //    {
    //        get { return _ProviderName; }
    //    }

    //    public string Type
    //    {
    //        get { return _ProviderType; }
    //    }

    //    public X2Provider(string providerName)
    //    {
    //        switch (providerName)
    //        {
    //            case "SqlDataProvider":
    //                _ProviderName = providerName;
    //                _ProviderType = "data";
    //                _ProviderAttributes = new NameValueCollection();
    //                Attributes.Add("connectionString", WebConfigurationManager.AppSettings["dbconnect"]);
    //                Attributes.Add("objectQualifier", "");
    //                Attributes.Add("databaseOwner", "");
    //                break;
    //            default:
    //                break;
    //        }
        
        
    //    }

    //    /// <summary>
    //    /// create a provider by xml collection
    //    /// </summary>
    //    /// <param name="Attributes"></param>
    //    public X2Provider(XmlAttributeCollection Attributes)
    //    { 
    //        _ProviderName = Attributes["name"].Value;

    //        // Set the type of the provider
    //        _ProviderType = Attributes["type"].Value;

    //        //Store all the attributes in the attributes bucket
    //        foreach (XmlAttribute Attribute in Attributes)
    //        {
    //             if(Attribute.Name != "name" && Attribute.Name != "type") 
    //             {
    //                 _ProviderAttributes.Add(Attribute.Name, Attribute.Value);
    //             }
    //        }
    //    }


    //    /// <summary>
    //    /// tmp provider 
    //    /// </summary>
    //    internal void tmpNewSqlProvider()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}


    ///// <summary>
    ///// manage all the provider in the system
    ///// </summary>
    //public class X2ProviderConfiguration
    //{
    //    /// <summary>
    //    /// store all provider of a specific type, such as in data provider,we can have sql access.etc..etc..
    //    /// </summary>
    //    private Hashtable _Providers;

    //    /// <summary>
    //    /// the default data provider of this type
    //    /// </summary>
    //    private string _DefaultProvider;


    //    /// <summary>
    //    /// get this type of provider list
    //    /// </summary>
    //    /// <param name="ProviderType"></param>
    //    /// <returns></returns>
    //    public static X2ProviderConfiguration GetProviderConfiguration(string ProviderType)
    //    {
    //        ///TODO::should load this type of providers CONFIGURATION from configure file
    //        ///but now ,add it by mannul
    //        X2ProviderConfiguration pc = new X2ProviderConfiguration();
    //        ///add a default provider for this providerType
    //        X2Provider newProvider = new X2Provider("SqlDataProvider");
    //        pc.AddProvider("SqlDataProvider", newProvider);
    //        pc.DefaultProvider = "SqlDataProvider";

    //        return pc;
    //     //   pc.addSetting("data",WebConfigurationManager.ConnectionStrings);
    //    }

    //    public string DefaultProvider
    //    {
    //        get { return _DefaultProvider; }
    //        set { _DefaultProvider = value; }
    //    }

    //    /// <summary>
    //    /// provider
    //    /// </summary>
    //    public Hashtable Providers
    //    {
    //        get { return _Providers; }
    //    }

    //    /// <summary>
    //    /// add a new provider into configure
    //    /// </summary>
    //    /// <param name="ProviderName"></param>
    //    /// <param name="provider"></param>
    //    public void AddProvider(string ProviderName,X2Provider provider)
    //    {
    //        if (_Providers == null)
    //        {
    //            _Providers = new Hashtable();
    //        }
    //        _Providers.Add(ProviderName, provider);        
    //    }

    //}
}
