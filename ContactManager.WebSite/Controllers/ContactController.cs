using ContactManager.Core;
using ContactManager.Core.Domain.Entities;
using ContactManager.WebSite.Utilities;
using ContactManager.WebSite.ViewModels.Contact;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ContactManager.WebSite.Controllers;

[Authorize]
public class ContactController(
    ContactManagerContext context,
    UserManager<User> userManager,
    DomainAsserts asserts) : Controller {
    private readonly ContactManagerContext context = context;
    private readonly UserManager<User> userManager = userManager;
    private readonly DomainAsserts asserts = asserts;

    [HttpGet]
    public async Task<IActionResult> Manage() {
        var user = await userManager.GetUserAsync(User);

        context.Entry(user!).Collection(u => u.Contacts).Load();

        var contacts = user!.Contacts
            .Select(contact => new ContactItem() {
                Id = contact.Id,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Age = contact.Age,
            });

        return View(contacts);
    }

    [HttpGet]
    public IActionResult Create() {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ContactCreate vm) {
        if (!ModelState.IsValid) {
            return View(vm);
        }

        var toAdd = new Contact() {
            FirstName = vm.FirstName!,
            LastName = vm.LastName!,
            DateOfBirth = vm.DateOfBirth!.Value,
        };
        toAdd.Addresses.Add(new Address() {
            StreetNumber = vm.Address_StreetNumber!.Value,
            StreetName = vm.Address_StreetName!,
            CityName = vm.Address_CityName!,
            PostalCode = vm.Address_PostalCode!,
        });

        var user = await userManager.GetUserAsync(User);
        user!.Contacts.Add(toAdd);
        context.SaveChanges();

        return RedirectToAction(nameof(Manage));
    }

    [HttpGet]
    public IActionResult Edit(Guid id) {
        var toEdit = context.Contacts.Find(id);

        asserts.Exists(toEdit, "Contact not found.");
        asserts.IsOwnedByCurrentUser(toEdit, User);

        var vm = new ContactEdit() {
            FirstName = toEdit.FirstName,
            LastName = toEdit.LastName,
            DateOfBirth = toEdit.DateOfBirth,
        };

        ViewBag.Id = id;
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Guid id, ContactEdit vm) {
        if (!ModelState.IsValid) {
            ViewBag.Id = id;
            return View(vm);
        }

        var toEdit = context.Contacts.Find(id);

        asserts.Exists(toEdit, "Contact not found.");
        asserts.IsOwnedByCurrentUser(toEdit, User);

        toEdit.FirstName = vm.FirstName!;
        toEdit.LastName = vm.LastName!;
        toEdit.DateOfBirth = vm.DateOfBirth!.Value;
        context.SaveChanges();

        return RedirectToAction(nameof(Manage));
    }

    [HttpGet]
    public IActionResult Remove(Guid id) {
        var toRemove = context.Contacts.Find(id);

        asserts.Exists(toRemove, "Contact not found.");
        asserts.IsOwnedByCurrentUser(toRemove, User);

        context.Contacts.Remove(toRemove);
        context.SaveChanges();

        return RedirectToAction(nameof(Manage));
    }
}