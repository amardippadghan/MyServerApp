using Dapper;
using MySqlConnector;
using MyServerApp.Models;
using MyServerApp.Models.DTOs;
using MyServerApp.Helpers;

namespace MyServerApp.Services
{
    public class UserService : IUserService
    {
        private readonly string _connectionString;

        public UserService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            using var connection = new MySqlConnection(_connectionString);
            var users = await connection.QueryAsync<User>(
                "SELECT Id, Name, Email, Phone, Type, CreatedAt, UpdatedAt FROM Users ORDER BY CreatedAt DESC"
            );

            return users.Select(MapToResponseDto);
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            var user = await connection.QueryFirstOrDefaultAsync<User>(
                "SELECT Id, Name, Email, Phone, Type, CreatedAt, UpdatedAt FROM Users WHERE Id = @id",
                new { id }
            );

            return user != null ? MapToResponseDto(user) : null;
        }

        public async Task<UserResponseDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            using var connection = new MySqlConnection(_connectionString);
            
            var hashedPassword = PasswordHelper.HashPassword(createUserDto.Password);
            var now = DateTime.UtcNow;

            var sql = @"
                INSERT INTO Users (Name, Email, Phone, Password, Type, CreatedAt, UpdatedAt) 
                VALUES (@Name, @Email, @Phone, @Password, @Type, @CreatedAt, @UpdatedAt);
                SELECT LAST_INSERT_ID();";

            var userId = await connection.QuerySingleAsync<int>(sql, new
            {
                createUserDto.Name,
                createUserDto.Email,
                createUserDto.Phone,
                Password = hashedPassword,
                Type = (int)createUserDto.Type,
                CreatedAt = now,
                UpdatedAt = now
            });

            var user = await connection.QueryFirstAsync<User>(
                "SELECT Id, Name, Email, Phone, Type, CreatedAt, UpdatedAt FROM Users WHERE Id = @id",
                new { id = userId }
            );

            return MapToResponseDto(user);
        }

        public async Task<bool> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            using var connection = new MySqlConnection(_connectionString);
            
            var setParts = new List<string>();
            var parameters = new DynamicParameters();
            parameters.Add("@id", id);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);

            if (!string.IsNullOrEmpty(updateUserDto.Name))
            {
                setParts.Add("Name = @Name");
                parameters.Add("@Name", updateUserDto.Name);
            }

            if (!string.IsNullOrEmpty(updateUserDto.Email))
            {
                setParts.Add("Email = @Email");
                parameters.Add("@Email", updateUserDto.Email);
            }

            if (!string.IsNullOrEmpty(updateUserDto.Phone))
            {
                setParts.Add("Phone = @Phone");
                parameters.Add("@Phone", updateUserDto.Phone);
            }

            if (!string.IsNullOrEmpty(updateUserDto.Password))
            {
                setParts.Add("Password = @Password");
                parameters.Add("@Password", PasswordHelper.HashPassword(updateUserDto.Password));
            }

            if (updateUserDto.Type.HasValue)
            {
                setParts.Add("Type = @Type");
                parameters.Add("@Type", (int)updateUserDto.Type.Value);
            }

            if (setParts.Count == 0) return false;

            setParts.Add("UpdatedAt = @UpdatedAt");

            var sql = $"UPDATE Users SET {string.Join(", ", setParts)} WHERE Id = @id";
            var affectedRows = await connection.ExecuteAsync(sql, parameters);

            return affectedRows > 0;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            using var connection = new MySqlConnection(_connectionString);
            var affectedRows = await connection.ExecuteAsync(
                "DELETE FROM Users WHERE Id = @id", 
                new { id }
            );

            return affectedRows > 0;
        }

        public async Task<IEnumerable<UserResponseDto>> GetUsersByTypeAsync(UserType type)
        {
            using var connection = new MySqlConnection(_connectionString);
            var users = await connection.QueryAsync<User>(
                "SELECT Id, Name, Email, Phone, Type, CreatedAt, UpdatedAt FROM Users WHERE Type = @type ORDER BY CreatedAt DESC",
                new { type = (int)type }
            );

            return users.Select(MapToResponseDto);
        }

        private static UserResponseDto MapToResponseDto(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Type = user.Type,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}
