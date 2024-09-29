using CivicaBookLibraryApi.Data.Contract;
using CivicaBookLibraryApi.Services.Contract;
using CivicaBookLibraryApi.Dtos;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CivicaBookLibraryApi.Models;
using System.Text.RegularExpressions;
using CivicaBookLibraryApi.Data.Implementation;

namespace CivicaBookLibraryApi.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IVerifyPasswordHashService _verifyPasswordHashService;



        public UserService(IUserRepository userRepository, IVerifyPasswordHashService verifyPasswordHashService)
        {
            _userRepository = userRepository; 
            _verifyPasswordHashService = verifyPasswordHashService;

        }

        public ServiceResponse<string> RegisterUserService(RegisterDto register)
        {
            var response = new ServiceResponse<string>();
            var message = string.Empty;
            if (register != null)
            {
                message = CheckPasswordStrength(register.Password);
               var age = CalculateAge(register.DateOfBirth);
                if (!string.IsNullOrWhiteSpace(message))
                {
                    response.Success = false;
                    response.Message = message;
                    return response;
                }
                else if (_userRepository.UserExists(register.LoginId, register.Email))
                {
                    response.Success = false;
                    response.Message = "User already exists";
                    return response;
                }
                else if (register.DateOfBirth > DateTime.Now)
                {
                    response.Success = false;
                    response.Message = "Date of birth cannot be in future.";
                    return response;
                }
                else if (age < 18 || age > 120)
                {
                    response.Success = false;
                    response.Message = "Age cannot be less than 18 or greater than 120.";
                    return response;
                }
                else
                {
                    User user = new User()
                    {
                        LoginId = register.LoginId,
                        Salutation = register.Salutation,
                        Name = register.Name,
                        Age = age,
                        DateOfBirth = register.DateOfBirth,
                        Gender = register.Gender,
                        Email = register.Email,
                        PhoneNumber = register.PhoneNumber,
                        PasswordHint = register.PasswordHint,
                    };

                    CreatePasswordHash(register.Password, out byte[] passwordHash, out byte[] passwordsalt);
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordsalt;

                    CreatePasswordHash(register.PasswordHintAnswer, out byte[] passwordHintAnswerHash, out byte[] passwordHintAnswerSalt);

                    user.PasswordHintAnswerHash = passwordHintAnswerHash;
                    user.PasswordHintAnswerSalt = passwordHintAnswerSalt;

                    if(_userRepository.UserCount() == 0)
                    {
                        user.IsAdmin = true;
                    }

                    

                    var result = _userRepository.RegisterUser(user);

                    response.Success = result;
                    response.Message = result ? string.Empty : "Something went wrong. Please try after sometime.";
                }
            }
            return response;
        }

        public ServiceResponse<UserDto> GetUserById(int userId)
        {
            var response = new ServiceResponse<UserDto>();
            var user = _userRepository.GetUserById(userId);
            if (user != null)
            {
                var userDto = new UserDto()
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    LoginId = user.LoginId,
                    Salutation = user.Salutation,
                    Age = user.Age,
                    DateOfBirth = user.DateOfBirth,
                    Gender = user.Gender,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    PasswordHint = user.PasswordHint,
                    IsAdmin = user.IsAdmin,
                    SecurityQuestionDto = new SecurityQuestionDto()
                    {
                        PasswordHint = user.PasswordHint,
                        Question = user.SecurityQuestion.Question,
                    }
                };
                response.Success = true;
                response.Data = userDto;
                return response;
            }
            else
            {
                response.Success = false;
                response.Message = "No record found!";
                return response;
            }

        }

        public ServiceResponse<string> LoginUserService(LoginDto login)
        {
            var response = new ServiceResponse<string>();
            string message = string.Empty;
            if (login != null)
            {
                var user = _userRepository.ValidateUser(login.Username);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Invalid user login id or password!";
                    return response;
                }

                else if (!_verifyPasswordHashService.VerifyPasswordHash(login.Password, user.PasswordHash, user.PasswordSalt))
                {
                    response.Success = false;
                    response.Message = "Invalid user login id or password!";
                    return response;
                }
                string token = _verifyPasswordHashService.CreateToken(user);
                response.Success = true;
                response.Data = token;
                return response;
            }
            response.Success= false;
            response.Message = "Something went wrong, Please try after sometime.";
            return response;
        }
          
        private string CheckPasswordStrength(string password)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (password.Length < 8)
            {
                stringBuilder.Append("Mininum password length should be 8" + Environment.NewLine);
            }
            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]")))
            {
                stringBuilder.Append("Password should be alphanumeric" + Environment.NewLine);
            }
            if (!Regex.IsMatch(password, "[<,>,@,!,#,$,%,^,&,*,*,(,),_,]"))
            {
                stringBuilder.Append("Password should contain special characters" + Environment.NewLine);
            }

            return stringBuilder.ToString();
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public ServiceResponse<string> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var response = new ServiceResponse<string>();
            if (changePasswordDto != null)
            {
                var user = _userRepository.ValidateUser(changePasswordDto.LoginId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Something went wrong, please try after some time.";
                    return response;
                }

                if (changePasswordDto.OldPassword == changePasswordDto.NewPassword)
                {
                    response.Success = false;
                    response.Message = "New password cannot be same as old password.";
                    return response;
                }

                if(!_verifyPasswordHashService.VerifyPasswordHash(changePasswordDto.OldPassword, user.PasswordHash, user.PasswordSalt))
                {
                    response.Success = false;
                    response.Message = "Old password is incorrect.";
                    return response;
                }

                CreatePasswordHash(changePasswordDto.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                var result = _userRepository.UpdateUser(user);
                response.Success = result;
                response.Message = result ? "Successfully updated password.Signin again!" : "Something went wrong, please try after some time.";

                var message = CheckPasswordStrength(changePasswordDto.NewPassword);
            }
            else
            {
                response.Success = false;
                response.Message = "Something went wrong, please try after some time.";
            }
            return response;
        }

        public ServiceResponse<string> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var response = new ServiceResponse<string>();
            if (resetPasswordDto != null)
            {
                var user = _userRepository.ValidateUser(resetPasswordDto.LoginId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Invalid loginId!";
                    return response;
                }

                if(resetPasswordDto.PasswordHint != user.PasswordHint)
                {
                    response.Success = false;
                    response.Message = "User verification failed!";
                    return response;
                }

                // Trimming and converting to lowercase
                resetPasswordDto.PasswordHintAnswer = resetPasswordDto.PasswordHintAnswer.Trim();

                if(!_verifyPasswordHashService.VerifyPasswordHash(resetPasswordDto.PasswordHintAnswer,user.PasswordHintAnswerHash,user.PasswordHintAnswerSalt))
                {
                    response.Success = false;
                    response.Message = "User verification failed!";
                    return response;
                }

                var message = CheckPasswordStrength(resetPasswordDto.NewPassword);
                if (!string.IsNullOrWhiteSpace(message))
                {
                    response.Success = false;
                    response.Message = message;
                    return response;
                }

                CreatePasswordHash(resetPasswordDto.NewPassword,out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                var result = _userRepository.UpdateUser(user);
                response.Success = result;
                response.Message = result ? "Successfully updated password." : "Something went wrong, please try after some time.";
            }
            else
            {
                response.Success = false;
                response.Message = "Something went wrong, please try after some time.";
            }
            return response;
        }

        public ServiceResponse<IEnumerable<UserDto>> GetAllUsers()
        {
            var response = new ServiceResponse<IEnumerable<UserDto>>();
            var users = _userRepository.GetAllUsers();
            if (users != null && users.Any())
            {
                List<UserDto> userDtos = new List<UserDto>();
                foreach (var user in users)
                {
                    userDtos.Add(new UserDto()
                    {
                        UserId = user.UserId,
                        LoginId = user.LoginId,
                        Salutation = user.Salutation,
                        Name = user.Name,
                        Age = user.Age,
                        DateOfBirth = user.DateOfBirth,
                        Gender = user.Gender,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,

                    });
                }
                response.Success = true;
                response.Data = userDtos;
            }
            else
            {
                response.Success = false;
                response.Message = "No record found!";
            }
            return response;
        }
        public ServiceResponse<IEnumerable<UserDto>> GetPaginatedUsers(int page, int pageSize, string? search, string sortOrder)
        {
            var response = new ServiceResponse<IEnumerable<UserDto>>();
            var users = _userRepository.GetPaginatedUsers(page, pageSize, search, sortOrder);

            if (users != null && users.Any())
            {
                List<UserDto> userDtos = new List<UserDto>();
                foreach (var user in users)
                {
                    userDtos.Add(new UserDto()
                    {
                        UserId = user.UserId,
                        LoginId = user.LoginId,
                        Salutation = user.Salutation,
                        Name = user.Name,
                        Age = user.Age,
                        DateOfBirth = user.DateOfBirth,
                        Gender = user.Gender,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,

                    });
                }


                response.Data = userDtos;
                response.Success = true;
            }
            else
            {
                response.Success = false;
                response.Message = "No records found";
            }

            return response;
        }
        public ServiceResponse<string> RemoveUser(int id)
        {
            var response = new ServiceResponse<string>();
            var result = _userRepository.DeleteUser(id);

            if (result)
            {
                response.Success = true;
                response.Message = "User deleted successfully";
            }
            else
            {
                response.Success = false;
                response.Message = "Something went wrong";
            }

            return response;
        }
        public ServiceResponse<int> TotalUsers(string? search)
        {
            var response = new ServiceResponse<int>();
            int totalPositions = _userRepository.TotalUsers(search);

            response.Success = true;
            response.Data = totalPositions;
            return response;
        }
        private int CalculateAge(DateTime birthdate)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - birthdate.Year;

            return age;
        }


    }
}
