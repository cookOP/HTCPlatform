using AutoMapper;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace HTCPlatform.ViewModel.Mapper
{
    /// <summary>
    /// 领域模型转换为视图模型 配置
    /// </summary>
    public class DomainMapperToViewModel : Profile, IMapperProfile
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DomainMapperToViewModel()
        {
         
        }

        /// <summary>
        /// 未知属性
        /// </summary>
        [ExcludeFromCodeCoverage]
        public int Order => 1;
    }
}
