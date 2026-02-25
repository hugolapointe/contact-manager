using ContactManager.Core;
using ContactManager.Core.Domain.Entities;
using ContactManager.WebSite.Authorization;
using ContactManager.WebSite.Utilities;
using ContactManager.WebSite.ViewModels.Contact;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.WebSite.Controllers;

[Authorize]
public class ContactController(ContactManagerContext context) : Controller {
    private readonly ContactManagerContext _context = context;

    [HttpGet]
    public async Task<IActionResult> Manage() {
        var currentUserId = this.GetRequiredUserId();

        var contactItems = await _context.Contacts
            .AsNoTracking()
            .Where(contact => contact.OwnerId == currentUserId)
            .OrderBy(contact => contact.LastName)
            .ThenBy(contact => contact.FirstName)
            .Select(contact => new ContactItem() {
                Id = contact.Id,
                FullName = contact.FullName,
                Age = contact.Age,
                CreatedAt = contact.CreatedAt,
                UpdateAt = contact.UpdateAt,
            })
            .ToListAsync();

        return View(contactItems);
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

        var contactToCreate = Contact.Create(
            currentUserId,
            viewModel.FirstName!,
            viewModel.LastName!,
            viewModel.DateOfBirth!.Value);
        await _context.Contacts.AddAsync(contactToCreate);

        var defaultAddress = Address.Create(
            contactToCreate.Id,
            viewModel.Address_StreetNumber!.Value,
            viewModel.Address_StreetName!,
            viewModel.Address_CityName!,
            viewModel.Address_PostalCode!);

        contactToCreate.Addresses.Add(defaultAddress);
        await _context.SaveChangesAsync();

        this.SetSuccessMessage("Contact created successfully.");
        return RedirectToAction(nameof(Manage));
    }

    [HttpGet]
    [ContactOwner]
    public IActionResult Edit(Guid id) {
        var contactToEdit = HttpContext.GetResourceOwned<Contact>();

        var viewModel = new ContactEdit() {
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
        if (!ModelState.IsValid) {
            viewModel.Id = id;
            return View(viewModel);
        }

        var contactToEdit = HttpContext.GetResourceOwned<Contact>();
        contactToEdit.Update(viewModel.FirstName!, viewModel.LastName!, viewModel.DateOfBirth!.Value);
        await _context.SaveChangesAsync();

        this.SetSuccessMessage("Contact updated successfully.");
        return RedirectToAction(nameof(Manage));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ContactOwner]
    public async Task<IActionResult> Remove(Guid id) {
        var contactToRemove = HttpContext.GetResourceOwned<Contact>();

        _context.Contacts.Remove(contactToRemove);
        await _context.SaveChangesAsync();

        this.SetSuccessMessage("Contact deleted successfully.");
        return RedirectToAction(nameof(Manage));
    }
}
