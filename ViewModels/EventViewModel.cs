using ProjectCMS.Services;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjectCMS.ViewModels
{
    public class EventViewModel: ValidationAttribute, INotifyPropertyChanged
    {


        [Required]
        [MinLength(3, ErrorMessage = "Name length must be more than 3 characters"), MaxLength(20, ErrorMessage = "Name length must be less than 20 characters ")]
        public string Name { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "Name length must be more than 3 characters"), MaxLength(100, ErrorMessage = "Name length must be less than 100 characters ")]
        public string Content { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [FutureDate(ErrorMessage = "The date must be equal or greater than now")]
        public DateTime First_Closure { get; set; }
        public string _errorMessage;

        public event PropertyChangedEventHandler PropertyChanged;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
