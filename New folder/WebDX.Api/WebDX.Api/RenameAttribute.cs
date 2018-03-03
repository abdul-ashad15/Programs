using System;
using System.Collections.Generic;
using System.Text;

namespace WebDX.Api
{
    public class RenameAttribute : Attribute
    {
        private string _newName;

        public RenameAttribute(string newName)
            : base()
        {
            _newName = newName;
        }

        public string NewName
        {
            get
            {
                return _newName;
            }
        }
    }
}
