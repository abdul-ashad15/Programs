using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Reflection;

namespace WebDX.Api
{
    /// <summary>  
    /// VersionHelper is an internal static object that follows the Singleton pattern. 
    /// This object will contain dictionaries that are hard-coded to contain information related to which 
    /// properties are filtered in the various versions. These dictionaries will only need to contain 
    /// information on properties that have changed over the lifetime of the API versions. Properties 
    /// not contained in the dictionaries will be assumed to be writable in any version.
    /// </summary>
    public sealed class VersionHelper
    {
        #region fields
        public static readonly VersionHelper _instance = new VersionHelper();
        private static Dictionary<string, WebDXApiVersion> dXmlChangesVersionOnPropiertiesAndCollections;
        private static Dictionary<string, WebDXApiVersion> dXmlChangesVersionOnEnumerationsElements;
        private static Dictionary<string, WebDXApiVersion> dXmlChangesVersionOnEnumerationsPrefixes;
        private static Dictionary<string, string> dLogDataLost;

        private static bool _isDataLost;
        #endregion

        #region Properties

        /// <summary>
        /// This property is a flag that indicates if there was data lost recollected
        /// during the write process.
        /// </summary>
        public static bool IsDataLost
        {
            get { return VersionHelper._isDataLost; }
        }
        
        /// <summary>
        /// This property contains the lost data recollected
        /// during the write process.
        /// </summary>
        public static string LostData
        {
            get {
                StringBuilder lostData = new StringBuilder();
                foreach (KeyValuePair<string, string> pair in dLogDataLost)
                {
                    lostData.Append("[" + pair.Key + "," + pair.Value + "]");
                }
                return lostData.ToString(); 
            }
        }

        #endregion

        #region Constructors
        private VersionHelper() 
        {
            dXmlChangesVersionOnPropiertiesAndCollections = new Dictionary<string, WebDXApiVersion>();
            dXmlChangesVersionOnEnumerationsElements = new Dictionary<string, WebDXApiVersion>();
            dXmlChangesVersionOnEnumerationsPrefixes = new Dictionary<string, WebDXApiVersion>();
            GetCustomAttributes(new VersionPropertyFilterAttribute(), new VersionEnumFilterAttribute(), new OutputPrefixAttribute());
            //This is a workaround in the attribute Conf
            dXmlChangesVersionOnPropiertiesAndCollections.Add("IWDECharRepair.Conf", new WebDXApiVersion("1.0.0.0","1.3.6.0"));

            dLogDataLost = new Dictionary<string, string>();
        }
        #endregion

        #region Methods

        /// <summary>
        /// Initialize can be used to clean up the lost data list and to perform other initialization functions as necessary.
        /// </summary>
        public static void Initialize()
        {
            _isDataLost = false;
            dLogDataLost.Clear();            
        }

        /// <summary>
        /// This method will return the full name including prefix, if any should be applied.
        /// </summary>
        /// <param name="enumerationName">Represents the enumeration name.</param>
        /// <param name="enumElement">Represents the enumeration element name.</param>
        /// <param name="fileVersion">Represents the API file version.</param>
        /// <returns></returns>
        public static string GetEnumerationString<T>(string enumerationName, T enumElement, string fileVersion)
        {
            WebDXApiVersion value;
            string result = string.Empty;
            string[] vEnumElements;

            vEnumElements = enumElement.ToString().Split(',');

            if (string.IsNullOrEmpty(fileVersion))
            {
                //TODO: Do I need to throw an exception?
                return result;
            }
            Version currentFileVersion = new Version(fileVersion);

            foreach (string iEnumElement in vEnumElements)
            {
                string eElement = iEnumElement.Trim();
                if (dXmlChangesVersionOnEnumerationsElements.TryGetValue(enumerationName + "." + eElement, out value))
                {
                    if (value != null)
                    {
                        if (value.VersionFinal != null)
                        {
                            if (currentFileVersion >= value.VersionInitial && value.VersionFinal >= currentFileVersion)
                            {
                                if (typeof(WDEMiscFlags).Name == enumerationName && WDEMiscFlags.None.ToString() == eElement)
                                {
                                    result = result + ",";
                                }
                                else
                                {
                                    result = result + GetEnumerationPrefix(enumerationName, fileVersion) + eElement + ",";                                    
                                }
                            }
                        }
                        else
                        {
                            if (value.VersionInitial != null)
                            {
                                if (currentFileVersion >= value.VersionInitial)
                                {
                                    if (typeof(WDEMiscFlags).Name == enumerationName && WDEMiscFlags.None.ToString() == eElement)
                                    {
                                        result = result + ",";
                                    }
                                    else
                                    {                                        
                                        result = result + GetEnumerationPrefix(enumerationName, fileVersion) + eElement + ",";
                                    }
                                }
                            }
                            else
                            {
                                result = result + GetEnumerationPrefix(enumerationName, fileVersion) + eElement + ",";
                            }
                        }
                    }
                }
                else
                {
                    LogDataLost(enumerationName + "." + eElement, eElement);
                }
            }

            if (!string.IsNullOrEmpty(result))
            {
                return result.Substring(0,result.Length -1);
            }
            else
            {
                return enumElement.ToString();
            }
        }


        /// <summary>
        /// This method will return the prefix, if any should be applied.
        /// </summary>
        /// <param name="enumerationName">Represents the enumeration name.</param>
        /// <param name="fileVersion">Represents the API file version.</param>
        /// <returns></returns>
        public static string GetEnumerationPrefix(string enumerationName, string fileVersion)
        {
            WebDXApiVersion value;
            Version currentFileVersion = new Version(fileVersion);
            string result = string.Empty;

            if (dXmlChangesVersionOnEnumerationsPrefixes.TryGetValue(enumerationName, out value))
            {
                if (value != null)
                {
                    if (value.VersionFinal != null)
                    {
                        if (currentFileVersion >= value.VersionInitial && value.VersionFinal >= currentFileVersion)
                        {
                            result = value.Prefix;
                        }
                    }
                    else
                    {
                        if (value.VersionInitial != null)
                        {
                            if (currentFileVersion >= value.VersionInitial)
                            {
                                result = value.Prefix;
                            }
                        }
                        else
                        {
                            result = value.Prefix;
                        }
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// This will return true if the collection should be excluded during the write process. 
        /// It will return false otherwise. The format of the property name will be Interface.Collection. 
        /// For example: “IWDEField.CharRepairs”.
        /// </summary>
        /// <param name="collectionName">Collec</param>
        /// <param name="fileVersion"></param>
        /// <returns></returns>
        public static bool FilterPropertyOrCollection(string collectionName, string fileVersion)
        {
            WebDXApiVersion value;
            bool result = true;

            if (string.IsNullOrEmpty(fileVersion))
            {
                return result;
            }

            Version currentFileVersion = new Version(fileVersion);                        
            if (currentFileVersion == null)
            {
                return result;
            }

            if (dXmlChangesVersionOnPropiertiesAndCollections.TryGetValue(collectionName, out value))
            {
                if (value != null)
                {
                    if (value.VersionFinal != null)
                    {
                        if (currentFileVersion >= value.VersionInitial && value.VersionFinal >= currentFileVersion)
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        if (value.VersionInitial != null)
                        {
                            if (currentFileVersion >= value.VersionInitial)
                            {
                                result = false;
                            }
                        }
                        else
                        {
                            result = false;
                        }
                    }

                }
            }

            return result;
        }

        /// <summary>
        /// Get custom attributes according the attributesVector given, 
        /// then the dictionaries are populated after the beforementioned step 
        /// </summary>
        /// <param name="attributesVector">Contains the attributes to look for</param>
        private void GetCustomAttributes(params Attribute[] attributesVector)
        {
            try
            {
                Assembly a = Assembly.GetExecutingAssembly();
                Type[] types = a.GetTypes();

                foreach (Type t in types)
                {
                    if (t.IsClass || t.IsEnum)
                    {
                        MemberInfo[] fi = t.GetMembers();
                        Attribute[] attributes;
                        //First looking for custom attributes in the Type
                        foreach (Attribute attr in attributesVector)
                        {
                            attributes = (Attribute[])t.GetCustomAttributes(attr.GetType(), true);
                            if (attributes.Length > 0)
                            {
                                PopulateAttributes(attributes);
                            }
                        }
                        //Second looking for custom attributes in the Members
                        foreach (MemberInfo obj in fi)
                        {
                            foreach (Attribute attr in attributesVector)
                            {
                                attributes = (Attribute[])obj.GetCustomAttributes(attr.GetType(), true);
                                if (attributes.Length > 0)
                                {
                                    PopulateAttributes(attributes);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

        }

        /// <summary>
        /// Retrieving the attributes
        /// </summary>
        /// <param name="attributes">Contains the attributes to look for</param>
        private void PopulateAttributes(Attribute[] attributes)
        {
            foreach (Object attribute in attributes)
            {
                if (attribute.GetType() == typeof(VersionPropertyFilterAttribute))
                {
                    VersionPropertyFilterAttribute vfa = attribute as VersionPropertyFilterAttribute;
                    if (vfa != null)
                    {
                        dXmlChangesVersionOnPropiertiesAndCollections.Add(vfa.Parent + "." + vfa.Name, new WebDXApiVersion(vfa.InitialVersion, vfa.FinalVersion));
                    }
                    continue;
                }

                if (attribute.GetType() == typeof(VersionEnumFilterAttribute))
                {
                    VersionEnumFilterAttribute vef = attribute as VersionEnumFilterAttribute;
                    if (vef != null)
                    {
                        dXmlChangesVersionOnEnumerationsElements.Add(vef.Parent + "." + vef.Name, new WebDXApiVersion(vef.InitialVersion, vef.FinalVersion));
                    }
                    continue;
                }

                if (attribute.GetType() == typeof(OutputPrefixAttribute))
                {
                    OutputPrefixAttribute opa = attribute as OutputPrefixAttribute;
                    if (opa != null)
                    {
                        dXmlChangesVersionOnEnumerationsPrefixes.Add(opa.EnumName, new WebDXApiVersion(opa.Prefix, opa.InitialVersion, opa.FinalVersion));
                    }
                    continue;
                }
            }
        }

        /// <summary>
        /// The potential for data loss exists when writing out data to older versions. 
        /// A method on VersionHelper should be created to log the occurrences of data loss
        /// </summary>
        /// <param name="memberName">The memberName parameter will match the value passed into 
        /// the FilterXXX method. In the case of enumeration values being filtered, the 
        /// GetEnumerationString call on VersionHelper will call LogDataLoss itself and pass
        /// in the enumeration type name as the memberName value.</param>
        /// <param name="value">The value is the string representation of the value that would have been written out.</param>
        public static void LogDataLost(string memberName, string value)
        {
            _isDataLost = true;
            if (!dLogDataLost.ContainsKey(memberName))
            {
                dLogDataLost.Add(memberName, value);
            }
        }
        #endregion

    }

    /// <summary>
    /// This class contains the initial version and final version of the WebDX XML element, i.e, enum, enum element, property or collection.
    /// Besides, contains the prefix if this applies.
    /// </summary>
    public class WebDXApiVersion
    {
        Version _versionInitial = null;
        Version _versionFinal = null;
        string _prefix = string.Empty;

        #region Constructor()
        public WebDXApiVersion(string versionInitial, string versionFinal)
        {
            try
            {
                if (!string.IsNullOrEmpty(versionInitial))
                {
                    _versionInitial = new Version(versionInitial);
                }
                if (!string.IsNullOrEmpty(versionFinal))
                {
                    _versionFinal = new Version(versionFinal);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Wrong error version syntax", e);
            }
        }
        public WebDXApiVersion(string prefix, string versionInitial, string versionFinal)
        {
            try
            {
                _prefix = prefix;
                if (!string.IsNullOrEmpty(versionInitial))
                {
                    _versionInitial = new Version(versionInitial);
                }
                if (!string.IsNullOrEmpty(versionFinal))
                {
                    _versionFinal = new Version(versionFinal);
                }

            }
            catch (Exception e)
            {
                throw new Exception("Wrong error version syntax", e);
            }
        }

        public WebDXApiVersion()
        {
        }
        #endregion

        #region Properties
        public Version VersionInitial
        {
            get { return _versionInitial; }
            set { _versionInitial = value; }
        }


        public Version VersionFinal
        {
            get { return _versionFinal; }
            set { _versionFinal = value; }
        }


        public string Prefix
        {
            get { return _prefix; }
            set { _prefix = value; }
        }
        #endregion
    }
}
