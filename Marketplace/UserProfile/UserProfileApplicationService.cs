using System;
using System.Threading.Tasks;
using Marketplace.Domain.Shared;
using Marketplace.Domain.UserProfile;
using Marketplace.Framework;
using MarketPlace.Framework;
using static Marketplace.Domain.Shared.ContentModeration;

namespace Marketplace.UserProfile
{
    public class UserProfileApplicationService : IApplicationService
    {
        private readonly IUserProfileRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly CheckTextForProfanity _checkText;

        public UserProfileApplicationService(IUserProfileRepository repository, IUnitOfWork unitOfWork, CheckTextForProfanity checkText)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _checkText = checkText;
        }

        public async Task Handle(object command)
        {
            switch (command)
            {
                case Contracts.V1.RegisterUser cmd:
                    await HandleCreate(cmd);
                    break;
                case Contracts.V1.UpdateUserFullName cmd:
                    await HandleUpdate(cmd.UserId,
                        profile => profile.UpdateFullName(FullName.FromString(cmd.FullName)));
                    break;
                case Contracts.V1.UpdateUserDisplayName cmd:
                    await HandleUpdate(cmd.UserId,
                        profile => profile.UpdateDisplayName(DisplayName.FromString(cmd.DisplayName, _checkText)));
                    break;
                case Contracts.V1.UpdateUserProfilePhoto cmd:
                    await HandleUpdate(cmd.UserId, profile => profile.UpdateProfilePhoto(new Uri(cmd.PhotoUrl)));
                    break;
                default:
                    throw new InvalidOperationException($"Command type {command.GetType().FullName} is unknown");
            }
        }

        private async Task HandleUpdate(Guid userProfleId, Action<Domain.UserProfile.UserProfile> operation)
        {
            var userProfile = await _repository.Load(new UserId(userProfleId));
            
            if(userProfile == null)
                throw new InvalidOperationException($"Entity with id {userProfleId} cannot be found");

            operation(userProfile);

            await _unitOfWork.Commit();
        }

        private async Task HandleCreate(Contracts.V1.RegisterUser cmd)
        {
            if(await _repository.Exists(new UserId(cmd.UserId)))
                throw new InvalidOperationException($"Entity with id {cmd.UserId} already exists");

            var userProfile = new Domain.UserProfile.UserProfile(
                new UserId(cmd.UserId),
                FullName.FromString(cmd.FullName), 
                DisplayName.FromString(cmd.DisplayName, _checkText));

            await _repository.Add(userProfile);
            await _unitOfWork.Commit();
        }
    }
}