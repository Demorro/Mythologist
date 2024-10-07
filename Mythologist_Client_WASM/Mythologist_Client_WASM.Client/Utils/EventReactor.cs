using MudBlazor;
using Mythologist_Client_WASM.Client.Infos;
using SharedLogic.Events;
using System.Timers;

namespace Mythologist_Client_WASM.Client.Utils
{
    public class EventReactor
    {

        private ISnackbar snackBar;
        private Action? stateHasChangedAction { get; set; }

        //String bound to a prominent text right in the middle of the view. On a timer before being set back to null.
        public string AnnouncementString {get; private set;}
        private static int ANNOUNCEMENT_DISPLAY_TIME_MS = 6000;
        private System.Timers.Timer announcementTimer = new System.Timers.Timer(ANNOUNCEMENT_DISPLAY_TIME_MS);

        public EventReactor(ISnackbar _snackBar, Action _stateHasChanged) {
            snackBar = _snackBar;
            stateHasChangedAction = _stateHasChanged;
            announcementTimer.Elapsed += AnnouncementTimeExpired;
        }

        
        public void NotifyOfEvent(Event theEvent)
        {
            switch (theEvent.EEventType)
            {
                case Event.EventType.SendText:
                PopupRecievedChatMessage(theEvent);
                return;
                case Event.EventType.Announcement:
                DisplayAnnouncement(theEvent.Text);
                return;

            }
            Console.WriteLine("ERROR: Received unrecognized event type");
        }

        private void PopupRecievedChatMessage(Event theEvent)
        {
            string? senderUserName = theEvent.SenderUserName;
            if(senderUserName != null)
            {
                snackBar.Configuration.PositionClass = Defaults.Classes.Position.BottomCenter;
                snackBar.Add($"{senderUserName}: {theEvent.Text}", Severity.Normal, config =>
                {
                    config.Icon = Icons.Material.Filled.Message;
                    config.IconColor = Color.Info;
                    config.IconSize = Size.Large;
                });
            }
        }

        private void AnnouncementTimeExpired(Object source, ElapsedEventArgs e) {
            AnnouncementString = "";
            stateHasChangedAction?.Invoke();
            announcementTimer.Stop();
        }

        private void DisplayAnnouncement(string announcementText) {
            announcementTimer.Stop();
            AnnouncementString = announcementText;
            stateHasChangedAction?.Invoke();
            announcementTimer.Start();
        }


    }
}
