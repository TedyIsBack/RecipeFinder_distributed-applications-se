﻿using RecipeFinderAPI.Entities;
using RecipeFinderAPI.Infrastructure;
using RecipeFinderAPI.Infrastructure.DTOs.UsersDTOs;
using System.Linq.Expressions;

namespace RecipeFinderAPI.Services.Interfaces
{
    public interface IUserService
    {

        //TODO: ValidateUserCredentialsAsync should return a DTO instead of User??
        //Task<User> ValidateUserCredentialsAsync(string username, string password);
        Task<ResponseUserDto> GetUserByIdAsync(string userId);
        //Task<ResponseUserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task<ResponseUserDto> UpdateUserAsync(string id,UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(string userId);
        Task<PagedResult<ResponseUserDto>> GetAllUsersAsync(Expression<Func<User, bool>> filter = null,
            int page = 1,
            int itemsPerPage = 10);
    }
}
