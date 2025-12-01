using Application.DTOs;
using Domain.Entities;

namespace Application.Factories;

public class UserFactory
{
    public User Create(UserLoginDTO userInfo)
    {
        if (string.IsNullOrEmpty(userInfo.Email) || string.IsNullOrEmpty(userInfo.Password))
        {
            throw new ArgumentNullException("Email and Password is required");
        }
        
        var user = new User
        {
            Email = userInfo.Email,
            Password = userInfo.Password
        };

        return user;
    }
}