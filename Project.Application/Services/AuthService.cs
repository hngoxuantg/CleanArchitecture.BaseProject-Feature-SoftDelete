using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Project.Application.DTOs;
using Project.Application.Exceptions;
using Project.Domain.Entities;
using Project.Domain.Interfaces.IRepositories;

namespace Project.Application.Services
{
    public class AuthService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        public AuthService(IMapper mapper, IUnitOfWork unitOfWork, RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task<User> Login(LoginDto loginDto, CancellationToken cancellationToken = default)
        {
            User? user = await _unitOfWork.UserRepository.GetQueryAsync<User>(cancellationToken, u => u.UserName == loginDto.UserName)
                ?? throw new NotFoundException($"UserCredentials with username {loginDto.UserName} not found");
            return user;
        }
    }
}
