using Core.Entitys;
using Core.Interfaces.Negocio.Calificacion;
using Core.Interfaces.Negocio.Coberturas;
using Core.Interfaces.Negocio.YunkeDispotivos;
using Microsoft.EntityFrameworkCore.Storage;

namespace Core.Interfaces.Negocio
{
    public interface IUnitOfWorkNegocio : IDisposable
    {
		IRepositrioYunke<Yonkes> YunkeRepository { get;  }
		IYunkeCoberturaRepository YunkeCoberturaRepository { get; }
		IYunkeDispositivoRepository YunkeDispositivoRepository { get; }
		IYunkeCalificacionRepository YunkeCalificacionRepository { get; }

		Task<int> SaveChangesAsync();

		Task<int> SaveChangesAsync(CancellationToken cancellationToken);


		Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
	}
}
