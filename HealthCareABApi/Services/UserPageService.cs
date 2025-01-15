using HealthCareABApi.Models;
using HealthCareABApi.DTO;
using HealthCareABApi.Repositories;

namespace HealthCareABApi.Services
{
    public class UserPageService
    {
        private readonly IUserRepository _userRepository;

        public UserPageService(IUserRepository userRepository)
        {
            _userRepository = userRepository;

        }

        public async Task<UserDto> GetUserInformationAsync(string userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            return new UserDto
            {
                Username = user.Username,
                Roles = user.Roles,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth
            };
        }

        public async Task<bool> UpdateUserInformationAsync(string userId, UserDto userDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            //Uppdaterar användarens information (förutsatt att de ändrat något, annars behålls gamla värdet)
            if (!string.IsNullOrEmpty(userDto.Username)) user.Username = userDto.Username;
            if (!string.IsNullOrEmpty(userDto.FirstName)) user.FirstName = userDto.FirstName;
            if (!string.IsNullOrEmpty(userDto.LastName)) user.LastName = userDto.LastName;
            if (!string.IsNullOrEmpty(userDto.Email)) user.Email = userDto.Email;
            if (!string.IsNullOrEmpty(userDto.Phone)) user.Phone = userDto.Phone;
            if (!string.IsNullOrEmpty(userDto.Address)) user.Address = userDto.Address;

            await _userRepository.UpdateAsync(userId, user);

            return true;
        }

    }
}
