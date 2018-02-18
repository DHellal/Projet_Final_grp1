using GrandHotel_WebApplication.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using GrandHotel_WebApplication;
using System.Collections.Generic;

namespace ConsoleTestAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

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
                    Email = "azersuyty@azttyerty",
                    CarteFidelite = false
                };

                var reponse = await CreateClientAsync(cli);
                Console.WriteLine($"{reponse}");

                // Get Client selon id
                string id = Console.ReadLine();

                cli = await GetClientAsync(id);
                ShowClient(cli);

                Console.Read();
                //// Update the emp
                //Console.WriteLine("Mise à jour ...");
                //await UpdateClientAsync(cli);

                //// Get the updated emp
                //cli = await GetClientAsync(url.PathAndQuery);
                //ShowClient(cli);

                // Delete the emp
                var statusCode = await DeleteClientAsync(id);
                Console.WriteLine($"Client supprimé (statut HTTP = {(int)statusCode})");
                Console.Read();

                string Nom = Console.ReadLine();
                var clis = await GetClientNomAsync(Nom);
                foreach(var c in clis)
                ShowClient(cli);

                Console.Read();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }

        //Methodes de Test
        #region MyRegion

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();

        static HttpClient client = new HttpClient();

        static void ShowClient(Client cli)
        {
            if(cli.Adresse == null)
            {
                cli.Adresse = new Adresse();
                cli.Adresse.CodePostal = "Non resigné";
                cli.Adresse.Rue = "";
                cli.Adresse.Ville = "";
            }
            if (cli.Telephone == null)
            {
                cli.Telephone = new List<Telephone>();
                cli.Telephone.Add(new Telephone());
                cli.Telephone[0].Numero = "Non renseigné";
            }
                

                Console.WriteLine($"{cli.Civilite} {cli.Nom} {cli.Prenom}, Email : { cli.Email} \n Adresse { cli.Adresse.Rue} {cli.Adresse.CodePostal} {cli.Adresse.Ville} \n Telephone : {cli.Telephone[0].Numero}  ");
        }

        //Post nouveau client
        static async Task<string> CreateClientAsync(Client cli)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(
                "api/ClientsAPI", cli);
            response.EnsureSuccessStatusCode();

            

            // retourne l'uri de la ressource créée
            return response.Content.ToString();
        }
        //Get Client selon id
        static async Task<Client> GetClientAsync(string id)
        {
            Client cli = null;
            HttpResponseMessage response = await client.GetAsync("api/ClientsAPI/" + id);
            if (response.IsSuccessStatusCode)
            {
                cli = await response.Content.ReadAsAsync<Client>();
                ShowClient(cli);
            }
            return cli;
        }


        //Get Client selon nom
        static async Task<List<Client>> GetClientNomAsync(string Nom)
        {
            List<Client> cli = null;
            HttpResponseMessage response = await client.GetAsync("api/ClientsAPI/FiltreNom" + Nom);
            if (response.IsSuccessStatusCode)
            {
                cli = await response.Content.ReadAsAsync<List<Client>>();

                foreach (var c in cli)
                {
                    ShowClient(c);
                }
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

        static async Task<HttpStatusCode> DeleteClientAsync(string id)
        {
            HttpResponseMessage response = await client.DeleteAsync(
                $"api/ClientsAPI/{id}");
            return response.StatusCode;
        }
        #endregion
    }
}
