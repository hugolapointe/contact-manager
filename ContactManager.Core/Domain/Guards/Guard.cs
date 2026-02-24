namespace ContactManager.Core.Domain.Guards;

public static class Guard {
    public static void AgainstEmptyGuid(Guid value, string paramName, string message) {
        if (value == Guid.Empty) {
            throw new ArgumentException(message, paramName);
        }
    }

    public static void AgainstInvalidBirthDate(DateTime birthDate, string paramName) {
        if (birthDate == default) {
            throw new ArgumentException("Birth date is required.", paramName);
        }

        if (birthDate.Date >= DateTime.Today) {
            throw new ArgumentOutOfRangeException(paramName, "Birth date must be in the past.");
        }
    }
}
