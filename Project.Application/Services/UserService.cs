using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Project.Application.Interfaces.IServices;
using Project.Domain.Entities;
using Project.Domain.Interfaces.IRepositories;

namespace Project.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        public UserService(IMapper mapper, IUnitOfWork unitOfWork, RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _userManager = userManager;
        }
    }
}
