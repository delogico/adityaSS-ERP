using RMERP.DAL.Models;
using RMERP.DAL.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace RMERP.DAL.Mappers
{
    public class CitiesMapper
    {
        public static CitiesVM mapMe(Cities cities)
        {
            CitiesVM cityVM = new CitiesVM();
            cityVM.CITY_Id = cities.CITY_Id;
            cityVM.CITY_Name = cities.CITY_Name;
            return cityVM;
        }

        public static List<CitiesVM> mapCities(List<Cities> cities)
        {
            List<CitiesVM> lst = new List<CitiesVM>();
            foreach (Cities city in cities)
            {
                lst.Add(mapMe(city));
            }
            return lst;
        }
    }
}
