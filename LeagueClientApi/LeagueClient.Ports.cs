using LeagueClientApi.Exceptions;
using ProcessUtils32;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace LeagueClientApi
{
    public partial class LeagueClient
    {
        /// <summary>
        /// Checks if we can connect to the specified port and get a valid response
        /// </summary>
        /// <param name="leagueClientPort">The port to be tested</param>
        /// <returns>Returns whether the port seems to be valid or not</returns>
        private bool CheckForRunningWebservice(uint leagueClientPort)
        {
            // Create a basic request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format("https://127.0.0.1:{0}/", leagueClientPort));

            try
            {
                // Send request
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                // If no exceptions occured, we were able to connect to the specified port and it seems that this port is a valid one
                return true;
            }
            catch (WebException ex)
            {
                // If a WebException is thrown and the StatusCode is not ConnectFailure, we were able to connect to the specified port and it seems that this port is a valid one
                return ex.Status != WebExceptionStatus.ConnectFailure;
            }
            // do nothing if a different exception is thrown...
            catch { }

            // ... and return false
            return false;
        }

        /// <summary>
        /// Returns the first valid port the LeagueClient is using
        /// </summary>
        /// <param name="leagueClientPort">The variable which will hold the valid port if a port was found</param>
        /// <returns>Returns whether a valid port could be found</returns>
        private bool FindLocalHostPort(out uint leagueClientPort)
        {
            // Set the port default to 0
            leagueClientPort = uint.MinValue;

            Dictionary<int, List<uint>> processPorts;

            // retrieve all (from all processes) activ local connections
            processPorts = GetProcessPortList();

            // Return false, if for the LeagueClient no ports were found
            if (processPorts.ContainsKey(process.Id) == false)
            {
                return false;
            }
            else
            {
                // Check for each found port if it's a valid one
                foreach (uint potentialPort in processPorts[process.Id])
                {
                    if (CheckForRunningWebservice(potentialPort))
                    {
                        leagueClientPort = potentialPort;
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Retrieves a list of all active ports with it's associated process on this machine
        /// More info: https://gist.github.com/cheynewallace/5971686
        /// </summary>
        /// <returns>Returns a Dictionary with all found processes and it's ports</returns>
        private Dictionary<int, List<uint>> GetProcessPortList()
        {
            Dictionary<int, List<uint>> processPorts = new Dictionary<int, List<uint>>();

            // Retrieves a list of all active ports on this machine
            if (ProcessManager.GetCommandOutput("netstat.exe", out string standardOutput, out string standardError, "-n", "-o") != 0)
            {
                // throw LeagueClientBase exception, if an error occurred (StatusCode not equal 0)
                throw new LeagueClientBaseException(string.Format(
                    "An error occurred while fetching local active connections:{2}{0}{2}{1}",
                    standardOutput,
                    standardError,
                    Environment.NewLine));
            }
            else
            {
                // parse the netstat result and fill the dictionary (each port only once)
                string[] rows = Regex.Split(standardOutput, "\r\n");
                foreach (string row in rows)
                {
                    string[] tokens = Regex.Split(row, "\\s+");
                    if (tokens.Length > 4 && (tokens[1].Equals("UDP") || tokens[1].Equals("TCP")))
                    {
                        string localAddress = Regex.Replace(tokens[2], @"\[(.*?)\]", "1.1.1.1");

                        int pid = tokens[1] == "UDP" ? int.Parse(tokens[4]) : int.Parse(tokens[5]);
                        uint port = uint.Parse(localAddress.Split(':')[1]);

                        if (processPorts.ContainsKey(pid) == false)
                            processPorts.Add(pid, new List<uint> { port });
                        else if (processPorts[pid].Contains(port) == false)
                            processPorts[pid].Add(port);
                    }
                }
            }

            return processPorts;
        }
    }
}
