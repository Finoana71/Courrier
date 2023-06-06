using Courrier.DAL;
using Courrier.Models;
using Courrier.Models.courrier;
using Courrier.Models.Courrier;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Courrier.Services
{
    public class CourrierService
    {
        private readonly AppDbContext _dbContext;
        private readonly UploadService _uploadService;

        public CourrierService(AppDbContext context, UploadService uploadService)
        {
            _dbContext = context;
            _uploadService = uploadService;
        }

        public CourrierModel Creer(CourrierModel courrier, User user, List<String> selectedDepartements, IFormFile file)
        {
            courrier.Receptionniste = user;
            courrier.Fichier = _uploadService.UploadFile(file);
            _dbContext.Courriers.Add(courrier);
            List<CourrierDestinataire> destinataires = selectedDepartements
                .Select(departement => new CourrierDestinataire { IdDepartement = int.Parse(departement), IdStatut = 1 })
                .ToList();
            foreach (CourrierDestinataire dest in destinataires)
            {
                dest.Historiques = new Collection<HistoriqueCourrierDestinataire>();
                dest.Historiques.Add(new HistoriqueCourrierDestinataire { IdStatut = 1 });
            }
            courrier.Destinataires = destinataires;
            foreach (CourrierDestinataire dest in destinataires)
            {
                dest.Historiques = new Collection<HistoriqueCourrierDestinataire>();
                dest.Historiques.Add(new HistoriqueCourrierDestinataire { IdStatut = 1, CourrierDestinaire = dest });
            }

            return courrier;
        }

        public Pages<CourrierDestinataire> GetCourriersPageByUser(User user, int page, int pageSize)
        {
            IQueryable<CourrierDestinataire> query = FilterCourriersByUserRole(user);

            // Pagination
            int totalCount = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            query = query.Skip((page - 1) * pageSize).Take(pageSize);
            query = IncludeRelatedEntities(query);

            List<CourrierDestinataire> CourriersDestinataires = query.ToList();

            return new Pages<CourrierDestinataire>(CourriersDestinataires, page, pageSize, totalCount, totalPages);
        }

        public List<CourrierDestinataire> GetCourriersByUser(User user, int page, int pageSize)
        {
            IQueryable<CourrierDestinataire> query = FilterCourriersByUserRole(user);

            // Includes
            query = IncludeRelatedEntities(query);

            // Pagination
            query = query.Skip((page - 1) * pageSize).Take(pageSize);

            return query.ToList();
        }

        private IQueryable<CourrierDestinataire> FilterCourriersByUserRole(User user)
        {
            int userId = user.Id;

            switch (user.IdRole)
            {
                case 3:
                    return _dbContext.CourriersDestinataires.Where(d => d.Courrier.IdReceptionniste == userId);
                case 4:
                    return _dbContext.CourriersDestinataires.Where(d => d.IdCoursier == userId);
                case 2:
                    return _dbContext.CourriersDestinataires.Where(d =>
                        d.IdDepartement == user.IdDepartement &&
                        d.IdStatut == 3
                    );
                case 1:
                    return _dbContext.CourriersDestinataires.Where(d =>
                        d.IdDepartement == user.IdDepartement &&
                        d.IdStatut == 4
                    );
                default:
                    return Enumerable.Empty<CourrierDestinataire>().AsQueryable();
            }
        }

        private IQueryable<CourrierDestinataire> IncludeRelatedEntities(IQueryable<CourrierDestinataire> query)
        {
            return query.Include(d => d.Courrier)
                .Include(d => d.Courrier)
                    .ThenInclude(d => d.Flag)
                .Include(d => d.Departement)
                .Include(d => d.Coursier)
                .Include(d => d.Statut);
        }

        public CourrierDestinataire GetCourrierById(int courrierId)
        {
            return _dbContext.CourriersDestinataires.
                Include(d => d.Courrier)
                .Include(d => d.Courrier)
                    .ThenInclude(d => d.Flag)
                .Include(d => d.Departement)
                .Include(d => d.Coursier)
                .Include(d => d.Statut)
                .FirstOrDefault(c => c.Id == courrierId);
        }

        public void TransfererCoursier(int idCourrierDestinataire, int idCoursier)
        {
            // Récupérer le courrier destinataire par son ID
            CourrierDestinataire courrierDestinataire = _dbContext.CourriersDestinataires
                .Include(cd => cd.Courrier)
                .Include(cd => cd.Statut)
                .FirstOrDefault(cd => cd.Id == idCourrierDestinataire);

            if (courrierDestinataire != null)
            {
                // Mettre à jour l'ID du coursier et le statut du courrier destinataire
                courrierDestinataire.IdCoursier = idCoursier;
                courrierDestinataire.IdStatut = 2;

                // Insérer un nouvel historique de courrier destinataire
                HistoriqueCourrierDestinataire historique = new HistoriqueCourrierDestinataire
                {
                    IdCourrierDestinataire = idCourrierDestinataire,
                    IdStatut = 2,
                    DateHistorique = DateTime.UtcNow
                };

                _dbContext.HistoriqueCourrierDestinataire.Add(historique);
                _dbContext.SaveChanges();
            }
        }
    }

    public class Pages<T>
    {
        public List<T> Items { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

        public Pages(List<T> items, int pageNumber, int pageSize, int totalCount, int totalPages)
        {
            Items = items;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = totalPages;
        }
    }
}
