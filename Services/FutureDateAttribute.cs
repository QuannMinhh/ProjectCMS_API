using System.ComponentModel.DataAnnotations;

namespace ProjectCMS.Services
{
    public class FutureDateAttribute: ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            DateTime inputDate = (DateTime)value;
            DateTime currentDate = DateTime.Now;

            if (inputDate >= currentDate)
            {
                return true;
            }

            return false;
        }
    }
}
