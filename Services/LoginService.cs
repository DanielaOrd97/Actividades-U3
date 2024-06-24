using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using U3Cliente.Models;

namespace U3Cliente.Services
{
    public class LoginService
    {
        string url = App.Url;
        HttpClient client;
        public LoginService()
        {
            client = new HttpClient()
            {
                BaseAddress = new Uri(url)

            };
        }

        public async Task<string> Login(LoginDTO login)
        {
            var request = await client.PostAsJsonAsync("login", login);
            if (request.IsSuccessStatusCode)
            {
                var json = await request.Content.ReadAsStringAsync();
                return json;
            }
            else
            {
                string error = await request.Content.ReadAsStringAsync();
                return error;
            }
        }
    }
}
