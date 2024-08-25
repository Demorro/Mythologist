using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts;
using MudBlazor;
using Mythologist_CRUD.Components.Layout;
using SharedLogic.Services;
using System.Data.Common;

namespace Mythologist_CRUD.Utils
{
    public class Utils
    {
        /*
         * For most stuff in the CRUD app, we get the game name/password, and do checks
         * based on what that says. Rather than duplicate that on OnAfterRenderAsync
         * every time, just call this
         * You should ONLY call this during the first render
         * Returns (userName, GMPassword)
         */
        public static async Task<(string, string)> PageSetup( ISessionStorageService sessionStorage, MainLayout layout, IDatabaseConnectionService dbConnection, NavigationManager navManager, ISnackbar snackbar)
        {
            string? gameName = null;
            string? gmPassword = null;

            var getGameNameTask = sessionStorage.GetItemAsync<string>("GameName");
            var getGMPasswordTask = sessionStorage.GetItemAsync<string>("GMPassword");
            gameName = await getGameNameTask;
            gmPassword = await getGMPasswordTask;

            if (gameName == null)
            {
                snackbar.Add("Couldn't find game, redirecting.");
                navManager.NavigateTo("/");
                return ("", "");
            }

            bool verified = await dbConnection.VerifyLogin(gameName, gmPassword);
            if (!verified)
            {
                snackbar.Add("Not authorized to be here. The authorities have been informed.");
                navManager.NavigateTo("/");
                return ("", "");
            }

            layout.SetHeaderName(gameName);
            layout.SetNavMenuEnabled(true);
            layout.SetGMPassword(gmPassword);

            return (gameName, gmPassword);
        }
    }
}
