using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GrandHotel_WebApplication.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //BuildWebHost(args).Run();
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            // Modifier le port selon les besoins
            client.BaseAddress = new Uri("https://localhost:44371/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                // Creation client
                Client cli = new Client()
                {
                    Civilite = "M",
                    Nom = "Durant",
                    Prenom = "Eric",
                    Email = "azerty@azert",
                    CarteFidelite = false
                };

                var url = await CreateClientAsync(cli);
                Console.WriteLine($"Client créé à l'url {url}");

                // Get the emp
                cli = await GetClientAsync(url.PathAndQuery);
                ShowClient(cli);

                //// Update the emp
                //Console.WriteLine("Mise à jour ...");
                //await UpdateClientAsync(cli);

                //// Get the updated emp
                //cli = await GetClientAsync(url.PathAndQuery);
                //ShowClient(cli);

                // Delete the emp
                var statusCode = await DeleteClientAsync(cli.Id);
                Console.WriteLine($"Client supprimé (statut HTTP = {(int)statusCode})");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }


        //Methode de Test
        #region MyRegion

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();

        static HttpClient client = new HttpClient();

        static void ShowClient (Client cli)
        {
            Console.WriteLine($"{cli.Civilite} {cli.Nom} {cli.Prenom}, Email : { cli.Email} \n Adresse { cli.Adresse.Rue} {cli.Adresse.CodePostal} {cli.Adresse.Ville} \n Telephone : {cli.Telephone[0].Numero}  ");
        }

        static async Task<Uri> CreateClientAsync(Client cli)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(
                "api/ClientsAPI", cli);
            response.EnsureSuccessStatusCode();

            // retourne l'uri de la ressource créée
            return response.Headers.Location;
        }

        static async Task<Client> GetClientAsync(string path)
        {
            Client cli = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                cli = await response.Content.ReadAsAsync<Client>();
            }
            return cli;
        }

        //static async Task<Client> UpdateEmployeeAsync(Client cli)
        //{
        //    HttpResponseMessage response = await client.PutAsJsonAsync(
        //        $"api/ClientsAPI/{cli.Id}", cli);
        //    response.EnsureSuccessStatusCode();

        //    // Deserialise l'employé mis à jour depuis le corps de la réponse
        //    cli = await response.Content.ReadAsAsync<Client>();
        //    return cli;
        //}

        static async Task<HttpStatusCode> DeleteClientAsync(int id)
        {
            HttpResponseMessage response = await client.DeleteAsync(
                $"api/ClientsAPI/{id}");
            return response.StatusCode;
        }
        #endregion

    }
}
