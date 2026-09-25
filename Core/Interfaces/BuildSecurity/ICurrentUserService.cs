namespace Core.Interfaces.BuildSecurity
{
	using System;

	public interface ICurrentUserService
	{
		Guid? UserId { get; }

		string? IdentityUserId { get; }

		string? Email { get; }

		string? Nombre { get; }

		int? YunkeId { get; }

		Guid? YonkeGuidId { get; }
	}
}
