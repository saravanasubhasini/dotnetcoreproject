using Entities;
using Microsoft.EntityFrameworkCore;
using ServiceContracts;
using ServiceContracts.DTO;
using RepositoryContracts;

namespace Services
{
    public class CountriesService : ICountriesService
    {
       
        private readonly ICountriesRepository _db;

        public CountriesService(ICountriesRepository db)
        {
            
            _db = db;
            
        }

        public async Task< CountryResponse> AddCountry(CountryAddRequest countryAddRequest)

        {

            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }

            if (countryAddRequest.CountryName == null)
            {
                throw new ArgumentException(nameof(countryAddRequest.CountryName));
            }

            if (await _db.GetCountryByCountryName(countryAddRequest.CountryName) != null )
            {
                throw new ArgumentException("Given Country Name Already present in the table");
            }
            Country country = countryAddRequest.ToCountry();

            country.CountryID = Guid.NewGuid();

            await _db.AddCountry(country);
            

            return country.ToCountryResponse();

        }

        public async Task<List<CountryResponse>> GetAllCountries()
        {
            var Country = await _db.GetAllCountries();
            return Country.Select(temp => temp.ToCountryResponse()).ToList();
        }

        public async Task<CountryResponse> GetCountryByCountryID(Guid? CountryID)
        {
            if (CountryID == null)
                return new CountryResponse();
            //throw new ArgumentNullException();

            Country? countryFromList = await _db.GetCountryByCountryID(CountryID.Value);

            if (countryFromList == null)  
                throw new InvalidOperationException();

            return countryFromList.ToCountryResponse();


        }

        public async Task<CountryResponse> GetCountryByCountryName(string CountryName)
        {
            if (CountryName == null)
            {
                return new CountryResponse();
            }
            Country? countryFromList = await _db.GetCountryByCountryName(CountryName);

            if (countryFromList == null)
                throw new InvalidOperationException();

            return countryFromList.ToCountryResponse();
        }
    }
}
