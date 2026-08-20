using fintech.API.Application.DTOs.UserCategorySettingDtos;
using fintech.API.Domain.Entities;

namespace fintech.API.Application.Mappings
{
    public static class UserCategorySettingMapper
    {
        public static UserCategorySetting MapToEntity(this UserCategorySettingsRequestDto dto)
        {
            return new UserCategorySetting
            {
                IsEssential = dto.IsEssential
            };
        }
    }
}
