using System;
using System.Collections.Generic;
using System.Text;

namespace WebDX.Api
{
    /// <summary>
    /// Custom attribute for the prefixes
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum)]
    public class OutputPrefixAttribute : System.Attribute
    {
        public OutputPrefixAttribute()
        {
        }
        // attribute constructor for 
        // positional parameters
        public OutputPrefixAttribute
           (string prefix,
            string initialVersion,
            string finalVersion,
            string enumName)
        {
            this._prefix = prefix;
            this._initialVersion = initialVersion;
            this._finalVersion = finalVersion;
            this._enumName = enumName;
        }

        // property for Prefix parameter
        public string Prefix
        {
            get
            {
                return _prefix;
            }
            set
            {
                this._prefix = value;
            }
        }

        // property for Initial version parameter
        public string InitialVersion
        {
            get
            {
                return _initialVersion;
            }
            set
            {
                _initialVersion = value;
            }
        }

        // property for Final version parameter
        public string FinalVersion
        {
            get
            {
                return _finalVersion;
            }
            set
            {
                _finalVersion = value;
            }
        }

        // property Name
        public string EnumName
        {
            get
            {
                return _enumName;
            }
            set
            {
                _enumName = value;
            }
        }

        // private member data 
        private string _prefix;
        private string _initialVersion;
        private string _finalVersion;
        private string _enumName;
    }

    /// <summary>
    /// Custom attribute for the attributes
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    class VersionPropertyFilterAttribute : System.Attribute
    {
        public VersionPropertyFilterAttribute()
        {
        }
        // attribute constructor for 
        // positional parameters
        public VersionPropertyFilterAttribute
           (string initialVersion,
            string finalVersion,
            string name,
            string parent
            )
        {
            this._initialVersion = initialVersion;
            this._finalVersion = finalVersion;
            this._name = name;
            this._parent = parent;
        }

        // property for Initial version parameter
        public string InitialVersion
        {
            get
            {
                return _initialVersion;
            }
            set
            {
                _initialVersion = value;
            }
        }

        // property for Final version parameter
        public string FinalVersion
        {
            get
            {
                return _finalVersion;
            }
            set
            {
                _finalVersion = value;
            }
        }

        // property Name
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        // property attribute's parent
        public string Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }


        // private member data 
        private string _initialVersion;
        private string _finalVersion;
        private string _name;
        private string _parent;


    }

    /// <summary>
    /// Custom attribute for the enum attributes
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field)]
    class VersionEnumFilterAttribute : System.Attribute
    {
        public VersionEnumFilterAttribute()
        {
        }
        // attribute constructor for 
        // positional parameters
        public VersionEnumFilterAttribute
           (string initialVersion,
            string finalVersion,
            string name,
            string parent
            )
        {
            this._initialVersion = initialVersion;
            this._finalVersion = finalVersion;
            this._name = name;
            this._parent = parent;
        }

        // property for Initial version parameter
        public string InitialVersion
        {
            get
            {
                return _initialVersion;
            }
            set
            {
                _initialVersion = value;
            }
        }

        // property for Final version parameter
        public string FinalVersion
        {
            get
            {
                return _finalVersion;
            }
            set
            {
                _finalVersion = value;
            }
        }

        // property Name
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        // property attribute's parent
        public string Parent
        {
            get
            {
                return _parent;
            }
            set
            {
                _parent = value;
            }
        }


        // private member data 
        private string _initialVersion;
        private string _finalVersion;
        private string _name;
        private string _parent;


    }

}
