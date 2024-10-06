namespace Mythologist_Client_WASM.Client.Infos
{
    //Big megatype for any sort of generic event that happens during gameplay
    //If you're not in the game when one of these fires, you dont get the message
    //So DONT rely on this for state syncronisation you fucking muppet.
    public class EventInfo
    {
        public enum EventType
        {
            SendText, // Uses 'Text'
            Announcement, // Uses `Text`
        };

        public EventType EEventType { get; set; }

        public ClientInfo? Sender { get; set; } //Should always be set

        public List<string>? TargetConnectionIds { get; set; } //null means everyone

        public string? Text { get; set; }
    }
}
