using System.Linq;
using System.Net.NetworkInformation;
using KitchenLib.Preferences;
using TwitchLib.Unity;

namespace ChefPlusPlus.Utils
{
    public class TwitchAuth
    {
        internal static string DecryptionKey = "";
        internal static string SessionKey = "";
        internal static string SessionUser = "";

        internal static string ClientID = "qqbge6p9il5pu9u0t1gfy04ahdpohv";

        internal static bool IsTokenValid(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;
            Api api = new();
            api.Settings.ClientId = ClientID;
            api.Settings.AccessToken = token;
            try
            {
                int x = api.Helix.Users.GetUsersAsync(accessToken: token).Result.Users.Length;
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal static string GetTokenOwner(string token)
        {
            Api api = new();
            api.Settings.ClientId = ClientID;
            api.Settings.AccessToken = token;
            return api.Helix.Users.GetUsersAsync(accessToken: token).Result.Users[0].DisplayName;
        }

        internal static string GetMACAddress()
        {
            string macAddr =
            (
                from nic in NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == OperationalStatus.Up
                select nic.GetPhysicalAddress().ToString()
            ).FirstOrDefault();
            return macAddr;
        }

        internal static string GetToken()
        {
            string _token = Mod.tokenManager.GetPreference<PreferenceString>("ACCESS_TOKEN").Value;
            int decryptionMethod = Mod.manager.GetPreference<PreferenceInt>("TOKEN_STORAGE_METHOD").Value;
            if (decryptionMethod == 0)
                return Cryptography.Decrypt(_token, GetMACAddress());
            if (decryptionMethod == 1)
                return Cryptography.Decrypt(_token, DecryptionKey);
            return SessionKey;
        }

        internal static void SetToken(string token)
        {
            string _token = "";
            int decryptionMethod = Mod.manager.GetPreference<PreferenceInt>("TOKEN_STORAGE_METHOD").Value;

            if (decryptionMethod == 0)
            {
                _token = Cryptography.Encrypt(token, GetMACAddress());
                Mod.tokenManager.GetPreference<PreferenceString>("ACCESS_TOKEN").Set(_token);
                Mod.tokenManager.Save();
            }
            else if (decryptionMethod == 1)
            {
                _token = Cryptography.Encrypt(token, DecryptionKey);
                Mod.tokenManager.GetPreference<PreferenceString>("ACCESS_TOKEN").Set(_token);
                Mod.tokenManager.Save();
            }
            else if (decryptionMethod == 2)
            {
                Mod.tokenManager.GetPreference<PreferenceString>("ACCESS_TOKEN").Set("");
                Mod.tokenManager.Save();
                SessionKey = token;
            }
        }

        public static string GetUser()
        {
            string _user = Mod.tokenManager.GetPreference<PreferenceString>("USER_ID").Value;
            int decryptionMethod = Mod.manager.GetPreference<PreferenceInt>("TOKEN_STORAGE_METHOD").Value;

            if (decryptionMethod == 0)
                return Cryptography.Decrypt(_user, GetMACAddress());
            if (decryptionMethod == 1)
                return Cryptography.Decrypt(_user, DecryptionKey);
            if (decryptionMethod == 2) return SessionUser;
            return "";
        }

        internal static void SetUser(string user)
        {
            string _user;
            int decryptionMethod = Mod.manager.GetPreference<PreferenceInt>("TOKEN_STORAGE_METHOD").Value;

            if (decryptionMethod == 0)
            {
                _user = Cryptography.Encrypt(user, GetMACAddress());
                Mod.tokenManager.GetPreference<PreferenceString>("USER_ID").Set(_user);
                Mod.tokenManager.Save();
            }
            else if (decryptionMethod == 1)
            {
                _user = Cryptography.Encrypt(user, DecryptionKey);
                Mod.tokenManager.GetPreference<PreferenceString>("USER_ID").Set(_user);
                Mod.tokenManager.Save();
            }
            else if (decryptionMethod == 2)
            {
                Mod.tokenManager.GetPreference<PreferenceString>("USER_ID").Set("");
                Mod.tokenManager.Save();
                SessionUser = user;
            }
        }

        internal static string URLBuilder(string[] scopes = null)
        {
            string URL = "https://id.twitch.tv/oauth2/authorize?response_type=token&client_id=" + ClientID + "&redirect_uri=http://localhost:8080/redirect/&scope=";

            if (scopes != null)
            {
                string scope = string.Join("+", scopes).Replace(":", "%3A");
                URL += scope;
            }

            return URL;
        }
    }
}