using System.ComponentModel.DataAnnotations;
using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO
{
    public class PersonUpdateRequest
    {
        public Guid PersonID { get; set; }

        [Required(ErrorMessage = "Name Required")]
        public string? PersonName { get; set; }
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string? Email { get; set; }

        public DateTime DateOfBirth { get; set; }
        public GenderOptions Gender { get; set; }
        public Guid CountryID { get; set; }
        public string? Address { get; set; }
        public bool RecieveNewsLetter { get; set; }

        public Person ToPerson()
        {
            return new Person()
            {
                PersonID = PersonID,
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
