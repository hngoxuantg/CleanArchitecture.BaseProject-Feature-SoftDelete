﻿using Microsoft.AspNetCore.Identity;
using Project.Application.DTOs;
using Project.Application.Exceptions;
using Project.Application.Interfaces.IServices;
using Project.Domain.Entities;
using Project.Domain.Interfaces.IRepositories;

namespace Project.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        public AuthService(IUnitOfWork unitOfWork, UserManager<User> userManager, IJwtTokenService jwtTokenService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<(string, string)> LoginAsync(LoginDto loginDto, string deviceInfo, string ipAddress, CancellationToken cancellationToken = default)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                User? user = await _unitOfWork.UserRepository.GetOneAsync<User>(filter: u => u.UserName == loginDto.UserName, cancellation: cancellationToken)
                    ?? throw new NotFoundException($"User with UserName {loginDto.UserName} not found");

                if (!await _userManager.CheckPasswordAsync(user, loginDto.Password))
                    throw new ValidatorException($"Invalid password for user {loginDto.UserName}");

                string accessToken = await _jwtTokenService.GenerateJwtTokenAsync(user, cancellationToken);
                string refreshToken = _jwtTokenService.GenerateRefreshToken();

                RefreshToken newToken = new RefreshToken(user.Id, refreshToken, DateTime.UtcNow.AddDays(7), deviceInfo, ipAddress);

                _unitOfWork.RefreshTokenRepository.AddEntity(newToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return (accessToken, refreshToken);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        public async Task<bool> LogoutAsync(string? refreshToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(refreshToken))
                throw new ValidatorException("Refresh token is required");

            RefreshToken? oldToken = await _unitOfWork.RefreshTokenRepository.GetOneAsync<RefreshToken>(
                filter: r => r.Token == refreshToken,
                cancellation: cancellationToken) ?? throw new NotFoundException("Refresh token not found");

            if (!oldToken.IsActive)
                throw new ValidatorException("Refresh token is not active or has expired");

            oldToken.Revoke();

            await _unitOfWork.RefreshTokenRepository.UpdateAsync(oldToken, cancellationToken);

            return true;
        }

        public async Task<(string, string)> RefreshAsync(string? refreshToken, string? deviceInfo, string? ipAddress, CancellationToken cancellationToken = default)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                RefreshToken? oldToken = await _unitOfWork.RefreshTokenRepository.GetOneAsync<RefreshToken>(
                    filter: r => r.Token == refreshToken,
                    cancellation: cancellationToken) ?? throw new NotFoundException("Refresh token not found");

                if (!oldToken.IsActive)
                    throw new ValidatorException("Refresh token is not active or has expired");

                User? user = await _unitOfWork.UserRepository.GetByIdAsync(oldToken.UserId, cancellation: cancellationToken)
                    ?? throw new NotFoundException("User not found");

                oldToken.Revoke();

                string accessToken = await _jwtTokenService.GenerateJwtTokenAsync(user, cancellationToken);
                string newRefreshToken = _jwtTokenService.GenerateRefreshToken();

                RefreshToken newRefreshTokenEntity = new RefreshToken(user.Id, newRefreshToken, DateTime.UtcNow.AddDays(7), deviceInfo, ipAddress);
                _unitOfWork.RefreshTokenRepository.AddEntity(newRefreshTokenEntity);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return (accessToken, newRefreshToken);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}
