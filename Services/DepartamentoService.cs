using Newtonsoft.Json;
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
    public class DepartamentoService
    {
        string url = App.Url;
        HttpClient client;

        public DepartamentoService()
        {
            client = new HttpClient()
            {
                BaseAddress = new Uri(url)
            };
        }

        public async Task<IEnumerable<DepartamentosDTO>> GetDeptos(int id)
        {
            IEnumerable<DepartamentosDTO>? deptos = Enumerable.Empty<DepartamentosDTO>(); ;
            var request = await client.GetAsync("departamento/" + id);

            if (request.IsSuccessStatusCode)
            {
                var json = await request.Content.ReadAsStringAsync();
                deptos = JsonConvert.DeserializeObject<IEnumerable<DepartamentosDTO>>(json);
            }

            if (deptos == null)
                deptos = Enumerable.Empty<DepartamentosDTO>();
            return deptos;
        }

        public async Task<string> Post(DepartamentosDTO departamento)
        {
            var request = await client.PostAsJsonAsync("departamento", departamento);
            if (request.IsSuccessStatusCode)
                return "";
            else
            {
                var message = await request.Content.ReadAsStringAsync();
                return message;
            }
        }

        public async Task<string> Put(DepartamentosDTO departamento)
        {
            var request = await client.PutAsJsonAsync("departamento", departamento);
            if (request.IsSuccessStatusCode)
                return "";
            else
            {
                var message = await request.Content.ReadAsStringAsync();
                return message;
            }
        }

        public async Task<string> Delete(int idDepto)
        {
            var request = await client.DeleteAsync("departamento/" + idDepto);
            if (request.IsSuccessStatusCode)
            {
                return "";
            }
            else
            {
                var message = await request.Content.ReadAsStringAsync();
                return message;
            }
        }
    }
}
