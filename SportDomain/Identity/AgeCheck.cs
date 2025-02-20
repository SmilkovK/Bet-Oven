using System.ComponentModel.DataAnnotations;

public class AgeCheck : ValidationAttribute
{
    private int _Limit;
    public AgeCheck(int Limit)
    { // The constructor which we use in modal.
        this._Limit = Limit;
    }
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        DateTime bday = DateTime.Parse(value.ToString());
        DateTime today = DateTime.Today;
        int age = today.Year - bday.Year;
        if (bday > today.AddYears(-age))
        {
            age--;
        }
        if (age < _Limit)
        {
            var result = new ValidationResult($"You must be at least {_Limit} years old.");
            return result;
        }

        return null;

    }
}