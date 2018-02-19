using WebApi.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;

namespace ConsoleTestAPI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Grand Hotel API!");

            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            // Modifier le port selon les besoins
            client.BaseAddress = new Uri("https://localhost:44371/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));



            bool encore = true;
            do
            {
            Console.WriteLine("Veuillez choisir l'action à effectuer :");
            Console.WriteLine("Tapez 1 pour créer un client");
            Console.WriteLine("Tapez 2 pour chercher un client selon l'ID");
            Console.WriteLine("Tapez 3 pour chercher un client selon les lettres de son nom");
            Console.WriteLine("Tapez 4 pour retirer un client");
            Console.WriteLine("Tapez 5 pour afficher la liste des clients");
            Console.WriteLine("Tapez 6 pour sortir");

            string reponse = Console.ReadLine();

                switch (reponse)
                {
                    // Nouveau client
                    case "1":

                        Client cli = new Client()
                        {
                            Civilite = "M",
                            Nom = "Durant",
                            Prenom = "Eric",
                            Email = "azersuyty@azttyerty",
                            CarteFidelite = false
                        };

                        try
                        {
                        var statut = await CreateClientAsync(cli);
                            Console.WriteLine($"{statut}");
                        }
                        catch(Exception e)
                        {
                        Console.WriteLine($"{e}");
                        }

                        break;

                    // Get Client selon id
                    case "2":
                        Console.WriteLine("Veullez entrer l'id du client");
                        string id = Console.ReadLine();

                        cli = await GetClientAsync(id);
                        if(cli != null)
                        ShowClient(cli);
                        else
                            Console.WriteLine("Client introuvable");
                        break;

                    // Get Client selon Nom
                    case "3":
                        Console.WriteLine("Veuillez entrer 3 lettres au minimum");
                        string Nom = Console.ReadLine();
                        var clis = await GetClientNomAsync(Nom);
                        if (clis != null)
                        {
                        foreach (var c in clis)
                            ShowClient(c);
                        }
                        break;

                        //Delete client
                    case "4":
                        Console.WriteLine("Veullez entrer l'id du client");
                        string idDelete = Console.ReadLine();
                        var statusCode = await DeleteClientAsync(idDelete);
                        Console.WriteLine($"Client supprimé (statut HTTP = {(int)statusCode})");
                        Console.Read();

                        break;
                    // Get liste client
                    case "5":
                        

                        var cliList = await GetClientListAsync();
                        if (cliList != null)
                        {
                            foreach (var c in cliList)
                                ShowClient(c);
                        }
                        else
                            Console.WriteLine("Client introuvable");
                        break;

                    case "6":

                        encore = false;
                        break;
                }
            } while (encore);
           
               
                //// Update the emp
                //Console.WriteLine("Mise à jour ...");
                //await UpdateClientAsync(cli);
                //// Get the updated emp
                //cli = await GetClientAsync(url.PathAndQuery);
                //ShowClient(cli);

                Console.Read();
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
                

                Console.WriteLine($"\n {cli.Id} {cli.Civilite} {cli.Nom} {cli.Prenom}, Email : { cli.Email} \n Adresse { cli.Adresse.Rue} {cli.Adresse.CodePostal} {cli.Adresse.Ville} \n Telephone : {cli.Telephone[0].Numero}  \n");
        }

        //Post nouveau client
        static async Task<string> CreateClientAsync(Client cli)
        {

            HttpResponseMessage response = await client.PostAsJsonAsync(
                "api/ClientsAPI", cli);

            //response.EnsureSuccessStatusCode();

            // retourne l'uri de la ressource créée
            return response.StatusCode.ToString();
        }
        //Get Client selon id
        static async Task<Client> GetClientAsync(string id)
        {
            Client cli = null;
            HttpResponseMessage response = await client.GetAsync("api/ClientsAPI/" + id);
            if (response.IsSuccessStatusCode)
            {
                cli = await response.Content.ReadAsAsync<Client>();
            }
            return cli;
        }

        //Get Client selon id
        static async Task<List<Client>> GetClientListAsync()
        {
            List<Client> cli = null;
            HttpResponseMessage response = await client.GetAsync("api/ClientsAPI/");
            if (response.IsSuccessStatusCode)
            {
                cli = await response.Content.ReadAsAsync<List<Client>>();

            }
            return cli;
        }



        //Get Client selon nom
        static async Task<List<Client>> GetClientNomAsync(string Nom)
        {
            List<Client> cli = null;
            HttpResponseMessage response = await client.GetAsync("api/ClientsAPI/FiltreNom/" + Nom);
            if (response.IsSuccessStatusCode)
            {
                cli = await response.Content.ReadAsAsync<List<Client>>();

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
