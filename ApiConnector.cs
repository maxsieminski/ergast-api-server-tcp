using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ErgastApi.Client;
using ErgastApi.Ids;
using ErgastApi.Requests;
using ErgastApi.Responses;

namespace TCP_Server_Asynchronous
{
    abstract class ApiConnector
    {
        private static ErgastClient client;
        private static ErgastResponse response;
        private static ErgastRequest<ErgastResponse> request;

        public static async Task GetRequest(string category, string[] args)
        {
            
        }
    }
}
