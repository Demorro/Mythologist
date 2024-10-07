using SharedLogic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLogic.Utils
{
    public class ReferenceString
    {
        public ReferenceString(string _val) { value = _val; }
        public string? value {get; set; }

        // Overrides for Equals, GetHashCode and ToString are important for MudSelect
		public override bool Equals(object o)
		{
			var other = o as ReferenceString;
			return other?.value == value;
		}
		public override int GetHashCode() => value?.GetHashCode() ?? 0;

		public override string ToString()
		{
			return value;
		}
    }
}
