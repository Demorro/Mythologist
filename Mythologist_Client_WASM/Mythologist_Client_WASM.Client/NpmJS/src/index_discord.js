import { DiscordSDK } from "@discord/embedded-app-sdk";
import { patchUrlMappings } from '@discord/embedded-app-sdk';

// Annoyingly discord requires you to forward all your external url calls in the portal.
// However, you can't always call a nice "/local/url" path, cause you're using an external lib (like blazor)
// This lets you intercept and patch those urls. You still need to do the redirect in the portal though.
patchUrlMappings([{ prefix: "/_blazor/initializers", target: "https://holly-fly-arbitrary-adequate.trycloudflare.com/_blazor/initializers" }]);
patchUrlMappings([{ prefix: "/_framework/blazor.boot.json", target: "https://holly-fly-arbitrary-adequate.trycloudflare.com/_framework/blazor.boot.json" }]);
patchUrlMappings([{ prefix: "/_framework", target: "https://holly-fly-arbitrary-adequate.trycloudflare.com/_framework" }]);

let auth; //The users access token, once we have exchanged our access code for it and authenticated
const discordClientId = "1266185455616004128";
const discordSdk = new DiscordSDK(discordClientId);

setupDiscordSdk().then(() => {
    console.log("Discord SDK is authenticated");
    // We can now make API calls within the scopes we requested in setupDiscordSDK()
    // Note: the access_token returned is a sensitive secret and should be treated as such
});

async function setupDiscordSdk() {
    await discordSdk.ready();
    console.log("Discord SDK is ready");

    // Authorize with Discord Client
    const { code } = await discordSdk.commands.authorize({
        client_id: discordClientId,
        response_type: "code",
        state: "",
        prompt: "none",
        scope: [
            "identify",
            "guilds",
        ],
    });

    // Retrieve an access_token from your activity's server
    // Note: We need to prefix our backend `/api/token` route with `/.proxy` to stay compliant with the CSP.
    // Read more about constructing a full URL and using external resources at
    // https://discord.com/developers/docs/activities/development-guides#construct-a-full-url
    const response = await fetch("/api/discordtoken", {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify({
            code,
        }),
    });
    const { access_token } = await response.json();

    // Authenticate with Discord client (using the access_token)
    auth = await discordSdk.commands.authenticate({
        access_token,
    });

    if (auth == null) {
        throw new Error("Authenticate command failed");
    }
}

window.getDiscordAccessToken = function () {
    console.log(auth)
    return auth["access_token"];
}

window.getDiscordUserInfo = function () {
    console.log(auth)
    return auth["user"];
}

//Used to delete the loading div. See Routes.razor
window.removeElementById = function (id) {
    var element = document.getElementById(id);
    element.parentNode.removeChild(element);
}

window.getElementWidth = function (elementId) {
    var element = document.getElementById(elementId);
    if (element) {
        return element.offsetWidth;
    }
    return 0;
}

