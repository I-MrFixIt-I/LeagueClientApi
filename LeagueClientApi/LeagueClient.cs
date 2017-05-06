using LeagueClientApi.Exceptions;
using ProcessUtils32;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

namespace LeagueClientApi
{
    public partial class LeagueClient
    {
        private Process process;
        private uint leagueClientPort;

        public event EventHandler Exited;

        public LeagueClient(bool DoInitialSetup)
        {
            // If desired, initialize the class
            if (DoInitialSetup)
            {
                // An error occurred while initializing, so raise exception
                if (Setup() == false)
                {
                    throw new LeagueClientBaseException(string.Format("An error occurred while initializing {0} class", GetType().Name));
                }
                else
                {
                    // enable events and register for Exited event, so that we can inform the user that the process has been shutdown
                    process.EnableRaisingEvents = true;
                    process.Exited += Process_Exited;
                }
            }
        }

        public bool Setup()
        {
            // Retrieve LeagueClient.exe process
            Process process = Process.GetProcessesByName("LeagueClient").FirstOrDefault();

            // return false, if not found
            if (process == null)
                return false;

            this.process = process;

            // Retrieve the local port the LeagueClient runs on
            // Returns false, if an error occurred
            if (FindLocalHostPort(out uint leagueClientPort) == false)
                return false;

            this.leagueClientPort = leagueClientPort;

            // During setup no errors occurred, so return true
            return true;
        }

        /// <summary>
        /// Check if the current session is logged in
        /// </summary>
        /// <returns>Returns whether the current session is logged in into LoL-Server</returns>
        public bool IsAuthorized()
        {
            HttpWebRequest request = CreateRequest("lol-login/v1/session", "GET");
            HttpWebResponse response = SubmitRequest(request, out string content);

            return response.StatusCode == HttpStatusCode.OK;
        }

        /// <summary>
        /// Returns the loginname for the Authenticate Header (currently, this is always "riot")
        /// </summary>
        /// <returns>Returns the loginname</returns>
        private string GetLoginUser()
        {
            return "riot";
        }

        /// <summary>
        /// Returns the password for the Authenticate Header
        /// </summary>
        /// <returns>Returns the password</returns>
        private string GetLoginPassword()
        {
            // Retrieves the first ThreadStack, which holds the password for the Authenticate Header
            uint threadStack0 = CheatengineSpecific.GetThreadStack0(process);

            // Open the LeagueClient process for reading
            IntPtr hProcess = InfoReader.OpenProcess(InfoReader.ProcessAccessFlags.VirtualMemoryRead, false, process.Id);

            // Retrieve the pointer to the password (hopefully the Offsets stays with bigger patches)
            IntPtr PtrToLoginPassword = MemReader.ReadToType<IntPtr>(hProcess, threadStack0 - 0xCC4);
            // Read the password from the pointer (22 chars)
            string loginPassword = Encoding.UTF8.GetString(MemReader.ReadToByteArray(hProcess, (uint)PtrToLoginPassword, 22));

            // Close the opened handle
            InfoReader.CloseHandle(hProcess);

            // return authenticate password
            return loginPassword;
        }

        /// <summary>
        /// Retrieves the encoded Authenticate Header, which is needed for every request to the LeagueClient 
        /// </summary>
        /// <returns>Returns the encoded Authenticate Header</returns>
        private string GetAuthHeaderValue()
        {
            string user = GetLoginUser();
            string password = GetLoginPassword();
            string encodedCredentials = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes(user + ":" + password));

            return "Basic " + encodedCredentials;
        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Exited.Invoke(this, e);
        }
    }
}
