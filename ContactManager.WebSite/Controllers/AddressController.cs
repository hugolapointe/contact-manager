using ContactManager.Core;
using ContactManager.Core.Domain.Entities;
using ContactManager.WebSite.Utilities;
using ContactManager.WebSite.ViewModels;
using ContactManager.WebSite.ViewModels.Address;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.WebSite.Controllers;

[Authorize]
public class AddressController(
    ContactManagerContext context,
    DomainAsserts asserts) : Controller {
    private readonly ContactManagerContext context = context;
    private readonly DomainAsserts asserts = asserts;

    [HttpGet]
    public async Task<IActionResult> Manage(Guid contactId) {
        var contact = await context.Contacts.FindAsync(contactId);

        asserts.Exists(contact, "Contact not found.");
        asserts.IsOwnedByCurrentUser(contact, User);

        await context.Entry(contact!).Collection(c => c.Addresses).LoadAsync();

        var addresses = contact.Addresses
            .Select(address => new AddressItem() {
                Id = address.Id,
                StreetNumber = address.StreetNumber,
                StreetName = address.StreetName,
                City = address.CityName,
                PostalCode = address.PostalCode,
            });

        ViewBag.ContactId = contactId;
        ViewBag.ContactFullName = contact.FullName;
        return View(addresses);
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid contactId) {
        var contact = await context.Contacts.FindAsync(contactId);

        asserts.Exists(contact, "Contact not found.");
        asserts.IsOwnedByCurrentUser(contact, User);

        ViewBag.ContactId = contactId;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Guid contactId, AddressCreate vm) {
        if (!ModelState.IsValid) {
            ViewBag.ContactId = contactId;
            return View(vm);
        }

        var contact = await context.Contacts.FindAsync(contactId);

        asserts.Exists(contact, "Contact not found.");
        asserts.IsOwnedByCurrentUser(contact, User);

        Address toAdd = new Address() {
            StreetNumber = vm.StreetNumber!.Value,
            StreetName = vm.StreetName!,
            CityName = vm.CityName!,
            PostalCode = vm.PostalCode!,
        };
        contact!.Addresses.Add(toAdd);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Manage), new { contactId });
    }

    [HttpGet]   
    public async Task<IActionResult> Edit(Guid id) {
        var toEdit = await context.Addresses.FindAsync(id);

        asserts.Exists(toEdit, "Address not found.");

        await context.Entry(toEdit!).Reference(a => a.Contact).LoadAsync();

        asserts.Exists(toEdit.Contact, "Contact not found.");
        asserts.IsOwnedByCurrentUser(toEdit.Contact, User);

        var vm = new AddressEdit() {
            StreetNumber = toEdit.StreetNumber,
            StreetName = toEdit.StreetName,
            CityName = toEdit.CityName,
            PostalCode = toEdit.PostalCode,
        };

        ViewBag.Id = id;
        ViewBag.ContactId = toEdit.Contact.Id;
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, AddressEdit vm) {
        var toEdit = await context.Addresses.FindAsync(id);

        asserts.Exists(toEdit, "Address not found.");

        if (!ModelState.IsValid) {
            ViewBag.Id = id;
            ViewBag.ContactId = toEdit!.ContactId;
            return View(vm);
        }

        await context.Entry(toEdit!).Reference(a => a.Contact).LoadAsync();

        asserts.Exists(toEdit.Contact, "Contact not found.");
        asserts.IsOwnedByCurrentUser(toEdit.Contact, User);

        toEdit.StreetNumber = vm.StreetNumber!.Value;
        toEdit.StreetName = vm.StreetName!;
        toEdit.CityName = vm.CityName!;
        toEdit.PostalCode = vm.PostalCode!;
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Manage),
            new { contactId = toEdit.Contact.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Remove(Guid id) {
        var toRemove = await context.Addresses.FindAsync(id);

        asserts.Exists(toRemove, "Address not found.");

        await context.Entry(toRemove!).Reference(a => a.Contact).LoadAsync();

        asserts.Exists(toRemove.Contact, "Contact not found.");
        asserts.IsOwnedByCurrentUser(toRemove.Contact, User);

        context.Addresses.Remove(toRemove);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Manage),
            new { contactId = toRemove.Contact.Id });
    }
}
