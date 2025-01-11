using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO
{
    public class PersonAddRequest
    {
        [Required(ErrorMessage = "Name Required")]
        public string? PersonName { get; set; }
        [EmailAddress(ErrorMessage = "Invalid Email")]
        [Required(ErrorMessage = "Email Required")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Date Required")]
        public DateTime DateOfBirth { get; set; }
        [Required(ErrorMessage = "Gender Required")]
        public GenderOptions? Gender { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public Guid CountryID { get; set; }
        [Required(ErrorMessage = "Address Required")]
        public string? Address { get; set; }
        public bool RecieveNewsLetter { get; set; }


        public Person ToPerson()
        {
            return new Person()
            {
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = DateOfBirth,
                Gender = Gender.ToString(),
                CountryID = CountryID,
                Address = Address,
                RecieveNewsLetter = RecieveNewsLetter
            };



        }
    }
}
