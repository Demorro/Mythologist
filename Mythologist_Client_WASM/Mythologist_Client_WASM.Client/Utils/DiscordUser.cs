namespace Mythologist_Client_WASM.Client.Utils
{
	//A DTO for the user field from the dicord api.
	//This comes from the auth response generally.
	public class DiscordUser
	{
		/// <summary>
		/// The user's id.
		/// </summary>
		public string? Id { get; set; }

		/// <summary>
		/// The user's username, not unique across the platform.
		/// </summary>
		public string? Username { get; set; }

		/// <summary>
		/// The user's 4-digit discord-tag.
		/// </summary>
		public string? Discriminator { get; set; }

		/// <summary>
		/// The user's avatar hash.
		/// </summary>
		public string? Avatar { get; set; }

		/// <summary>
		/// Whether the user belongs to an OAuth2 application.
		/// </summary>
		public bool? Bot { get; set; }

		/// <summary>
		/// Whether the user is an Official Discord System user (part of the urgent message system).
		/// </summary>
		public bool? System { get; set; }

		/// <summary>
		/// Whether the user has two factor enabled on their account.
		/// </summary>
		public bool? MfaEnabled { get; set; }

		/// <summary>
		/// The user's banner hash.
		/// </summary>
		public string? Banner { get; set; }

		/// <summary>
		/// The user's banner color encoded as an integer representation of hexadecimal color code.
		/// </summary>
		public int? AccentColor { get; set; }

		/// <summary>
		/// The user's chosen language option.
		/// </summary>
		public string? Locale { get; set; }

		/// <summary>
		/// Whether the email on this account has been verified.
		/// </summary>
		public bool? Verified { get; set; }

		/// <summary>
		/// The user's email.
		/// </summary>
		public string? Email { get; set; }

		/// <summary>
		/// The flags on a user's account.
		/// </summary>
		public int? Flags { get; set; }

		/// <summary>
		/// The type of Nitro subscription on a user's account.
		/// </summary>
		public int? PremiumType { get; set; }

		/// <summary>
		/// The public flags on a user's account.
		/// </summary>
		public int? PublicFlags { get; set; }

		/// <summary>
		/// The user's global name (if they have one).
		/// </summary>
		public string? GlobalName { get; set; }

		public Uri? AvatarUrl()
		{
			if (Avatar == null) return null;

			return new Uri($"https://cdn.discordapp.com/avatars/{Id}/{Avatar}.png");
		}

        public static DiscordUser FakeUser()
        {
            return new DiscordUser()
            {
                Id = "fake user",
                Username = "fake user name"
            };

        }
    }
}
