using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMERP.DAL.Mappers
{
    public class CitiesMapper
    {
        public static CitiesVM mapMe(City cities)
        {
            CitiesVM cityVM = new CitiesVM();
            cityVM.CITY_Id = cities.CIT_Id;
            cityVM.CITY_Name = cities.CIT_Name;
            return cityVM;
        }

        public static List<CitiesVM> mapCities(List<City> cities)
        {
            List<CitiesVM> lst = new List<CitiesVM>();
            foreach (City city in cities)
            {
                lst.Add(mapMe(city));
            }
            return lst;
        }
    }
}
