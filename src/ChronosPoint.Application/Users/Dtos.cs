// -----------------------------------------------------------------------------
// Copyright (C) 2026 ChronosPoint. Todos os direitos reservados.
// Este codigo e de propriedade exclusiva e confidencial.
// A reproducao ou distribuicao sem autorizacao previa por escrito e proibida.
// -----------------------------------------------------------------------------

using ChronosPoint.Domain.Entities;

namespace ChronosPoint.Application.Users;

public sealed record CreateUserRequest(
    string FullName,
    string Email,
    string Password,
    Guid TenantId,
    TenantRole Role = TenantRole.User);

public sealed record UserResponse(
    Guid Id,
    string FullName,
    string Email,
    bool IsActive,
    DateTimeOffset CreatedAtUtc,
    IReadOnlyList<UserTenantResponse> Tenants);

public sealed record UserTenantResponse(
    Guid TenantId,
    string TenantName,
    TenantRole Role);
