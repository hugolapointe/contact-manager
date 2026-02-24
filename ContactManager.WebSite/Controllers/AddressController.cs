using ContactManager.Core;
using ContactManager.Core.Domain.Entities;
using ContactManager.WebSite.Authorization;
using ContactManager.WebSite.ViewModels.Address;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.WebSite.Controllers;

[Authorize]
public class AddressController(ContactManagerContext context) : Controller {
    private readonly ContactManagerContext _context = context;

    [HttpGet]
    [ResourceOwner("contactId")]
    public async Task<IActionResult> Manage(Guid contactId) {
        var contact = HttpContext.GetResourceOwned<Contact>();

        var addressItems = await _context.Addresses
            .AsNoTracking()
            .Where(address => address.ContactId == contactId)
            .Select(address => new AddressItem() {
                Id = address.Id,
                StreetNumber = address.StreetNumber,
                StreetName = address.StreetName,
                City = address.CityName,
                PostalCode = address.PostalCode,
            })
            .ToListAsync();

        return View(new AddressManage {
            ContactId = contactId,
            ContactFullName = contact.FullName,
            Addresses = addressItems,
        });
    }

    [HttpGet]
    [ResourceOwner("contactId")]
    public IActionResult Create(Guid contactId) {
        return View(new AddressCreate {
            ContactId = contactId,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ResourceOwner("contactId")]
    public async Task<IActionResult> Create(Guid contactId, AddressCreate viewModel) {
        viewModel.ContactId = contactId;

        if (!ModelState.IsValid) {
            return View(viewModel);
        }

        var contact = HttpContext.GetResourceOwned<Contact>();

        var addressToCreate = Address.Create(
            contact.Id,
            viewModel.StreetNumber!.Value,
            viewModel.StreetName!,
            viewModel.CityName!,
            viewModel.PostalCode!);
        _context.Addresses.Add(addressToCreate);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Manage), new { contactId });
    }

    [HttpGet]
    [ResourceOwner("contactId")]
    public async Task<IActionResult> Edit(Guid id, Guid contactId) {
        var contact = HttpContext.GetResourceOwned<Contact>();

        var addressToEdit = await _context.Addresses.FindAsync(id);
        if (addressToEdit is null || addressToEdit.ContactId != contact.Id) {
            return NotFound();
        }

        var viewModel = new AddressEdit() {
            AddressId = id,
            ContactId = contact.Id,
            StreetNumber = addressToEdit.StreetNumber,
            StreetName = addressToEdit.StreetName,
            CityName = addressToEdit.CityName,
            PostalCode = addressToEdit.PostalCode,
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ResourceOwner("contactId")]
    public async Task<IActionResult> Edit(Guid id, Guid contactId, AddressEdit viewModel) {
        var contact = HttpContext.GetResourceOwned<Contact>();

        viewModel.AddressId = id;
        viewModel.ContactId = contact.Id;

        if (!ModelState.IsValid) {
            return View(viewModel);
        }

        var addressToEdit = await _context.Addresses.FindAsync(id);
        if (addressToEdit is null || addressToEdit.ContactId != contact.Id) {
            return NotFound();
        }

        addressToEdit.Update(
            viewModel.StreetNumber!.Value,
            viewModel.StreetName!,
            viewModel.CityName!,
            viewModel.PostalCode!);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Manage),
            new { contactId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ResourceOwner("contactId")]
    public async Task<IActionResult> Remove(Guid id, Guid contactId) {
        var contact = HttpContext.GetResourceOwned<Contact>();

        var addressToRemove = await _context.Addresses.FindAsync(id);
        if (addressToRemove is null || addressToRemove.ContactId != contact.Id) {
            return NotFound();
        }

        _context.Addresses.Remove(addressToRemove);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Manage),
            new { contactId });
    }
}
