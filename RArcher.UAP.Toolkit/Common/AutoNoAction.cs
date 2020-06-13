using System;

namespace RArcher.UAP.Toolkit.Common
{
    public class AutoNoAction : Attribute, IAutoAttribute
    {
        public object DefaultValue { get; set; }

        public bool SaveNullValues { get; set; }

        public bool RestoreNullValues { get; set; }
    }
}