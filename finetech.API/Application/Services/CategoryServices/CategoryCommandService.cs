using AutoMapper;
using fintech.API.Application.DTOs.ApiResponsesDtos;
using fintech.API.Application.DTOs.CategoryDtos;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Application.Interfaces.Services.ICategoryServices;
using fintech.API.Application.Exceptions;
using fintech.API.Domain.Entities;

namespace fintech.API.Application.Services.CategoryServices
{
    public class CategoryCommandService(ICategoryRepository categoryRepository,
        IUserCategorySettingRepository userCategorySettingsRepository,
        IFinancialTransactionRepository transactionRepository,
        IMapper mapper) : ICategoryCommandService
    {
        public async Task<CategoryResponseDto> CreateCategoryAsync(CategoryRequestDto dto, Guid userId) 
        { 
            ArgumentNullException.ThrowIfNull(dto);
            if (userId == Guid.Empty) 
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            if(dto.ParentCategoryId.HasValue)
            {
                var parentCategory = await categoryRepository.GetAsync(dto.ParentCategoryId.Value) ?? throw new NotFoundException($"Parent category with ID '{dto.ParentCategoryId.Value}' not found.");    
                if(!parentCategory.IsSystem && parentCategory.UserId != userId)
                {
                    throw new UnauthorizedException("You do not have permission to assign this parent category.");
                }
            }
            if( await categoryRepository.CategoryExistAsync(dto.Name,userId))
            {
                throw new BadRequestException($"A category with the name '{dto.Name}' already exists.");
            }

            var category = mapper.Map<Category>(dto);

            category.UserId = userId;

            await categoryRepository.AddAsync(category);
            await categoryRepository.SaveChangesAsync();

            return mapper.Map<CategoryResponseDto>(category);
         
        }

        public async Task<CategoryResponseDto> UpdateCategoryAsync(Guid CategoryId, UpdateCategoryRequestDto dto, Guid userId)
        {
            ArgumentNullException.ThrowIfNull(dto);
            if (CategoryId == Guid.Empty)
            {
                throw new ArgumentException("Category ID cannot be empty.", nameof(CategoryId));
            }
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var category = await categoryRepository.GetAsync(CategoryId) ?? throw new NotFoundException($"Category with ID '{CategoryId}' not found.");

            if(category.IsSystem || category.UserId != userId)
            {
                    throw new UnauthorizedException("You do not have permission to update this category.");
            }
            if ( await categoryRepository.CategoryExistAsync(dto.Name,userId))
            {
                throw new BadRequestException($"A category with the name '{dto.Name}' already exists.");
            }
            
            mapper.Map(dto, category);

            await categoryRepository.SaveChangesAsync();

            return mapper.Map<CategoryResponseDto>(category);
        }

        public async Task DeleteCategoryAsync(Guid categoryId, Guid userId) 
        {
            if (categoryId == Guid.Empty)
            {
                throw new ArgumentException("Category ID cannot be empty.", nameof(categoryId));
            }
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var category = await categoryRepository.GetAsync(categoryId) ?? throw new NotFoundException($"Category with ID '{categoryId}' not found.");
            if (category.IsSystem || category.UserId != userId)
            {
                throw new UnauthorizedException("You do not have permission to delete this category.");
            }
            var subCategories = await categoryRepository.GetSubCategoriesAsync(categoryId, userId);
            if (subCategories.Any())
            {
                throw new BadRequestException("Cannot delete category with child categories.");
            }
            var transactionsUsingCategory = await transactionRepository.GetByCategoryAsync(categoryId);
            if (transactionsUsingCategory.Any())
            {
                throw new BadRequestException("Cannot delete category that is used in transactions.");
            }
            category.IsDeleted = true;
            await categoryRepository.SaveChangesAsync();
        }

        public async Task<ConfirmationResponseDto> SetCategoryEssentialAsync(UserCategorySettingsRequestDto dto,Guid categoryId,  Guid userId)
        {
            ArgumentNullException.ThrowIfNull(dto); 
            if(categoryId == Guid.Empty)
            {
                throw new ArgumentException("Category ID cannot be empty.", nameof(categoryId));
            }
            if(userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var category = await categoryRepository.GetAsync(categoryId) ?? throw new NotFoundException($"Category with ID '{categoryId}' not found.");
            if(!category.IsSystem && category.UserId != userId)
            {
                throw new UnauthorizedException("You do not have permission to flag this category.");
            }
            var existingSettings = await userCategorySettingsRepository.GetByUserAsync(userId, categoryId);
            if(existingSettings != null)
            {
                existingSettings.IsEssential = dto.IsEssential;
                userCategorySettingsRepository.Update(existingSettings);          
            } 
            else
            {
                var newUserCategorySettings = mapper.Map<UserCategorySetting>(dto);
                newUserCategorySettings.UserId = userId;
                newUserCategorySettings.CategoryId = categoryId;
                await userCategorySettingsRepository.AddAsync(newUserCategorySettings);
            }
            await userCategorySettingsRepository.SaveChangesAsync();

            return new ConfirmationResponseDto
            {
                Message = $"Category '{category.Name}' has been marked as {(dto.IsEssential ? "essential." : "non-essential")}" 
            };

        }
    }
}
