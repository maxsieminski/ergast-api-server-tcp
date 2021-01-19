using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace TCP_Server_Asynchronous
{
    class AsyncServer : Server
    {

        #region Fields

        #endregion

        #region Constructors

        public AsyncServer(IPAddress address, int port) : base(address, port) { }

        #endregion

        #region Functions

        /// <summary>
        /// Initializes asynchronous TCP Server
        /// </summary>
        public override void Start()
        {
            StartListening();
            AcceptClient();
        }

        /// <summary>
        /// Listens for incoming clients
        /// </summary>
        protected override void AcceptClient()
        {
            while (true)
            {
                TcpClient tcpClient = TcpListener.AcceptTcpClient();

                Stream = tcpClient.GetStream();

                TransmissionDataDelegate transmissionDelegate = new TransmissionDataDelegate(BeginDataTransmission);

                transmissionDelegate.BeginInvoke(Stream, TransmissionCallback, tcpClient);
            }
        }

        private void TransmissionCallback(IAsyncResult asyncResult)
        {
            TcpClient = (TcpClient)asyncResult.AsyncState;
            TcpClient.Close();
        }


        /// <summary>
        /// Takes incoming messages and returns answer
        /// </summary>
        /// <param name="stream">Client stream</param>
        protected override void BeginDataTransmission(NetworkStream stream)
        {
            string currentUser = null;

            bool authenticated = false;

            char[] separators = new char[]{' ', '\0'};
            byte[] buffer = new byte[Buffer_size];
            byte[] serverResponseBuffer = new byte[Buffer_size];

            string firstMessage = "Enter command : ";

            while(true)
            {
                try
                {
                    if (!authenticated) {
                        string loginFirstMsg = "Enter one of the following\nlogin [login] [password]\nregister [login] [password]\nregister admin [login] [password] [key]: ";
                        stream.Write(System.Text.Encoding.ASCII.GetBytes(loginFirstMsg), 0, loginFirstMsg.Length);

                        stream.Read(buffer, 0, Buffer_size);
                        string[] login  = System.Text.Encoding.ASCII.GetString(buffer).Split(separators, StringSplitOptions.None);
                        
                        if(login[0] == "login") {
                            if (Authentication.AuthenticateUser(login[1], login[2]) == 'y') 
                            {
                                authenticated = true;
                                currentUser = login[1];
                                Console.WriteLine("Logged in!");
                            }
                        }
                        else if(login[0] == "register" && login[1] != "admin") {
                            Authentication.CreateUser(login[1], login[2], false);
                        }
                        else if(login[0] == "register" && login[1] == "admin")
                        {
                            if (login[4] == "kiskes")
                            {
                                Authentication.CreateUser(login[2], login[3], true);
                            }
                            else
                            {
                                string wrong_key_message = "Wrong masterkey entered. Try again.\n";
                                stream.Write(System.Text.Encoding.ASCII.GetBytes(wrong_key_message), 0, wrong_key_message.Length);
                            }
                        }
                    }
                    else 
                    {
                        stream.Write(System.Text.Encoding.ASCII.GetBytes(firstMessage), 0, firstMessage.Length);
                        stream.Read(buffer, 0, Buffer_size);
                        string[] message = System.Text.Encoding.ASCII.GetString(buffer).Split(separators, StringSplitOptions.None).Select(s => s.ToLowerInvariant()).ToArray();      

                        foreach(string s in message) {
                            if (s.Contains((char)13)) {
                                message = null;
                            }
                        }

                        HistoryHandling.getHistory(currentUser);

                        if(message != null) 
                        {
                            Console.WriteLine(message[0]);
                            if (message[0].Substring(0, 10) == "printusers")
                            {
                                string response = Authentication.PrintUsers(currentUser);
                                stream.Write(System.Text.Encoding.ASCII.GetBytes(response), 0, response.Length);
                            }
                            else if (message[0].Substring(0, 10) == "deleteuser")
                            {
                                string somemess = "Enter username of who you want to delete big man.";
                                stream.Write(System.Text.Encoding.ASCII.GetBytes(somemess), 0, somemess.Length);
                                stream.Read(buffer, 0, Buffer_size); // sorry ja wiem ze to jest obrzydliwy kod, troche idc musze spac a to działa wyzej i nie pluje tych losowych charów jak message
                                string[] someuser = System.Text.Encoding.ASCII.GetString(buffer).Split(separators, StringSplitOptions.None);
                                string delresponse = Authentication.DeleteUser(someuser[0], currentUser);
                                stream.Write(System.Text.Encoding.ASCII.GetBytes(delresponse), 0, delresponse.Length);

                            }

                            else if (message.Length == 1)
                            {
                                serverResponseBuffer = System.Text.Encoding.ASCII.GetBytes(ConnectionHandler.GetRequest(message[0], null, currentUser));
                            }
                            else 
                            {
                                string[] arg = message.Skip(1).Take(message.Length).ToArray();

                                for (int i = 0; i < arg.Length; i++)
                                {
                                    arg[i] = arg[i].Replace("\0", string.Empty);
                                }
                                
                                serverResponseBuffer = System.Text.Encoding.ASCII.GetBytes(ConnectionHandler.GetRequest(message[0], arg, currentUser));
                            }
                            stream.Write(serverResponseBuffer, 0, serverResponseBuffer.Length);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    System.Environment.Exit(1);
                }
            }
        }

        public delegate void TransmissionDataDelegate(NetworkStream stream);

        #endregion
    }
}