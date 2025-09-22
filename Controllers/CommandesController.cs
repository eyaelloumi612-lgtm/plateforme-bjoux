using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JewelryManagementPlatform.Data;
using JewelryManagementPlatform.Models;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JewelryManagementPlatform.Controllers
{
    [Authorize]
    public class CommandesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CommandesController> _logger;

        public CommandesController(ApplicationDbContext context, ILogger<CommandesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Commandes
        public async Task<IActionResult> Index()
        {
            try
            {
                var commandes = await _context.Commandes
                    .Include(c => c.Client)
                    .Include(c => c.LignesCommande)
                    .ThenInclude(l => l.Product)
                    .ToListAsync();

                return View(commandes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des commandes");
                return View(new List<Commande>());
            }
        }

        // GET: Commandes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var commande = await _context.Commandes
                .Include(c => c.Client)
                .Include(c => c.LignesCommande)
                .ThenInclude(l => l.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (commande == null) return NotFound();

            return View(commande);
        }

        // GET: Commandes/Create
        // GET: Commandes/Create
        public IActionResult Create()
        {
            try
            {
                ViewBag.Clients = new SelectList(_context.Clients.ToList(), "Id", "NomComplet");

                // Inclure les prix dans les données des produits
                var productsWithPrice = _context.Products.Select(p => new {
                    Value = p.Id,
                    Text = $"{p.Nom} - {p.Prix}DT",
                    Price = p.Prix
                }).ToList();

                ViewBag.Products = new SelectList(productsWithPrice, "Value", "Text");
                ViewBag.ProductsData = productsWithPrice; // Données supplémentaires pour JavaScript

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du chargement de la page de création");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Commandes/Create - VERSION CORRIGÉE
        [HttpPost]
        public async Task<IActionResult> Create(IFormCollection form)
        {
            try
            {
                Debug.WriteLine("=== DÉBUT CRÉATION COMMANDE ===");

                // 1. VALIDATION DE BASE
                if (!int.TryParse(form["ClientId"], out int clientId) || clientId <= 0)
                {
                    Debug.WriteLine("❌ ClientId invalide");
                    TempData["ErrorMessage"] = "Veuillez sélectionner un client valide";
                    return RedirectToAction(nameof(Create));
                }

                // 2. CRÉATION DE LA COMMANDE
                var commande = new Commande
                {
                    ClientId = clientId,
                    Statut = form["Statut"].ToString() ?? "En attente", // CORRECTION ICI
                    DateCommande = DateTime.Now,
                    Total = 0
                };

                _context.Commandes.Add(commande);
                await _context.SaveChangesAsync();
                Debug.WriteLine($"✅ Commande créée avec ID: {commande.Id}");

                // 3. TRAITEMENT DES PRODUITS
                var productIds = form["productIds"].ToString().Split(',');
                var quantities = form["quantities"].ToString().Split(',');

                decimal totalCommande = 0;

                for (int i = 0; i < productIds.Length; i++)
                {
                    if (!string.IsNullOrEmpty(productIds[i]) &&
                        int.TryParse(productIds[i], out int productId) &&
                        int.TryParse(quantities[i], out int quantity) &&
                        productId > 0 && quantity > 0)
                    {
                        var product = await _context.Products.FindAsync(productId);
                        if (product != null)
                        {
                            var ligneCommande = new LigneCommande
                            {
                                CommandeId = commande.Id,
                                ProductId = productId,
                                Quantite = quantity,
                                PrixUnitaire = product.Prix
                            };

                            _context.LignesCommande.Add(ligneCommande);
                            totalCommande += product.Prix * quantity;

                            Debug.WriteLine($"✅ Produit ajouté: {product.Nom}, Quantité: {quantity}");
                        }
                    }
                }

                // 4. MISE À JOUR DU TOTAL
                commande.Total = totalCommande;
                _context.Commandes.Update(commande);
                await _context.SaveChangesAsync();

                Debug.WriteLine($"✅ Commande finalisée - Total: {totalCommande}€");
                TempData["SuccessMessage"] = "Commande créée avec succès!";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ ERREUR: {ex.Message}");
                Debug.WriteLine(ex.StackTrace);

                TempData["ErrorMessage"] = "Une erreur s'est produite: " + ex.Message;
                return RedirectToAction(nameof(Create));
            }
        }

        // GET: Commandes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // Dans les actions Create et Edit
            ViewBag.Clients = new SelectList(_context.Clients, "Id", "NomComplet");
            ViewBag.Products = new SelectList(_context.Products.Select(p => new {
                Value = p.Id,
                Text = $"{p.Nom}|{p.Prix}"
            }), "Value", "Text");
            if (id == null)
            {
                return NotFound();
            }

            var commande = await _context.Commandes.FindAsync(id);
            if (commande == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "NomComplet", commande.ClientId);
            return View(commande);
        }

        // POST: Commandes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClientId,DateCommande,Statut,Total")] Commande commande)
        {
            if (id != commande.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(commande);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommandeExists(commande.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "NomComplet", commande.ClientId);
            return View(commande);
        }

        // GET: Commandes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Charger explicitement les lignes de commande pour éviter les null
            var commande = await _context.Commandes
                .Include(c => c.Client)
                .Include(c => c.LignesCommande) // IMPORTANT: Inclure les lignes de commande
                    .ThenInclude(l => l.Product) // Et les produits associés
                .FirstOrDefaultAsync(m => m.Id == id);

            if (commande == null)
            {
                return NotFound();
            }

            // S'assurer que LignesCommande n'est pas null
            if (commande.LignesCommande == null)
            {
                commande.LignesCommande = new List<LigneCommande>();
            }

            return View(commande);
        }

        // POST: Commandes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var commande = await _context.Commandes.FindAsync(id);
            if (commande != null)
            {
                _context.Commandes.Remove(commande);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommandeExists(int id)
        {
            return _context.Commandes.Any(e => e.Id == id);
        }
    }
}