using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace HTCPlatform.ViewModel.Mapper.Extension
{
    internal class NullableDateTimeConverter : ITypeConverter<DateTime?, DateTime>
    {
        public DateTime Convert(DateTime? source, DateTime destination, ResolutionContext context)
        {
            return source.SetTimeNullDefaultDate();
        }
    }
}
