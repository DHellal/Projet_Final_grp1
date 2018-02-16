using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GrandHotel_WebApplication.Outil
{

    /// <summary>
    /// Modélise une page d'éléments
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageItems<T> : List<T>
    {
        // Indice de la page courante
        public int PageIndex { get; private set; }
        // Nombre total de pages
        public int TotalPages { get; private set; }

        /// <summary>
        /// Crée une page d'éléments à partir d'une liste
        /// </summary>
        /// <param name="items">Liste des éléments de la page</param>
        /// <param name="count">Nombre total d'éléments de la source dont est extraite la liste</param>
        /// <param name="pageIndex">Indice de la page</param>
        /// <param name="pageSize">Nombre d'éléments par page</param>
        public PageItems(List<T> items, int count, int pageIndex, int pageSize)
        {
            // Mémorise l'indice de la page crée
            PageIndex = pageIndex;

            // Calcule le nombre total de pages
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            // Ajoute les éléments de la page à la liste interne
            this.AddRange(items);
        }

        // Renvoie Vrai s'il existe une page avant la page courante
        public bool HasPreviousPage
        {
            get { return (PageIndex > 1); }
        }

        // Renvoie Vrai s'il existe une page après la page courante
        public bool HasNextPage
        {
            get { return (PageIndex < TotalPages); }
        }

        /// <summary>
        /// Renvoie une page d'éléments tirés d'une liste source
        /// </summary>
        /// <param name="source">liste d'éléments source</param>
        /// <param name="pageIndex">Indice de la page</param>
        /// <param name="pageSize">Taille de la page (nombre d'éléments)</param>
        /// <returns></returns>
        public static async Task<PageItems<T>> CreateAsync(IQueryable<T> source,
              int pageIndex, int pageSize)
        {
            // récupère le nombre d'éléments de la source
            var count = await source.CountAsync();

            // Extrait de la source un nombre d'éléments correpondant à pageSize
            // en commençant à partir de l'élément d'indice
            // (pageIndex - 1) * pageSize + 1
            var items = await source.Skip((pageIndex - 1) * pageSize)
                           .Take(pageSize).ToListAsync();

            // Crée et renvoie une page constituée de ces éléments
            return new PageItems<T>(items, count, pageIndex, pageSize);
        }
    }
}



