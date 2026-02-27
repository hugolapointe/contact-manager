using System.ComponentModel.DataAnnotations;

using ContactManager.WebSite.ViewModels.Shared;

namespace ContactManager.WebSite.ViewModels.Address;

public class AddressList : PaginatedList<AddressItem> {
    public Guid ContactId { get; }

    [Display(Name = "Contact")]
    public string ContactFullName { get; }

    public AddressList(
        IReadOnlyList<AddressItem> items,
        int totalCount,
        int pageIndex,
        int pageSize,
        Guid contactId,
        string contactFullName) : base(items, totalCount, pageIndex, pageSize) {
        ContactId = contactId;
        ContactFullName = contactFullName;
    }
}
