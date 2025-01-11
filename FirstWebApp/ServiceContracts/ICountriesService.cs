using ServiceContracts.DTO;

namespace ServiceContracts
{
    public interface ICountriesService
    {
        /// <summary>
        /// Add country object to the list of countries
        /// </summary>
        /// <param name="countryAddRequest">country object to add</param>
        /// <returns></returns>
       Task< CountryResponse> AddCountry(CountryAddRequest countryAddRequest);
        /// <summary>
        /// Return all the countries as list
        /// </summary>
        /// <returns></returns>
       Task< List<CountryResponse>> GetAllCountries();

        /// <summary>
        /// retrun country obj 
        /// </summary>
        /// <param name="CountryID">Guid</param>
        /// <returns>Matching country object</returns>
        Task<CountryResponse> GetCountryByCountryID(Guid? CountryID);

        Task<CountryResponse> GetCountryByCountryName(string CountryName);
    }
}
