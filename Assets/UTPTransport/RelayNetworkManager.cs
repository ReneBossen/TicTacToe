using Mirror;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Utp
{
    public class RelayNetworkManager : NetworkManager
    {
        private UtpTransport utpTransport;

        /// <summary>
        /// Server's join code if using Relay.
        /// </summary>
        public string relayJoinCode = "";

        public override void Awake()
        {
            base.Awake();

            utpTransport = GetComponent<UtpTransport>();

            string[] args = System.Environment.GetCommandLineArgs();
            for (int key = 0; key < args.Length; key++)
            {
                if (args[key] == "-port")
                {
                    if (key + 1 < args.Length)
                    {
                        string value = args[key + 1];

                        try
                        {
                            utpTransport.Port = ushort.Parse(value);
                        }
                        catch
                        {
                            UtpLog.Warning($"Unable to parse {value} into transport Port");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get the port the server is listening on.
        /// </summary>
        /// <returns>The port.</returns>
        public ushort GetPort()
        {
            return utpTransport.Port;
        }

        /// <summary>
        /// Get whether Relay is enabled or not.
        /// </summary>
        /// <returns>True if enabled, false otherwise.</returns>
        public bool IsRelayEnabled()
        {
            return utpTransport.useRelay;
        }

        /// <summary>
        /// Ensures Relay is disabled. Starts the server, listening for incoming connections.
        /// </summary>
        public void StartStandardServer()
        {
            utpTransport.useRelay = false;
            StartServer();
        }

        /// <summary>
        /// Ensures Relay is disabled. Starts a network "host" - a server and client in the same application
        /// </summary>
        public void StartStandardHost()
        {
            utpTransport.useRelay = false;
            StartHost();
        }

        /// <summary>
        /// Gets available Relay regions.
        /// </summary>
        /// 
        public void GetRelayRegions(Action<List<Region>> onSuccess, Action onFailure)
        {
            utpTransport.GetRelayRegions(onSuccess, onFailure);
        }

        /// <summary>
        /// Ensures Relay is enabled. Starts a network "host" - a server and client in the same application
        /// </summary>
        public Task StartRelayHost(int maxPlayers, string regionId = null)
        {
            TaskCompletionSource<bool> tcs = new();

            utpTransport.useRelay = true;
            utpTransport.AllocateRelayServer(maxPlayers, regionId,
            (string joinCode) =>
            {
                relayJoinCode = joinCode;

                StartHost();
                tcs.SetResult(true);
            },
            () =>
            {
                UtpLog.Error($"Failed to start a Relay host.");
                tcs.SetException(new Exception("Failed to start a Relay host"));
            });

            return tcs.Task;
        }

        /// <summary>
        /// Ensures Relay is disabled. Starts the client, connects it to the server with networkAddress.
        /// </summary>
        public void JoinStandardServer()
        {
            utpTransport.useRelay = false;
            StartClient();
        }

        /// <summary>
        /// Ensures Relay is enabled. Starts the client, connects to the server with the relayJoinCode.
        /// </summary>
        public void JoinRelayServer()
        {
            utpTransport.useRelay = true;
            utpTransport.ConfigureClientWithJoinCode(relayJoinCode,
            () =>
            {
                StartClient();
            },
            () =>
            {
                UtpLog.Error($"Failed to join Relay server.");
            });
        }
    }
}