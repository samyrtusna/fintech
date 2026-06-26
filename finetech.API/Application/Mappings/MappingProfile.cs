using AutoMapper;
using fintech.API.Application.DTOs.AuthDtos;
using fintech.API.Application.DTOs.CategoryDtos;
using fintech.API.Application.DTOs.ExchangeRateDtos;
using fintech.API.Application.DTOs.FinancialIntelligenceSnapshotDtos;
using fintech.API.Application.DTOs.FinancialTransactionDtos;
using fintech.API.Domain.Entities;

namespace fintech.API.Application.Mappings
{
    public class MappingProfile : Profile 
    {
        public MappingProfile() 
        {
            CreateMap<RegisterRequestDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore())
                .ForMember(dest => dest.BaseCurrency, opt => opt.Ignore())
                .ForMember(dest => dest.Role, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());


            CreateMap<CategoryRequestDto, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.ParentCategory, opt => opt.Ignore())
                .ForMember(dest => dest.IsSystem, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Transactions, opt => opt.Ignore());

            CreateMap<Category, CategoryResponseDto>()
                .ForMember(dest => dest.ParentCategory, opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.Name : null));


            CreateMap<UpdateCategoryRequestDto, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore()) 
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .ForMember(dest => dest.ParentCategoryId, opt => opt.Ignore())
                .ForMember(dest => dest.ParentCategory, opt => opt.Ignore())
                .ForMember(dest => dest.IsSystem, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Transactions, opt => opt.Ignore());

            CreateMap<UserCategorySettingsRequestDto, UserCategorySetting>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore());

            CreateMap<FinancialTransactionRequestDto, FinancialTransaction>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .ForMember(dest => dest.ExchangeRate, opt => opt.Ignore())
                .ForMember(dest => dest.BaseAmount, opt => opt.Ignore())
                .ForMember(dest => dest.IsEssential, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.TransactionDate, act => act.NullSubstitute(DateTime.UtcNow))
                .ForMember(dest => dest.Description, act => act.NullSubstitute(string.Empty));

            CreateMap<FinancialTransaction, FinancialTransactionResponseDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<UpdateFinancialTransactionRequestDto, FinancialTransaction>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
                .ForMember(dest => dest.Type, opt => opt.Ignore())
                .ForMember(dest => dest.Amount, opt => opt.Ignore())
                .ForMember(dest => dest.Currency, opt => opt.Ignore())
                .ForMember(dest => dest.ExchangeRate, opt => opt.Ignore())
                .ForMember(dest => dest.BaseAmount, opt => opt.Ignore())
                .ForMember(dest => dest.TransactionDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Description, opt => opt.Condition(src => src.Description != null))
                .ForMember(dest => dest.IsEssential, opt => opt.Condition(src => src.IsEssential.HasValue));

            CreateMap<CreateExchangeRateRequestDto, ExchangeRate>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Date, opt => opt.Ignore());

            CreateMap<ExchangeRate, ExchangeRateResponseDto>();

            CreateMap<FinancialIntelligenceSnapshotRequestDto, FinancialIntelligenceSnapshot>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<FinancialIntelligenceSnapshot, FinancialIntelligenceSnapshotResponseDto>();

        }
    }
} 
