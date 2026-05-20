// -----------------------------------------------------------------------------
// Copyright (C) 2026 ChronosPoint. Todos os direitos reservados.
// Este codigo e de propriedade exclusiva e confidencial.
// A reproducao ou distribuicao sem autorizacao previa por escrito e proibida.
// -----------------------------------------------------------------------------

using ChronosPoint.Domain.Entities;

namespace ChronosPoint.Application.Auth;

// --- Requests ---

public sealed record RegisterRequest(
    string FullName,
    string Email,
    string Password);

public sealed record LoginRequest(
    string Email,
    string Password,
    Guid? TenantId = null);

// --- Responses ---

public sealed record AuthTokenResponse(
    string Token,
    DateTime ExpiresAtUtc,
    UserInfo User,
    TenantInfo Tenant);

public sealed record TenantChoiceResponse(
    UserInfo User,
    IReadOnlyList<TenantInfo> Tenants);

public sealed record UserInfo(
    Guid Id,
    string FullName,
    string Email);

public sealed record TenantInfo(
    Guid Id,
    string Name,
    TenantRole Role);
