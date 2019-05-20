
using AutoMapper;
using HTCPlatform.Domain.Models;
using HTCPlatform.ViewModel.Models.Product;

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
