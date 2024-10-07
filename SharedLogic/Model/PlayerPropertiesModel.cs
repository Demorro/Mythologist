using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLogic.Model
{
	public class PlayerPropertiesModel
	{
        public class PlayerProperty : ICloneable
        {

			public PlayerProperty() {
				name = "New Name";
				propertyName = "New Property";
				propertyValue = "New Value";
			}
			public PlayerProperty(string _name, string _propertyName, string _propertyValue) {
				name = _name;
				propertyName = _propertyName;
				propertyValue = _propertyValue;
			}

			public string? name;
			public string? propertyName;
			public string? propertyValue;

            public object Clone() {
				return this.MemberwiseClone();
            }
        }
		public List<PlayerProperty> playerProperties {get; set;} = new List<PlayerProperty>();
		public bool treatAsWhitelist {get; set;} = false;
	}
}
