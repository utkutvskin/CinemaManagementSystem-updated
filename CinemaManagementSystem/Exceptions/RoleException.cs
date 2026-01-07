using CinemaManagementSystem.Enums;

namespace CinemaManagementSystem.Exceptions;

public class RoleException(Role role)
    : Exception($"This employee already has this role {role}");
