using ContactManager.Core;
using ContactManager.Core.Domain.Entities;
using ContactManager.WebSite.Authorization;
using ContactManager.WebSite.Utilities;
using ContactManager.WebSite.ViewModels.Contact;
using ContactManager.WebSite.ViewModels.Shared;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.WebSite.Controllers;

[Authorize]
public class ContactController(ContactManagerContext context) : Controller {

    private const int PageSize = 10;

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1) {
        var currentUserId = this.GetRequiredUserId();

        var baseQuery = context.Contacts
            .AsNoTracking()
            .Where(contact => contact.OwnerId == currentUserId);

        var totalCount = await baseQuery.CountAsync();

        var items = await baseQuery
            .OrderBy(contact => contact.LastName)
            .ThenBy(contact => contact.FirstName)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Select(contact => new ContactItem {
                Id = contact.Id,
                FullName = contact.FirstName + " " + contact.LastName,
                Age = contact.Age,
                CreatedAt = contact.CreatedAt,
                UpdatedAt = contact.UpdatedAt,
            }).ToListAsync();

        return View(new PaginatedList<ContactItem>(items, totalCount, page, PageSize));
    }

    [HttpGet]
    public IActionResult Create() {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ContactCreate viewModel) {
        if (!ModelState.IsValid) {
            return View(viewModel);
        }

        var currentUserId = this.GetRequiredUserId();

        var contact = Contact.Create(
            currentUserId,
            viewModel.FirstName!,
            viewModel.LastName!,
            viewModel.DateOfBirth!.Value);

        contact.Addresses.Add(Address.Create(
            contact.Id,
            viewModel.Address_StreetNumber!.Value,
            viewModel.Address_StreetName!,
            viewModel.Address_CityName!,
            viewModel.Address_PostalCode!));

        context.Contacts.Add(contact);
        await context.SaveChangesAsync();

        this.AddNotification("Contact created successfully.", NotificationType.Success);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [ContactOwner]
    public IActionResult Edit(Guid id) {
        var contactToEdit = HttpContext.GetContactOwned();

        var viewModel = new ContactEdit {
            Id = id,
            FirstName = contactToEdit.FirstName,
            LastName = contactToEdit.LastName,
            DateOfBirth = contactToEdit.DateOfBirth,
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ContactOwner]
    public async Task<IActionResult> Edit(Guid id, ContactEdit viewModel) {
        viewModel.Id = id;  // Toujours synchroniser avec la route (protection anti-tamper).

        if (!ModelState.IsValid) {
            return View(viewModel);
        }

        var contactToEdit = HttpContext.GetContactOwned();
        contactToEdit.Update(viewModel.FirstName!, viewModel.LastName!, viewModel.DateOfBirth!.Value);
        await context.SaveChangesAsync();

        this.AddNotification("Contact updated successfully.", NotificationType.Success);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ContactOwner]
    public async Task<IActionResult> Remove(Guid id) {
        var contactToRemove = HttpContext.GetContactOwned();

        context.Contacts.Remove(contactToRemove);
        await context.SaveChangesAsync();

        this.AddNotification("Contact deleted successfully.", NotificationType.Success);
        return RedirectToAction(nameof(Index));
    }
}
