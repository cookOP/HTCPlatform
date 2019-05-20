using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HTCPlatform.Api.ViewModel.Product;
using HTCPlatform.Domain.Models;

namespace HTCPlatform.Api.Mapper
{
    public class ViewModelToDomainMapper:Profile
    {
        public ViewModelToDomainMapper()
        {
            CreateMap<AddProductViewModel, Products>();
            CreateMap<UpdateProductViewModel, Products>();
        }
    }
}
