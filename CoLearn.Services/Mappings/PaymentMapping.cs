using AutoMapper;
using CoLearn.Domain.DTOs;
using CoLearn.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoLearn.Services.Mappings
{
    class PaymentMapping : Profile
    {
        public PaymentMapping() 
        {
            // Request -> Entity
            CreateMap<TransactionRequest, Transaction>();

            // Entity -> Response
            CreateMap<Transaction, TransactionResponse>()
            .ForMember(dest => dest.PaymentDetail,
                opt => opt.MapFrom(src => src.Payment)); // map Payment -> PaymentDetail


            CreateMap<Payment, Detail>()
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.StatusName))
            .ForMember(dest => dest.MethodName, opt => opt.MapFrom(src => src.Method.MethodName))
            .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.StatusId))
            .ForMember(dest => dest.MethodId, opt => opt.MapFrom(src => src.MethodId))
            .ForMember(dest => dest.PaymentId, opt => opt.MapFrom(src => src.PaymentId))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));

        }

    }
}
