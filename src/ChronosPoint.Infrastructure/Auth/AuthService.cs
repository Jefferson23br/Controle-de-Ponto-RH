// -----------------------------------------------------------------------------
// Copyright (C) 2026 ChronosPoint. Todos os direitos reservados.
// Este codigo e de propriedade exclusiva e confidencial.
// A reproducao ou distribuicao sem autorizacao previa por escrito e proibida.
// -----------------------------------------------------------------------------

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChronosPoint.Application.Auth;
using ChronosPoint.Domain.Entities;
using ChronosPoint.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ChronosPoint.Infrastructure.Auth;

public sealed class AuthService : IAuthService
{
    private readonly ChronosPointDbContext _db;
    private readonly JwtSettings _jwt;

    public AuthService(ChronosPointDbContext db, IOptions<JwtSettings> jwt)
    {
        _db = db;
        _jwt = jwt.Value;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password) ||
            string.IsNullOrWhiteSpace(request.FullName))
        {
            return AuthResult.Fail("Nome, email e password sao obrigatorios.");
        }

        var emailNormalized = request.Email.Trim().ToUpperInvariant();
        var exists = await _db.Users.AnyAsync(u => u.Email == emailNormalized, ct);
        if (exists)
            return AuthResult.Fail("Ja existe um utilizador com este email.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName.Trim(),
            Email = emailNormalized,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            IsActive = true,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        return AuthResult.Ok(new AuthTokenResponse(
            Token: string.Empty,
            ExpiresAtUtc: DateTime.MinValue,
            User: new UserInfo(user.Id, user.FullName, user.Email),
            Tenant: null!));
    }

    public async Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return AuthResult.Fail("Email e password sao obrigatorios.");

        var emailNormalized = request.Email.Trim().ToUpperInvariant();
        var user = await _db.Users
            .Include(u => u.UserTenants)
                .ThenInclude(ut => ut.Tenant)
            .FirstOrDefaultAsync(u => u.Email == emailNormalized, ct);

        if (user is null || !user.IsActive)
            return AuthResult.Fail("Credenciais invalidas.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return AuthResult.Fail("Credenciais invalidas.");

        var tenants = user.UserTenants
            .Select(ut => new TenantInfo(ut.TenantId, ut.Tenant.Name, ut.Role))
            .ToList();

        if (tenants.Count == 0)
            return AuthResult.Fail("Utilizador nao esta vinculado a nenhuma empresa.");

        if (request.TenantId.HasValue)
        {
            var selected = tenants.FirstOrDefault(t => t.Id == request.TenantId.Value);
            if (selected is null)
                return AuthResult.Fail("Utilizador nao pertence a esta empresa.");

            return AuthResult.Ok(GenerateToken(user, selected));
        }

        if (tenants.Count == 1)
            return AuthResult.Ok(GenerateToken(user, tenants[0]));

        return AuthResult.ChooseTenant(new TenantChoiceResponse(
            new UserInfo(user.Id, user.FullName, user.Email),
            tenants));
    }

    private AuthTokenResponse GenerateToken(User user, TenantInfo tenant)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_jwt.ExpirationMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("fullName", user.FullName),
            new Claim("tenantId", tenant.Id.ToString()),
            new Claim("tenantName", tenant.Name),
            new Claim(ClaimTypes.Role, tenant.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        return new AuthTokenResponse(
            Token: new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAtUtc: expires,
            User: new UserInfo(user.Id, user.FullName, user.Email),
            Tenant: tenant);
    }
}
