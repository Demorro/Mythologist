using System.Diagnostics;

namespace SharedLogic.Events
{
    //Big megatype for any sort of generic event that happens during gameplay
    //If you're not in the game when one of these fires, you dont get the message
    //So DONT rely on this for state syncronisation you fucking muppet.
    public class Event : ICloneable
    {
        public string? EventName {get; set; }

        public enum EventType
        {
            SendText, // Uses 'Text'
            Announcement, // Uses `Text`
        };

        public EventType EEventType { get; set; }

        public static string EventTypeName(EventType type) {
            switch(type) {
                case EventType.SendText:
                    return "Send Message";
                case EventType.Announcement:
                    return "Announcement";
                default:
                    Debug.Assert(false);
                    return "";
            }
        }

        public string? SenderUserName { get; set; } //Should always be set

        public IEnumerable<string>? TargetUsernames { get; set; } //null means everyone

        public string? Text { get; set; }

        public List<EventCondition> conditions = new List<EventCondition>();

        public object Clone() {
			Event clone = new Event();
            clone.EventName = this.EventName;
            clone.EEventType = this.EEventType;
            clone.SenderUserName = this.SenderUserName;
            if (TargetUsernames != null){
                clone.TargetUsernames = new List<string>();
                List<string> userNames = (List<string>)clone.TargetUsernames;
                foreach(string? targetUsername in this.TargetUsernames) {
                    userNames.Add(targetUsername);
                }
            }
            clone.Text = this.Text;
            foreach(EventCondition condition in conditions) {
                clone.conditions.Add((EventCondition)condition.Clone());
            }

            return clone;
        }
    }
}
