using Newtonsoft.Json.Linq;
using SharedLogic.Model;

namespace SharedLogic.Events
{

    public class EventCondition : ICloneable
    {
        public enum ConditionType {
            GreaterThanProperty, // Uses PlayerProperty, CompareValue
            GreaterThanOrEqualToProperty, // Uses PlayerProperty, CompareValue
            LessThanProperty, // Uses PlayerProperty, CompareValue
            LessThanOrEqualToProperty, // Uses PlayerProperty, CompareValue
            EqualToProperty, // Uses PlayerProperty, CompareValue
            ActorInScene // Uses CompareStringValue (Actor username)
        }

        public static string ConditionTypeString(ConditionType conditionType) {
            switch (conditionType) { 
                case ConditionType.GreaterThanProperty: 
                    return "Value > Property";
                case ConditionType.GreaterThanOrEqualToProperty:
                    return "Value >= Property";
                case ConditionType.LessThanProperty:
                    return "Value < Property";
                case ConditionType.LessThanOrEqualToProperty:
                    return "Value <= Property";
                case ConditionType.EqualToProperty:
                    return "Value == Property";
                case ConditionType.ActorInScene:
                    return "Is Actor in Current Scene";
            }
            return "";
        }

        public string? ConditionName {get; set;}

        public ConditionType EConditionType { get; set; }

        public string? CompareValue {get; set; }

        //You'll need/want to refresh this property when you do the event check cause it may have changed
        public PlayerPropertiesModel.PlayerProperty? PlayerProperty { get; set; }

        public bool True(List<string> allActorsInCurrentScene) {
            switch (EConditionType) {
                case ConditionType.GreaterThanProperty: return CompareToProperty();
                case ConditionType.GreaterThanOrEqualToProperty: return CompareToProperty();
                case ConditionType.LessThanProperty: return CompareToProperty();
                case ConditionType.LessThanOrEqualToProperty: return CompareToProperty();
                case ConditionType.EqualToProperty: return CompareToProperty();
                case ConditionType.ActorInScene: return IsActorInScene(allActorsInCurrentScene);
            }
            return false;
        }

        private bool CompareFloats(float conditionValue, float propertyValue)
        {
            switch (EConditionType) {
                case ConditionType.GreaterThanProperty: return conditionValue > propertyValue;
                case ConditionType.GreaterThanOrEqualToProperty: return conditionValue >= propertyValue;
                case ConditionType.LessThanProperty: return conditionValue < propertyValue;
                case ConditionType.LessThanOrEqualToProperty: return conditionValue <= propertyValue;
                case ConditionType.EqualToProperty:  return Math.Abs(conditionValue - propertyValue) <= 0.0000001;
            }
            return false;
        }

        private bool CompareStrings(string conditionValue, string propertyValue)
        {
            switch (EConditionType) {
                case ConditionType.GreaterThanProperty: return string.Compare(conditionValue, propertyValue) > 0;
                case ConditionType.GreaterThanOrEqualToProperty: return string.Compare(conditionValue, propertyValue) >= 0;
                case ConditionType.LessThanProperty: return string.Compare(conditionValue, propertyValue) < 0;
                case ConditionType.LessThanOrEqualToProperty: return string.Compare(conditionValue, propertyValue) <= 0;
                case ConditionType.EqualToProperty:  return string.Compare(conditionValue, propertyValue) == 0;
            }
            return false;
        }

        private bool CompareToProperty() {
            if (CompareValue == null) {
                return false;
            }

            if (PlayerProperty == null) {
                return false;
            }

            if (PlayerProperty.propertyValue == null) {
                return false;
            }

            if (float.TryParse(CompareValue, out float floatCompareValue)) {
                if (float.TryParse(PlayerProperty.propertyValue, out float floatPropertyValue)) {
                    return CompareFloats(floatCompareValue, floatPropertyValue);
                }
            }
            
            return CompareStrings(CompareValue, PlayerProperty.propertyValue);
        }

        private bool IsActorInScene(List<string> allActorsInCurrentScene) {
            return allActorsInCurrentScene.Any(x => x == CompareValue);
        }
        public object Clone() {
			return this.MemberwiseClone();
        }
    }
}