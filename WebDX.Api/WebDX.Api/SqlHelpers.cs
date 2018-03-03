using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace WebDX.Api.Scripts
{
    public class SqlHelpers
    {
        private const string FIELD_MATCH = ":[A-Za-z0-9_]+";

        /// <summary>
        /// Replaces :FieldName patterns with the matching field value.
        /// </summary>
        /// <param name="query">The query to process.</param>
        /// <returns>The modified query string.</returns>
        public static string PopulateQueryData(string query)
        {
            if (query == null)
                throw new ArgumentNullException("query");

            MatchCollection matches = Regex.Matches(query, FIELD_MATCH);

            Dictionary<string, IScriptField> fields = new Dictionary<string, IScriptField>();
            foreach (Match m in matches)
            {
                string fieldName = m.Value.Substring(1);
                if (string.Compare(fieldName, "FieldValue", true) == 0)
                {
                    if (!fields.ContainsKey(fieldName))
                    {
                        CurrentValueHelper cv = new CurrentValueHelper(Host.TextBox.Text);
                        fields.Add(fieldName, cv);
                    }
                }
                else
                {
                    if (!fields.ContainsKey(fieldName))
                        fields.Add(fieldName, FindField(fieldName));
                }
            }

            string result = query;
            foreach (KeyValuePair<string, IScriptField> kv in fields)
            {
                result = Regex.Replace(result, ":" + kv.Key, QuotedStr(kv.Value.Value == null ? "" : kv.Value.Value));
            }
            return result;
        }

        /// <summary>
        /// Searches the current record and the header record for the specified field.
        /// </summary>
        /// <param name="fieldName">The name of the field to find.</param>
        /// <returns>The matching field.</returns>
        public static IScriptField FindField(string fieldName)
        {
            if (fieldName == null)
                throw new ArgumentNullException("fieldName");

            try
            {
                return Host.Record.Fields[fieldName];
            }
            catch
            {
                if (Host.Record.Parent == null)
                    throw;
                else
                {
                    IScriptRecord curRec = Host.Record;
                    while ((curRec.Parent != null))
                        curRec = curRec.Parent;

                    return curRec.Fields[fieldName];
                }
            }
        }

        /// <summary>
        /// Quotes and escapes the given string value so that it matches SQL syntax requirements.
        /// </summary>
        /// <param name="value">The value to quote.</param>
        /// <returns>The modified string.</returns>
        public static string QuotedStr(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            return "'" + value.Replace("'", "''") + "'";
        }
    }

    /// <summary>
    /// Wrapper for the unposted field value.
    /// </summary>
    public class CurrentValueHelper : IScriptField
    {
        private string _value;

        /// <summary>
        /// Creates a new instance of the CurrentValueHelper class.
        /// </summary>
        /// <param name="initialValue">The initial value of Value.</param>
        public CurrentValueHelper(string initialValue)
        {
            _value = initialValue;
        }

        #region IScriptField Members

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public bool Exclude
        {
            get { throw new Exception("CurrentValueHelper: The method or operation is not implemented."); }
        }

        public string FlagDescription
        {
            get
            {
                throw new Exception("CurrentValueHelper: The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("CurrentValueHelper: The method or operation is not implemented.");
            }
        }

        public string CustomData
        {
            get
            {
                throw new Exception("CurrentValueHelper: The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("CurrentValueHelper: The method or operation is not implemented.");
            }
        }

        public string FieldName
        {
            get { throw new Exception("CurrentValueHelper: The method or operation is not implemented."); }
        }

		public int Length
		{
            get { throw new Exception("CurrentValueHelper: The method or operation is not implemented."); }
		}

		public WDEFieldStatus UpdateStatus
		{
			get
			{
                throw new Exception("CurrentValueHelper: The method or operation is not implemented.");
			}
			set
			{
                throw new Exception("CurrentValueHelper: The method or operation is not implemented.");
			}
		}

        public void Dupe()
        {
            throw new Exception("CurrentValueHelper: The method or operation is not implemented.");
        }

        public IScriptRevisions Revisions
        {
            get { throw new Exception("CurrentValueHelper: The method or operation is not implemented."); }
        }

        public double NumberValue
        {
            get { throw new Exception("CurrentValueHelper: The method or operation is not implemented."); }
        }

        #endregion
    }
}
