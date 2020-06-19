using AutoMapper;
using DysFin.Models;
using DysFin.Models.DysFinViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DysFin.Extensions
{
    public class Profiles : Profile
    {
        public Profiles()
        {
            CreateMap<Uzytkownik, UzytkownikViewModel>();
            CreateMap<JednostkaKontrolowana, JednostkaKontrolowanaViewModel>();
            CreateMap<Kontrola, KontrolaViewModel>();
        }
    }
}
