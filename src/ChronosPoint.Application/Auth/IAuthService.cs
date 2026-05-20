// -----------------------------------------------------------------------------
// Copyright (C) 2026 ChronosPoint. Todos os direitos reservados.
// Este codigo e de propriedade exclusiva e confidencial.
// A reproducao ou distribuicao sem autorizacao previa por escrito e proibida.
// -----------------------------------------------------------------------------

namespace ChronosPoint.Application.Auth;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<AuthResult> LoginAsync(LoginRequest request, CancellationToken ct = default);
}

/// <summary>
/// Resultado generico de operacao de autenticacao.
/// Sucesso com token, escolha de tenant, ou erro.
/// </summary>
public sealed class AuthResult
{
    public bool Success { get; init; }
    public string? Error { get; init; }
    public AuthTokenResponse? Token { get; init; }
    public TenantChoiceResponse? TenantChoice { get; init; }

    public static AuthResult Ok(AuthTokenResponse token) => new() { Success = true, Token = token };
    public static AuthResult ChooseTenant(TenantChoiceResponse choice) => new() { Success = true, TenantChoice = choice };
    public static AuthResult Fail(string error) => new() { Success = false, Error = error };
}
