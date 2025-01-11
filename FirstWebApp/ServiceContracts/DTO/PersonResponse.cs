using Entities;
using ServiceContracts.Enums;

namespace ServiceContracts.DTO
{
    public class PersonResponse
    {
        public Guid PersonID { get; set; }

        public string? PersonName { get; set; }

        public string? Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public Guid CountryID { get; set; }
        public string? Address { get; set; }
        public bool RecieveNewsLetter { get; set; }

        public double? Age { get; set; }

        public string? CountryName { get; set; }


        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(PersonResponse)) return false;

            PersonResponse personResponse = (PersonResponse)obj;

            return PersonID == personResponse.PersonID &&
                PersonName == personResponse.PersonName &&
                Email == personResponse.Email &&
                DateOfBirth == personResponse.DateOfBirth &&
                Gender == personResponse.Gender &&
                CountryID == personResponse.CountryID &&
                Address == personResponse.Address &&
                RecieveNewsLetter == personResponse.RecieveNewsLetter;

        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return $" PersonID:{PersonID} Person Name : {PersonName}";
        }

        public PersonUpdateRequest ToPersonUpdateRequest()
        {


            return new PersonUpdateRequest()
            {
                PersonID = PersonID,
                PersonName = PersonName,
                Email = Email,
                DateOfBirth = Convert.ToDateTime(DateOfBirth),
                Gender = !string.IsNullOrEmpty(Gender) ? (GenderOptions)Enum.Parse(typeof(GenderOptions), Gender, true) : GenderOptions.others,
                Address = Address,
                CountryID = CountryID,
                RecieveNewsLetter = RecieveNewsLetter,
                
            };

        }
    }

    public static class PersonExtension
    {
        public static PersonResponse ToPersonResponse(this Person person)
        {
            return new PersonResponse()
            {
                PersonID = person.PersonID,
                PersonName = person.PersonName,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender,
                CountryID = person.CountryID,
                Address = person.Address,
                RecieveNewsLetter = person.RecieveNewsLetter,
                Age = ((person.DateOfBirth != null)
                    ? Math.Round((DateTime.Now - person.DateOfBirth.Value).TotalDays / 365.25) : null),
                CountryName = person.Country?.CountryName,
                   
            };
        }
    }
}
