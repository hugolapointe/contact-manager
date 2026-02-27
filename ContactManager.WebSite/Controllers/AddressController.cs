using ContactManager.Core;
using ContactManager.Core.Domain.Entities;
using ContactManager.WebSite.Authorization;
using ContactManager.WebSite.Utilities;
using ContactManager.WebSite.ViewModels.Address;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContactManager.WebSite.Controllers;

[Authorize]
[ContactOwner("contactId")]
public class AddressController(ContactManagerContext context) : Controller {

    private const int PageSize = 10;

    [HttpGet]
    public async Task<IActionResult> Index(Guid contactId, int page = 1) {
        var contact = HttpContext.GetContactOwned();

        var baseQuery = context.Addresses
            .AsNoTracking()
            .Where(address => address.ContactId == contactId);

        var totalCount = await baseQuery.CountAsync();

        var items = await baseQuery
            .OrderBy(address => address.StreetName)
            .ThenBy(address => address.StreetNumber)
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .Select(address => new AddressItem {
                Id = address.Id,
                StreetNumber = address.StreetNumber,
                StreetName = address.StreetName,
                City = address.CityName,
                PostalCode = address.PostalCode,
                CreatedAt = address.CreatedAt,
                UpdatedAt = address.UpdatedAt,
            }).ToListAsync();

        return View(new AddressList(items, totalCount, page, PageSize, contactId, contact.FullName));
    }

    [HttpGet]
    public IActionResult Create(Guid contactId) {
        return View(new AddressCreate {
            ContactId = contactId,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Guid contactId, [FromForm] AddressCreate viewModel) {
        // Synchroniser avec la route : la route est la source de vérité (protection anti-tamper).
        viewModel.ContactId = contactId;

        if (!ModelState.IsValid) {
            return View(viewModel);
        }

        var address = Address.Create(
            contactId,
            viewModel.StreetNumber!.Value,
            viewModel.StreetName!,
            viewModel.CityName!,
            viewModel.PostalCode!);

        context.Addresses.Add(address);
        await context.SaveChangesAsync();

        this.AddNotification("Address added successfully.", NotificationType.Success);
        return RedirectToAction(nameof(Index), new { contactId });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, Guid contactId) {
        var addressToEdit = await context.Addresses.FindAsync(id);
        if (addressToEdit is null || addressToEdit.ContactId != contactId) {
            return NotFound();
        }

        var viewModel = new AddressEdit {
            AddressId = id,
            ContactId = contactId,
            StreetNumber = addressToEdit.StreetNumber,
            StreetName = addressToEdit.StreetName,
            CityName = addressToEdit.CityName,
            PostalCode = addressToEdit.PostalCode,
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Guid contactId, AddressEdit viewModel) {
        var contact = HttpContext.GetContactOwned();

        // Synchroniser avec la route : la route est la source de vérité (protection anti-tamper).
        viewModel.AddressId = id;
        viewModel.ContactId = contact.Id;

        if (!ModelState.IsValid) {
            return View(viewModel);
        }

        var addressToEdit = await context.Addresses.FindAsync(id);
        if (addressToEdit is null || addressToEdit.ContactId != contact.Id) {
            return NotFound();
        }

        addressToEdit.Update(
            viewModel.StreetNumber!.Value,
            viewModel.StreetName!,
            viewModel.CityName!,
            viewModel.PostalCode!);
        await context.SaveChangesAsync();

        this.AddNotification("Address updated successfully.", NotificationType.Success);
        return RedirectToAction(nameof(Index), new { contactId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(Guid id, Guid contactId) {
        var contact = HttpContext.GetContactOwned();

        var addressToRemove = await context.Addresses.FindAsync(id);
        if (addressToRemove is null || addressToRemove.ContactId != contact.Id) {
            return NotFound();
        }

        context.Addresses.Remove(addressToRemove);
        await context.SaveChangesAsync();

        this.AddNotification("Address deleted successfully.", NotificationType.Success);
        return RedirectToAction(nameof(Index), new { contactId });
    }
}
