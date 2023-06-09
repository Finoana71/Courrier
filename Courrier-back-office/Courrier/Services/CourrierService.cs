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
            CourrierDestinataire courrierDestinataire = GetCourrierById(idCourrierDestinataire);            
            if (courrierDestinataire != null)
            {
                // Mettre à jour l'ID du coursier et le statut du courrier destinataire
                courrierDestinataire.IdCoursier = idCoursier;
                courrierDestinataire.IdStatut = 2;
                insererHistorique(idCourrierDestinataire, 2);
                _dbContext.SaveChanges();
            }
        }

        public void insererHistorique(int idCourrierDestinataire, int idStatut)
        {
            // Insérer un nouvel historique de courrier destinataire
            HistoriqueCourrierDestinataire historique = new HistoriqueCourrierDestinataire
            {
                IdCourrierDestinataire = idCourrierDestinataire,
                IdStatut = idStatut,
                DateHistorique = DateTime.UtcNow
            };
            _dbContext.HistoriqueCourrierDestinataire.Add(historique);

        }
        public void TransfererSecretaire(int idCourrierDestinataire)
        {
            // Récupérer le courrier destinataire par son ID
            CourrierDestinataire courrierDestinataire = GetCourrierById(idCourrierDestinataire);
            if (courrierDestinataire != null)
            {
                courrierDestinataire.IdStatut = 3;
                insererHistorique(idCourrierDestinataire, 3);
                _dbContext.SaveChanges();
            }
        }

        public void TransfererDirecteur(int idCourrierDestinataire)
        {
            // Récupérer le courrier destinataire par son ID
            CourrierDestinataire courrierDestinataire = GetCourrierById(idCourrierDestinataire);
            if (courrierDestinataire != null)
            {
                courrierDestinataire.IdStatut = 4;
                insererHistorique(idCourrierDestinataire, 4);
                _dbContext.SaveChanges();
            }
        }
        public IQueryable<CourrierDestinataire> GetFilteredCourriers(User user, int? flag, int? statut, string expediteur, int? departement, string reference)
        {
            IQueryable<CourrierDestinataire> query = FilterCourriersByUserRole(user);
            query = ApplyFlagFilter(query, flag);
            query = ApplyStatutFilter(query, statut);
            query = ApplyExpediteurFilter(query, expediteur);
            query = ApplyDepartementFilter(query, departement);
            query = ApplyReferenceFilter(query, reference);
            return query;
        }

        public Pages<CourrierDestinataire> GetCourriersByCriteria(User user, int page, int pageSize, int? flag, int? statut, string expediteur, int? departement, string reference)
        {
            IQueryable<CourrierDestinataire> query = GetFilteredCourriers(user, flag, statut, expediteur, departement, reference);

            // Pagination
            int totalCount = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            query = query.Skip((page - 1) * pageSize).Take(pageSize);
            query = IncludeRelatedEntities(query);

            List<CourrierDestinataire> CourriersDestinataires = query.ToList();

            return new Pages<CourrierDestinataire>(CourriersDestinataires, page, pageSize, totalCount, totalPages);
        }

        public List<CourrierDestinataire> GetCourriersByCriteriaSansPage(User user, int? flag, int? statut, string expediteur, int? departement, string reference)
        {
            IQueryable<CourrierDestinataire> query = GetFilteredCourriers(user, flag, statut, expediteur, departement, reference);

            query = IncludeRelatedEntities(query);

            List<CourrierDestinataire> CourriersDestinataires = query.ToList();

            return CourriersDestinataires;
        }

        private IQueryable<CourrierDestinataire> ApplyFlagFilter(IQueryable<CourrierDestinataire> query, int? flag)
        {
            if (flag != null)
            {
                return query.Where(d => d.Courrier.Flag.Id == flag);
            }
            return query;
        }

        private IQueryable<CourrierDestinataire> ApplyStatutFilter(IQueryable<CourrierDestinataire> query, int? statut)
        {
            if (statut != null)
            {
                return query.Where(d => d.Statut.Id == statut);
            }
            return query;
        }

        private IQueryable<CourrierDestinataire> ApplyExpediteurFilter(IQueryable<CourrierDestinataire> query, string expediteur)
        {
            if (!string.IsNullOrEmpty(expediteur))
            {
                return query.Where(d => d.Courrier.Expediteur.Contains(expediteur));
            }
            return query;
        }

        private IQueryable<CourrierDestinataire> ApplyDepartementFilter(IQueryable<CourrierDestinataire> query, int? departement)
        {
            if (departement != null)
            {
                return query.Where(d => d.Departement.Id == departement);
            }
            return query;
        }

        private IQueryable<CourrierDestinataire> ApplyReferenceFilter(IQueryable<CourrierDestinataire> query, string reference)
        {
            if (!string.IsNullOrEmpty(reference))
            {
                return query.Where(d => d.Courrier.Reference.Contains(reference));
            }
            return query;
        }

        public Dictionary<string, int> GetStatCourrierFlag()
        {
            var flags = _dbContext.Flags.Select(f => f.Libelle).ToList();
            var courriersByFlag = _dbContext.Courriers
                .GroupBy(c => c.Flag.Libelle)
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var flag in flags)
            {
                if (!courriersByFlag.ContainsKey(flag))
                    courriersByFlag[flag] = 0;
            }
            return courriersByFlag;
        }

        public Dictionary<string, int> GetStatCourrierStatut()
        {
            var statuts = _dbContext.Statuts.Select(s => s.Libelle).ToList();
            var courriersByStatut = _dbContext.CourriersDestinataires
                .GroupBy(d => d.Statut.Libelle)
                .ToDictionary(g => g.Key, g => g.Count());
            foreach (var statut in statuts)
            {
                if (!courriersByStatut.ContainsKey(statut))
                    courriersByStatut[statut] = 0;
            }
            return courriersByStatut;
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
